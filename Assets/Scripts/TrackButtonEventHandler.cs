using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackButtonEventHandler : MonoBehaviour, IVirtualButtonEventHandler {
    
    private VirtualButtonBehaviour[] virtualButtonBehaviours;

    // Use this for initialization
    void Start () {
        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            Debug.Log(virtualButtonBehaviours[i].name);
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }
    }
	
	void FixedUpdate () {
        
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

        switch (vb.VirtualButtonName) {
            case "play":
                gameObject.GetComponent<TrackScript>().togglePlaying();
                break;

            case "loop":
                gameObject.GetComponent<TrackScript>().toggleLooping();
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
