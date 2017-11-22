/*============================================================================== 
 Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.
 
 Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using Vuforia;
using System.Collections.Generic;

public class DrumButtonEventHandler : MonoBehaviour,
                                         IVirtualButtonEventHandler
{
    #region PUBLIC_MEMBERS
    public GameObject MenuSoundObject;
    public GameObject FrontPageObject;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    AudioSource menuSound;
    RecorderScript recorder;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        recorder = GameObject.FindGameObjectWithTag("Recorder").GetComponent<RecorderScript>();

        if(MenuSoundObject != null)
        {
            menuSound = MenuSoundObject.GetComponent<AudioSource>();
        }

        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }
    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    /// <summary>
    /// Called when the virtual button has just been pressed:
    /// </summary>
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

        float pitch = 0;
        float volume = 0;

        AudioSource audio = GetComponent<AudioSource>();

        //get audio source settings from front page object
        if (FrontPageObject != null)
        {
            audio = FrontPageObject.GetComponent<AudioSource>();
            pitch = FrontPageObject.GetComponent<AudioSource>().pitch;
            volume = FrontPageObject.GetComponent<AudioSource>().volume;

            Debug.Log(vb.VirtualButtonName.Remove(vb.VirtualButtonName.Length - 2, 2));

            switch (vb.VirtualButtonName.Remove(vb.VirtualButtonName.Length - 1, 1))
            {
                case "pitchUp":
                    Debug.Log("WULULU");
                    pitch += 0.1f;
                    GetChildObject(gameObject.transform, "PitchInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(pitch * 10) / 10).ToString();
                    audio.pitch = pitch;
                    menuSound.Play();
                    break;

                case "pitchDown":
                    pitch -= 0.1f;
                    GetChildObject(gameObject.transform, "PitchInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(pitch * 10) / 10).ToString();
                    audio.pitch = pitch;
                    menuSound.Play();
                    break;

                case "volumeUp":
                    volume += 0.1f;
                    GetChildObject(gameObject.transform, "VolumeInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(volume * 10) / 10).ToString();
                    audio.volume = volume;
                    menuSound.Play();
                    break;

                case "volumeDown":
                    volume -= 0.1f;
                    GetChildObject(gameObject.transform, "VolumeInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(volume * 10) / 10).ToString();
                    audio.volume = volume;
                    menuSound.Play();
                    break;

                case "record":
                    audio.clip = Microphone.Start("Built-in Microphone", false, 2, 44100);
                    break;

            }
        } else
        {
            //since there was no front page object, we can assume it is front page button and therefore a drum
            audio.Play();
            Animation anim = gameObject.GetComponentInChildren<Animation>();
            anim.Play("drumAnim");
            recorder.saveSound(gameObject);
        }
    }

    /// <summary>
    /// Called when the virtual button has just been released:
    /// </summary>
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);

        SetButtonMaterial(1f, vb);
    }

    #endregion //PUBLIC_METHODS

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

    public GameObject GetChildObject(Transform parent, string _tag)
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
    #endregion // PRIVATE METHODS
}
