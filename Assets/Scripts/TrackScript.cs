using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackScript : MonoBehaviour {
    
    private GameObject ImageTargetTrackStart;
    private GameObject ImageTargetTrackEnd;
    private LineRenderer TrackLine;


    // Use this for initialization
    void Start () {
        ImageTargetTrackStart = GameObject.Find("ImageTargetTrackStart");
        ImageTargetTrackEnd = GameObject.Find("ImageTargetTrackEnd");

        //create track line object
        TrackLine = this.gameObject.AddComponent<LineRenderer>();
        // Set the width of the Line Renderer
        TrackLine.SetWidth(0.005F, 0.005F);
        // Set the number of vertex fo the Line Renderer
        TrackLine.SetVertexCount(2);
    }
	
	// Update is called once per frame
	void Update () {

        if(ImageTargetTrackStart.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED && ImageTargetTrackEnd.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
        {
            //draw trackline
            TrackLine.SetVertexCount(2);
            TrackLine.SetPosition(0, ImageTargetTrackStart.transform.position);
            TrackLine.SetPosition(1, ImageTargetTrackEnd.transform.position);

            /*
             * calculate note positions
             */
            //for each vumark
            foreach(GameObject Note in GameObject.FindGameObjectsWithTag("Note"))
            {
                if (Note.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
                {
                    //calculate orthogonal angle to trackline

                    //get rotation on 2d surface
                }
            }

            /*
             * calculate sounds
             */
        } else
        {
            //hide trackline
            TrackLine.SetVertexCount(0);
        }
    }
}
