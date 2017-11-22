using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MetronomeScript : MonoBehaviour, IVirtualButtonEventHandler
{
    #region PUBLIC_MEMBERS
    [HideInInspector] public float lastTickTimestamp = 0;
    [HideInInspector] public float nextTickTimestamp = 0;
    [HideInInspector]
    public bool ticking = false;
    #endregion

    #region PRIVATE_MEMBERS
    AudioSource tickAudio;
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    private float BPM = 60;
    private float tickStartTimestamp = 0;
    #endregion

    // Use this for initialization
    void Start () {

        tickAudio = GetComponent<AudioSource>();

        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(ticking && Time.time >= nextTickTimestamp)
        {
            tickAudio.Play();
            lastTickTimestamp = nextTickTimestamp;
            nextTickTimestamp = nextTickTimestamp + 60f / BPM;
        }
	}

    public void toggleMetronome()
    {
        ticking = !ticking;

        if(ticking)
        {
            startMetronome();
        } else
        {
            stopMetronome();
        }
    }

    void startMetronome()
    {
        tickStartTimestamp = Time.time;
        lastTickTimestamp = Time.time;
        nextTickTimestamp = 60f / BPM + tickStartTimestamp;
    }

    void stopMetronome()
    {
        
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        SetButtonMaterial(0.5f, vb);

        switch (vb.VirtualButtonName)
        {
            case "metronome":
                toggleMetronome();
                break;

            case "BPMUp":
                BPM += 10f;
                break;

            case "BPMDown":
                BPM -= 10f;
                break;

        }
        Debug.Log(BPM);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        SetButtonMaterial(1f, vb);
    }

    #region PRIVATE_METHODS
    void SetButtonMaterial(float transparency, VirtualButtonBehaviour vb)
    {
        // Set the Virtual Button material
        if (transparency != null)
        {
            Color color = vb.GetComponent<MeshRenderer>().material.color;
            vb.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, transparency);
        }
    }
    #endregion
}
