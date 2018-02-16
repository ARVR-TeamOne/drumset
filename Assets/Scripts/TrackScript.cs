using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackScript : MonoBehaviour {

    public GameObject TrackSlider;
    public GameObject ProjectedNote;

    private GameObject TrackSliderInstance;
    private List<GameObject> ProjectedNoteInstances = new List<GameObject>();
    private GameObject ImageTargetTrackStart;
    private GameObject ImageTargetTrackEnd;
    private GameObject TrackPlane;
    private TrackTargetTrackableEventHandler ImageTargetTrackStartTrackable;
    private TrackTargetTrackableEventHandler ImageTargetTrackEndTrackable;
    private LineRenderer MainTrackLine;
    private List<GameObject> TrackLines = new List<GameObject>();
    private List<Note> NoteList = new List<Note>();
    private int lastPlayedNoteIndex = -1;
    private const float TRACK_DURATION = 8f;            //playtime of the track in seconds
    private const float VOLUME_DISTANCE = 0.2f;         //distance of note from main track line for full volume
    private bool playing = false;
    private float playStartTimestamp;
    private bool looping;

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

        //create previewplane
        TrackPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        TrackPlane.name = "TrackPlane";
        TrackPlane.transform.position = new Vector3(-1, -1, -1);
        TrackPlane.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
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

        //clear all projected notes
        foreach (GameObject projectedNoteObject in ProjectedNoteInstances)
        {
            Destroy(projectedNoteObject);
        }

        ProjectedNoteInstances.Clear();

        MainTrackLine.SetVertexCount(0);

        //if start and end marker are tracked
        if (ImageTargetTrackStart.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED && ImageTargetTrackEnd.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED)
        {
            MainTrackLine.SetVertexCount(2);
            MainTrackLine.SetPosition(0, ImageTargetTrackStart.transform.position);
            MainTrackLine.SetPosition(1, ImageTargetTrackEnd.transform.position);

            //DEBUG
            /*
            Debug.Log("Start Position:");
            Debug.Log(ImageTargetTrackStart.transform.position);
            Debug.Log("End Position:");
            Debug.Log(ImageTargetTrackEnd.transform.position);
            */

            /*
             * calculate note positions
             */

            LineRenderer NoteLine;

            //calculate track length
            float trackLength = Vector3.Distance(ImageTargetTrackEnd.transform.position, ImageTargetTrackStart.transform.position);

            //get track middle point
            Vector3 TrackMiddleVector = (ImageTargetTrackStart.transform.position + ImageTargetTrackEnd.transform.position) / 2;

            //get average rotation of track
            Quaternion TrackRotation = new Quaternion((ImageTargetTrackStart.transform.rotation.x + ImageTargetTrackEnd.transform.rotation.x) / 2, (ImageTargetTrackStart.transform.rotation.y + ImageTargetTrackEnd.transform.rotation.y) / 2, (ImageTargetTrackStart.transform.rotation.z + ImageTargetTrackEnd.transform.rotation.z) / 2, (ImageTargetTrackStart.transform.rotation.w + ImageTargetTrackEnd.transform.rotation.w) / 2);

            //get normal of track
            Vector3 TrackNormal = TrackPlane.GetComponent<MeshFilter>().mesh.normals[0].normalized;

            Vector3 TrackMeshPoint = TrackPlane.transform.TransformPoint(TrackPlane.GetComponent<MeshFilter>().mesh.vertices[0]);

            TrackPlane.transform.position = TrackMiddleVector;
            TrackPlane.transform.rotation = TrackRotation;
            TrackPlane.transform.localScale = new Vector3(trackLength * 0.1f, 0.1f, 0.03f);

            //DEBUG
            /*
            Debug.Log("Track Middle Vector");
            Debug.Log(TrackMiddleVector);

            Debug.Log("Track Rotation");
            Debug.Log(TrackRotation);

            Debug.Log("Track Normal");
            Debug.Log(TrackNormal);
            */

            //create surface vector to project note onto
            //Vector3 trackSurfaceVector = ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position;

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

                    //center note position
                    Note.transform.position = GetChildObject(Note.transform, "NoteCenter").transform.position;

                    //get Normal of plane
                    //Vector3 PlaneNormal = Vector3.Cross(GetChildObject(ImageTargetTrackStart.transform, "Normal").transform.position - ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position - ImageTargetTrackStart.transform.position);

                    //project note onto plane
                    /*Vector3 v = Note.transform.position - trackSurfaceVector;
                    Vector3 d = Vector3.Project(v, PlaneNormal);
                    Vector3 ProjectedNoteVector = Note.transform.position - d;
                    */

                    Vector3 ProjectedNoteVector = new Vector3(0, 0, 0);

                    ProjectedNoteVector = GetClosestPointOnSurface(Note.transform.position, ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position, TrackMeshPoint);

                    //align note rotation to plane rotation
                    //Note.transform.rotation = ImageTargetTrackStart.transform.rotation;

                    //get orthogonal angle from trackVector to note
                    Vector3 noteTrackPosition = NearestPointOnLine(ImageTargetTrackStart.transform.position, (ImageTargetTrackStart.transform.position - ImageTargetTrackEnd.transform.position).normalized, ProjectedNoteVector);

                    /*
                     * create note obj
                     * 1. Get Distance noteTrackPosition to trackStartPosition
                     * 2. Calculate timestamp from that Distance
                     * 3. Get Distance From Projected Note to TrackLine, Compute Volume From that Distance
                     * 4. Project Note Rotation on TrackPlane, save Y-Rotatation as Pitch
                     */

                    //1
                    float noteTrackDistanceStart = Vector3.Distance(ImageTargetTrackStart.transform.position, noteTrackPosition);
                    float noteTrackDistanceEnd = Vector3.Distance(ImageTargetTrackEnd.transform.position, noteTrackPosition);
                    float noteTrackTimestamp = 0f;

                    //DEBUG
                    /*
                    Debug.Log("Note Position:");
                    Debug.Log(Note.transform.position);
                    Debug.Log("Projected Note:");
                    Debug.Log(ProjectedNoteVector);
                    Debug.Log("Track Position:");
                    Debug.Log(noteTrackPosition);
                    Debug.Log("Track Length: " + trackLength.ToString());
                    Debug.Log("Note Distance Start: " + noteTrackDistanceStart.ToString());
                    Debug.Log("Note Distance End: " + noteTrackDistanceEnd.ToString());
                    */

                    //2
                    //check if noteTrackPosition is inbetween trackstart and trackend
                    if (noteTrackDistanceStart <= trackLength && noteTrackDistanceEnd <= trackLength)
                    {
                        noteTrackTimestamp = Mathf.Ceil((TRACK_DURATION / trackLength * noteTrackDistanceStart) * 10) / 10;
                    } else
                    {
                        Debug.Log("Note is not on Trackline, skipping");
                        continue;
                    }

                    //3
                    float volume = Mathf.Ceil((1 / VOLUME_DISTANCE * Vector3.Distance(noteTrackPosition, ProjectedNoteVector)) * 10) / 10;

                    //4
                    Quaternion projectedNoteRotation = Quaternion.Inverse(TrackPlane.transform.rotation) * Note.transform.rotation;
                    float pitch = Mathf.Ceil((projectedNoteRotation.y + 1f) * 100) / 100;

                    //DEBUG
                    /*
                    Debug.Log("Note Rotation:");
                    Debug.Log(Note.transform.rotation);
                    Debug.Log("Projected Note Rotation:");
                    Debug.Log(projectedNoteRotation);
                    Debug.Log("Track Timestamp:" + noteTrackTimestamp.ToString());
                    Debug.Log("Track Timestamp:" + noteTrackTimestamp.ToString());
                    Debug.Log("Track Distance:" + noteTrackDistanceStart.ToString());
                    Debug.Log("Volume:" + volume.ToString());
                    Debug.Log("Pitch:" + pitch.ToString());
                    */

                    //create projectNoteObject where note is projected onto trackPlane
                    GameObject ProjectedNoteInstance = Instantiate(ProjectedNote);
                    ProjectedNoteInstance.transform.position = ProjectedNoteVector;
                    ProjectedNoteInstances.Add(ProjectedNoteInstance);
                    
                    //draw line from projected note to trackline
                    TrackLines.Add(new GameObject());
                    NoteLine = TrackLines[TrackLines.Count - 1].gameObject.AddComponent<LineRenderer>();
                    // Set the width of the Line Renderer
                    NoteLine.SetWidth(0.005F, 0.015F);
                    NoteLine.SetColors(new Color(0, 0, 255), new Color(0, 255, 255));
                    // Set the number of vertex fo the Line Renderer
                    NoteLine.SetVertexCount(2);

                    NoteLine.SetPosition(0, noteTrackPosition);
                    NoteLine.SetPosition(1, ProjectedNoteVector);

                    NoteList.Add(new global::Note(Note, noteTrackTimestamp, pitch, volume));

                    //set infotexts
                    GetChildObject(Note.transform, "TrackTimestampInfoText").GetComponent<TextMesh>().text = noteTrackTimestamp.ToString() + "s";
                    GetChildObject(Note.transform, "TrackVolumeInfoText").GetComponent<TextMesh>().text = volume.ToString();
                    GetChildObject(Note.transform, "TrackPitchInfoText").GetComponent<TextMesh>().text = pitch.ToString();
                    
                    //DEBUG
                    /*
                    Debug.Log("Note saved");
                    Debug.Log(NoteList[NoteList.Count-1]);
                    */
                }
            }


            /*
             * play sounds
             */

            if (playing)
            {

                float playTimestamp = Time.time - playStartTimestamp;
                Debug.Log("Play Timestamp: " + playTimestamp.ToString());
                Debug.Log("Slider Delta:");
                Debug.Log(trackLength / TRACK_DURATION * playTimestamp);
                TrackSliderInstance.transform.position = Vector3.MoveTowards(ImageTargetTrackStart.transform.position, ImageTargetTrackEnd.transform.position, trackLength / TRACK_DURATION * playTimestamp);

                for (int i = lastPlayedNoteIndex + 1; i < NoteList.Count; i++)
                {
                    //play note and break if it is due
                    if (playTimestamp >= NoteList[i].TrackTimestamp)
                    {
                        Debug.Log("Play Note " + NoteList[i].ImageTarget.GetComponent<ImageTargetBehaviour>().name);
                        AudioSource NoteToPlay = NoteList[i].ImageTarget.GetComponent<AudioSource>();
                        NoteToPlay.volume = NoteList[i].Volume;
                        NoteToPlay.pitch = NoteList[i].Pitch;
                        NoteToPlay.Play();
                        lastPlayedNoteIndex = i;
                        break;
                    }
                }

                //rewind if loop is played
                if (playTimestamp >= TRACK_DURATION && looping)
                {
                    lastPlayedNoteIndex = -1;
                    playStartTimestamp = Time.time;
                }
            }
        } else
        {
            TrackPlane.transform.position = new Vector3(-1, -1, -1);

            //stop playing if track is lost
            if (playing) togglePlaying();
        }

        //move trackplane behind the camera before rendering it
        TrackPlane.transform.position = new Vector3(-1, -1, -1);
    }

    public void togglePlaying()
    {
        //if track is displayed
        if (ImageTargetTrackStart.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED && ImageTargetTrackEnd.GetComponent<TrackTargetTrackableEventHandler>().status == TrackableBehaviour.Status.TRACKED)
        {
            playing = !playing;
            playStartTimestamp = Time.time;

            if (playing)
            {
                Debug.Log("Stop Playing");
                TrackSliderInstance = Instantiate(TrackSlider);
                TrackSliderInstance.transform.position = ImageTargetTrackStart.transform.position;
                TrackSliderInstance.transform.rotation = ImageTargetTrackStart.transform.rotation;
            }
            else
            {
                Debug.Log("Start Playing");
                Destroy(TrackSliderInstance);
            }
        }
    }

    public void toggleLooping()
    {
        looping = !looping;
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

    private Vector3 ProjectTransformOnPlane(Transform objectToProject, Vector3 planeOrigin, Vector3 planeNormal) 
    {
        Plane projectionPlane = new Plane(planeNormal, planeOrigin);

        //return projectionPlane.ClosestPointOnPlane(objectToProject.position);

        float distanceToIntersection;
        Ray intersectionRay = new Ray(objectToProject.position, objectToProject.forward);
        if (projectionPlane.Raycast(intersectionRay, out distanceToIntersection))
        {
            return objectToProject.position + objectToProject.forward * distanceToIntersection;
        }

        Debug.Log("Cannot Project Note");
        throw new Exception("Cannot Project Note");
    }

    /*
    private Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
    {
        planeNormal.Normalize();
        var distance = -Vector3.Dot(planeNormal.normalized, (point - planeOrigin));
        return point + planeNormal * distance;
    }
    */
    /*
    public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {

        float distance;
        Vector3 translationVector;

        //First calculate the distance from the point to the plane:
        distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

        //Reverse the sign of the distance
        distance *= -1;

        //Get a translation vector
        translationVector = SetVectorLength(planeNormal, distance);

        //Translate the point to form a projection
        return point + translationVector;
    }



    //Get the shortest distance between a point and a plane. The output is signed so it holds information
    //as to which side of the plane normal the point is.
    public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {

        return Vector3.Dot(planeNormal, (point - planePoint));
    }



    //create a vector of direction "vector" with length "size"
    public static Vector3 SetVectorLength(Vector3 vector, float size)
    {

        //normalize the vector
        Vector3 vectorNormalized = Vector3.Normalize(vector);

        //scale the vector
        return vectorNormalized *= size;
    }
    */

    private Vector3 GetClosestPointOnSurface(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
    {
        Plane plane = new Plane(a, b, c);
        return plane.ClosestPointOnPlane(point);
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
