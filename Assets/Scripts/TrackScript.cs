using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackScript : MonoBehaviour, IVirtualButtonEventHandler {

    public GameObject TrackSlider;

    private GameObject TrackSliderInstance;
    private GameObject ImageTargetTrackStart;
    private GameObject ImageTargetTrackEnd;
    private TrackTargetTrackableEventHandler ImageTargetTrackStartTrackable;
    private TrackTargetTrackableEventHandler ImageTargetTrackEndTrackable;
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    private LineRenderer MainTrackLine;
    private List<GameObject> TrackLines = new List<GameObject>();
    private List<Note> NoteList = new List<Note>();
    private int lastPlayedNoteIndex = -1;
    private const float TRACK_DURATION = 8f;            //playtime of the track in seconds
    private const float VOLUME_DISTANCE = 0.3f;         //distance of note from main track line for full volume
    private bool playing = false;
    private float playStartTimestamp;

    // Use this for initialization
    void Start () {

        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }

        ImageTargetTrackStart = GameObject.FindGameObjectWithTag("ImageTargetTrackStart");
        ImageTargetTrackStartTrackable = ImageTargetTrackStart.GetComponent<TrackTargetTrackableEventHandler>();
        ImageTargetTrackEnd = GameObject.FindGameObjectWithTag("ImageTargetTrackEnd");
        ImageTargetTrackEndTrackable = ImageTargetTrackEnd.GetComponent<TrackTargetTrackableEventHandler>();

        //create track line object
        MainTrackLine = this.gameObject.AddComponent<LineRenderer>();
        // Set the width of the Line Renderer
        MainTrackLine.SetWidth(0.005F, 0.015F);
        MainTrackLine.SetColors(new Color(255, 255, 255), new Color(0, 255, 0));
        // Set the number of vertex fo the Line Renderer
        MainTrackLine.SetVertexCount(2);
        //draw trackline
        MainTrackLine.SetVertexCount(2);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //clear all linerenderers
        foreach(GameObject lineObject in TrackLines)
        {
            Destroy(lineObject);
        }

        TrackLines.Clear();

        MainTrackLine.SetVertexCount(0);

        if (ImageTargetTrackStart.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED && ImageTargetTrackEnd.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED)
        {
            MainTrackLine.SetVertexCount(2);
            MainTrackLine.SetPosition(0, ImageTargetTrackStart.transform.position);
            MainTrackLine.SetPosition(1, ImageTargetTrackEnd.transform.position);

            /*
             * calculate note positions
             */

            LineRenderer NoteLine;

            //calculate some track metrics
            float trackLength = Vector3.Distance(ImageTargetTrackEnd.transform.position, ImageTargetTrackStart.transform.position);

            //for each target
            foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
            {

                if (Note.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
                {
                    /*
                    Note.transform.rotation = Quaternion.FromToRotation(ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position);
                    Note.transform.Translate(0, trackSurfaceVector.y, 0);
                    
                    */


                    //Vector3 PlaneNormal = GetNormal(ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position, Note.transform.position);
                    //Note.transform.position = Vector3.ProjectOnPlane(Note.transform.position, PlaneNormal);

                    Debug.Log("Note Position:");
                    Debug.Log(Note.transform.position);

                    //create surface vector to project note onto
                    Vector3 trackSurfaceVector = ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position;

                    //get Normal of plane
                    Vector3 PlaneNormal = ImageTargetTrackStart.GetComponentInChildren<MeshFilter>().mesh.normals[0];

                    //project note onto plane
                    Vector3 v = Note.transform.position - trackSurfaceVector;
                    Vector3 d = Vector3.Project(v, PlaneNormal);
                    Note.transform.position = Note.transform.position - d;

                    //align note rotation to plane rotation
                    Note.transform.rotation = ImageTargetTrackStart.transform.rotation;

                    Debug.Log("Plane Normal:");
                    Debug.Log(Note.transform.position);
                    Debug.Log("Projected Note:");
                    Debug.Log(Note.transform.position);

                    //get orthogonal angle from trackVector to note
                    Vector3 noteTrackPosition = NearestPointOnLine(ImageTargetTrackStart.transform.position, (ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position).normalized, Note.transform.position);


                    Debug.Log("Track Position:");
                    Debug.Log(noteTrackPosition);

                    //create note obj
                    float noteTrackDistance = Vector3.Distance(ImageTargetTrackStart.transform.position, noteTrackPosition) <= trackLength && Vector3.Distance(ImageTargetTrackEnd.transform.position, noteTrackPosition) <= trackLength ? Vector3.Distance(ImageTargetTrackStart.transform.position, noteTrackPosition) : -Vector3.Distance(ImageTargetTrackEnd.transform.position, noteTrackPosition); //Distance is always positive, so make it negative if it is larger than the total tracklength
                    float noteTrackTimestamp = Mathf.Ceil((TRACK_DURATION / trackLength * noteTrackDistance) * 100) / 100;
                    float volume = Mathf.Ceil((1 / VOLUME_DISTANCE * Vector3.Distance(noteTrackPosition, Note.transform.position)) * 100) / 100;

                    Debug.Log("Track Timestamp:" + noteTrackTimestamp.ToString());
                    Debug.Log("Volume:" + volume.ToString());

                    //proceed if note is on the maintrackline
                    if (noteTrackTimestamp > 0 && noteTrackTimestamp <= TRACK_DURATION) { 
                        //draw noteline
                        TrackLines.Add(new GameObject());
                        NoteLine = TrackLines[TrackLines.Count - 1].gameObject.AddComponent<LineRenderer>();
                        // Set the width of the Line Renderer
                        NoteLine.SetWidth(0.005F, 0.015F);
                        NoteLine.SetColors(new Color(0, 0, 255), new Color(0, 255, 255));
                        // Set the number of vertex fo the Line Renderer
                        NoteLine.SetVertexCount(2);

                        NoteLine.SetPosition(0, noteTrackPosition);
                        NoteLine.SetPosition(1, Note.transform.position);

                        NoteList.Add(new global::Note(Note, noteTrackTimestamp, 1, volume));

                        //set infotexts
                        GetChildObject(Note.transform, "TrackTimestampInfoText").GetComponent<TextMesh>().text = noteTrackTimestamp.ToString() + "s";

                        Debug.Log("Note saved");
                    } else
                    {
                        Debug.Log("Note not saved");
                    }
                }
            }

            /*
             * play sounds
             */

            if (playing)
            {
                for (int i = lastPlayedNoteIndex + 1; i < NoteList.Count; i++)
                {
                    float playTimestamp = Time.time - playStartTimestamp;

                    //play note and break if it is due
                    if (playTimestamp <= NoteList[i].TrackTimestamp)
                    {
                        Debug.Log("Play Note " + NoteList[i].ImageTarget.GetComponent<ImageTargetBehaviour>().name);
                        TrackSliderInstance.transform.position = Vector3.MoveTowards(ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position, trackLength / TRACK_DURATION * playTimestamp);
                        AudioSource NoteToPlay = NoteList[i].ImageTarget.GetComponent<AudioSource>();
                        NoteToPlay.volume = NoteList[i].Volume;
                        NoteToPlay.pitch = NoteList[i].Pitch;
                        NoteToPlay.Play();
                        lastPlayedNoteIndex = i;
                        break;
                    }
                }

                //rewind if loop is played
                if(lastPlayedNoteIndex == NoteList.Count - 1)
                {
                    lastPlayedNoteIndex = -1;
                    playStartTimestamp = Time.time;
                }
            }
        }
    }

    private void togglePlaying()
    {
        playing = !playing;
        playStartTimestamp = Time.time;

        if(playing)
        {
            GameObject TrackSliderInstance = Instantiate(TrackSlider);
            TrackSliderInstance.transform.position = ImageTargetTrackStart.transform.position;
            TrackSliderInstance.transform.rotation = ImageTargetTrackStart.transform.rotation;

        } else
        {
            Destroy(TrackSliderInstance);
        }
    }

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;
        return Vector3.Cross(side1, side2).normalized;
    }

    void SetButtonMaterial(float transparency, VirtualButtonBehaviour vb)
    {
        // Set the Virtual Button material
        if (transparency != null)
        {
            Color color = vb.GetComponent<MeshRenderer>().material.color;
            vb.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, transparency);
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        SetButtonMaterial(0.5f, vb);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        togglePlaying();
        SetButtonMaterial(1f, vb);
    }

    private GameObject GetChildObject(Transform parent, string _tag)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                return child.gameObject;
            }
        }

        return null;
    }
}

class Note
{
    public float TrackTimestamp;
    public float Pitch;
    public float Volume;
    public GameObject ImageTarget;

    public Note(GameObject obj, float timestamp, float pitch, float volume)
    {
        ImageTarget = obj;
        TrackTimestamp = timestamp;
        Pitch = pitch;
        Volume = volume;
    }
}
