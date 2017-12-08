using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackScript : MonoBehaviour {

    public GameObject TrackSlider;

    private GameObject ImageTargetTrackStart;
    private GameObject ImageTargetTrackEnd;
    private TrackTargetTrackableEventHandler ImageTargetTrackStartTrackable;
    private TrackTargetTrackableEventHandler ImageTargetTrackEndTrackable;
    private LineRenderer MainTrackLine;
    private List<GameObject> TrackLines = new List<GameObject>();
    private List<Note> NoteList = new List<Note>();
    private const float TRACK_DURATION = 8f;            //playtime of the track in seconds

    // Use this for initialization
    void Start () {
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
	void Update ()
    {
        //clear all linerenderers
        foreach(GameObject lineObject in TrackLines)
        {
            Destroy(lineObject);
        }

        TrackLines.Clear();

        if (ImageTargetTrackStart.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED && ImageTargetTrackEnd.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED)
        {

            MainTrackLine.SetPosition(0, ImageTargetTrackStart.transform.position);
            MainTrackLine.SetPosition(1, ImageTargetTrackEnd.transform.position);

            /*
             * calculate note positions
             */

            LineRenderer NoteLine;

            //for each vumark
            foreach (GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
            {
                if (Note.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
                {
                    //translate note onto surface between trackstart and trackend
                    Vector3 trackSurfaceVector = ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position;
                    Note.transform.rotation = Quaternion.FromToRotation(ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position);
                    Note.transform.Translate(0, trackSurfaceVector.y, 0);

                    //get orthogonal angle from trackVector to note
                    Vector3 noteTrackPosition = NearestPointOnLine(ImageTargetTrackStart.transform.position, ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position, Note.transform.position);
                    Debug.Log(noteTrackPosition);

                    //draw noteline
                    TrackLines.Add(new GameObject());
                    NoteLine = TrackLines[TrackLines.Count - 1].gameObject.AddComponent<LineRenderer>();
                    // Set the width of the Line Renderer
                    NoteLine.SetWidth(0.005F, 0.015F);
                    NoteLine.SetColors(new Color(0, 255, 255), new Color(0, 255, 0));
                    // Set the number of vertex fo the Line Renderer
                    NoteLine.SetVertexCount(2);

                    NoteLine.SetPosition(0, noteTrackPosition);
                    NoteLine.SetPosition(1, Note.transform.position);
                }
            }

            /*
             * calculate sounds
             */
        }
    }

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
}

class Note
{
    public float TrackTimestamp;
    public float Pitch;
    public float Volume;

    public Note(float timestamp, float pitch, float volume)
    {
        TrackTimestamp = timestamp;
        Pitch = pitch;
        Volume = volume;
    }
}
