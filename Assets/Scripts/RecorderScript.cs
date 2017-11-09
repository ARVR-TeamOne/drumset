using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RecorderScript : MonoBehaviour, IVirtualButtonEventHandler
{

    private bool recording = false;
    private bool playing = false;

    List<GameObject> recordedLoop = new List<GameObject>();
    List<float> recordedTimes = new List<float>();
    List<float> playTimes = new List<float>();
    private float recordStartTime = 0;
    private int currLoopIndex = 0;


    VirtualButtonBehaviour[] virtualButtonBehaviours;


    // Use this for initialization
    void Start () {
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
            case "play":
                playLoop();
                break;

            case "recordLoop":
                recordLoop();
                break;

        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);

        SetButtonMaterial(1f, vb);
    }

    void playLoop()
    {
            if(!playing)
            {
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
                    Debug.Log("Playtime:" + playTime.ToString());
                    playTimes.Add(playTime);
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
        }
    }

    public void saveSound(GameObject sender)
    {
        if(recording)
        {
            if (recordStartTime == -1) recordStartTime = Time.time;

            Debug.Log("Saving Sound");
            Debug.Log(sender.name);
            recordedTimes.Add(Time.time - recordStartTime);
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
