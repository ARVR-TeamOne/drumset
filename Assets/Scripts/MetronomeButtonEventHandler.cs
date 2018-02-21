using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MetronomeButtonEventHandler : MonoBehaviour, IVirtualButtonEventHandler
{

    private VirtualButtonBehaviour[] virtualButtonBehaviours;
    private TrackScript trackScript;

    // Use this for initialization
    void Start()
    {
        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            Debug.Log(virtualButtonBehaviours[i].name);
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }

        //get trackscript 
        trackScript = GameObject.FindGameObjectWithTag("ImageTargetTrackStart").GetComponent<TrackScript>();
    }

    void FixedUpdate()
    {

    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonPressed: " + vb.VirtualButtonName);
        /*
        Debug.Log(this);
        Debug.Log(gameObject);
        Debug.Log(vb.gameObject);
        Debug.Log(vb.name);
        */

        SetButtonMaterial(0.5f, vb);

        switch (vb.VirtualButtonName)
        {
            case "bpmUp":
                trackScript.BPM = Math.Min(200, trackScript.BPM + 10);
                break;

            case "bpmDown":
                trackScript.BPM = Math.Max(0, trackScript.BPM - 10);
                break;

            case "metronome":
                trackScript.toggleMetronome();
                break;
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        SetButtonMaterial(1f, vb);
    }

    private void SetButtonMaterial(float transparency, VirtualButtonBehaviour vb)
    {
        // Set the Virtual Button material
        if (transparency != null)
        {
            Color color = vb.GetComponent<MeshRenderer>().material.color;
            vb.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, transparency);
        }
    }
}
