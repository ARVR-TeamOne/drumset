﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class RecorderScript : MonoBehaviour, IVirtualButtonEventHandler
{
    #region PUBLIC_MEMBERS
    private bool recording = false;
    private bool playing = false;
    #endregion

    #region PRIVATE_MEMBERS
    List<GameObject> recordedLoop = new List<GameObject>(); //gameobjects in order of recording
    List<float> recordedTimes = new List<float>();          //recorded sounds go here
    List<float> playTimes = new List<float>();              //timestamps for replay go here
    private float recordStartTime = -1;
    private int currLoopIndex = 0;
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    MetronomeScript metronome;
    #endregion

    // Use this for initialization
    void Start () {
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<MetronomeScript>();

        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update() {
        /*
        if(playing)
        {
            Debug.Log(playTimes[0]);
            Debug.Log(Time.time);
        }
        */
        if (playing && playTimes[0] <= Time.time)
        {
            Debug.Log("Playing Sound");
            recordedLoop[currLoopIndex].GetComponent<AudioSource>().Play();
            ++currLoopIndex;
            playTimes.RemoveAt(0);
        }
        
        if(playing && playTimes.Count <= 0)
        {
            Debug.Log("Loop over");
            playing = false;
            currLoopIndex = 0;
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonPressed: " + vb.VirtualButtonName);

        SetButtonMaterial(0.5f, vb);
        Debug.Log(vb.name);

        switch (vb.VirtualButtonName)
        {
            case "playLoop":
                playLoop();
                break;

            case "recordLoop":
                recordLoop();
                break;

        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);

        SetButtonMaterial(1f, vb);
    }

    void playLoop()
    {
            if(!playing && recordedTimes.Count > 0)
            {
                //stop metronome
                metronome.ticking = false;

                if(recording)
                {
                    Debug.Log("Stop Recording");
                    recording = false;
                }

                Debug.Log("Playing loop");

                //set playtimes
                playTimes.Clear();
                foreach (float time in recordedTimes)
                {
                    float playTime = time + Time.time;
                    playTimes.Add(playTime);

                    Debug.Log("Playtime:" + playTime.ToString());
                }

                playing = true;
            } else
            {
                Debug.Log("Stopping Loop");
                playing = false;
                currLoopIndex = 0;
                playTimes.Clear();
            }
    }

    //toggle recording, clean recorded loop if new record
    private void recordLoop()
    {
        if(!recording)
        {
            Debug.Log("Start Recording");
            recordedLoop.Clear();
            recordedTimes.Clear();
            playTimes.Clear();
            recording = true;
            playing = false;
            currLoopIndex = 0;
            recordStartTime = -1;
        } else
        {
            Debug.Log("Stop Recording");
            recording = false;
            //stop metronome
            metronome.ticking = false;
        }
    }

    public void saveSound(GameObject sender)
    {
        if(recording)
        {
            //first saved sound
            if (recordStartTime == -1)
            {
                recordStartTime = Time.time;
            }

            float soundPlayedAtTime = Time.time;

            if (metronome.ticking)
            {

                //align sound to metronome ticking
                float metronomeAlignedTimestamp = soundPlayedAtTime > (metronome.nextTickTimestamp - metronome.lastTickTimestamp) / 2 ? metronome.nextTickTimestamp - recordStartTime : metronome.lastTickTimestamp - recordStartTime;

                //check if sound is already played on this beat with same sender

                Debug.Log("sounds on same beat:");
                foreach (float wu in recordedTimes.ToArray().Select((b, i) => {
                    Debug.Log(b);
                    Debug.Log(i);
                    return b == metronomeAlignedTimestamp ? i : -1;
                    }).ToList())
                {
                    Debug.Log(wu);
                    if (wu == -1) continue;
                    Debug.Log(recordedLoop[(int) wu].name);
                }

                bool soundOnSameBeat = recordedTimes.ToArray().Select((b, i) => b == metronomeAlignedTimestamp ? i : -1).Where(i => {
                    if (i == -1) return false;
                    Debug.Log("Object on same Beat:");
                    Debug.Log(recordedLoop[i].name);
                    return recordedLoop[i].name == sender.name;
                    }).Count() > 0;

                if (soundOnSameBeat)
                {
                    Debug.Log("Sound will not be saved");
                    return;
                } else
                {
                    Debug.Log("Sound aligned to metronome");
                    soundPlayedAtTime = metronomeAlignedTimestamp;
                }
            }

            Debug.Log("Saving Sound");
            Debug.Log(sender.name);
            Debug.Log(Time.time);
            Debug.Log(soundPlayedAtTime);

            recordedTimes.Add(soundPlayedAtTime);
            recordedLoop.Add(sender);
        }
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
}
