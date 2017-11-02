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

    public GameObject FrontPageObject;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    bool optionsShownFlag = false;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {

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

        SetButtonMaterial(0.5f, vb);
        Debug.Log(vb.name);

        float pitch = 0;
        float volume = 0;

        AudioSource audio = GetComponent<AudioSource>();

        //get audio source settings from front page object
        if (FrontPageObject != null)
        {
            audio = FrontPageObject.GetComponent<AudioSource>();
            pitch = FrontPageObject.GetComponent<AudioSource>().pitch;
            volume = FrontPageObject.GetComponent<AudioSource>().volume;
        }


        switch (vb.VirtualButtonName)
        {
            case "drum1":
            case "drum2":
                audio.Play();
                Animation anim = gameObject.GetComponentInChildren<Animation>();
                anim.Play("drumAnim");
                break;

            case "pitchUp":
                pitch += 0.1f;
                GetChildObject(gameObject.transform, "PitchInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(pitch * 10) / 10).ToString();
                audio.pitch = pitch;
                break;

            case "pitchDown":
                pitch -= 0.1f;
                GetChildObject(gameObject.transform, "PitchInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(pitch * 10) / 10).ToString();
                audio.pitch = pitch;
                break;
                
            case "volumeUp":
                volume += 0.1f;
                GetChildObject(gameObject.transform, "VolumeInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(volume * 10) / 10).ToString();
                audio.volume = volume;
                break;

            case "volumeDown":
                volume -= 0.1f;
                GetChildObject(gameObject.transform, "VolumeInfoText").GetComponent<TextMesh>().text = (Mathf.Ceil(volume * 10) / 10).ToString();
                audio.volume = volume;
                break;

            case "record":
                audio.clip = Microphone.Start("Built-in Microphone", false, 2, 44100);
                break;

            case "option":
                //show buttons
                if (!optionsShownFlag)
                {
                    /*
                    GameObject pitchDown = Instantiate(VirtualButtonPitchDown, vb.transform.position + new Vector3(0f, 0f, -0.025f), vb.transform.rotation, gameObject.transform);
                    GameObject pitchUp = Instantiate(VirtualButtonPitchUp, vb.transform.position + new Vector3(0.045f, 0f, -0.025f), vb.transform.rotation, gameObject.transform);
                    GameObject record = Instantiate(VirtualButtonRecord, vb.transform.position + new Vector3(0.045f, 0f, 0f), vb.transform.rotation, gameObject.transform);
                    */

                    StateManager stateManager = TrackerManager.Instance.GetStateManager();

                    foreach (TrackableBehaviour tb in stateManager.GetTrackableBehaviours())
                    {

                        //stateManager.
                    }

                        ImageTargetBehaviour.CreateVirtualButton("pitchDown", new Vector2(0.02f, 0.015f), gameObject);
                    /*
                    foreach (GameObject button in GameObject.FindGameObjectsWithTag("OptionButtonDrum"))
                    {
                        button.GetComponent<VirtualButtonBehaviour>().enabled = true;
                        button.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
                        button.GetComponent<VirtualButtonBehaviour>().UpdateAreaRectangle();
                    }
                    */

                    optionsShownFlag = true;
                } else //hide buttons
                {
                    foreach(GameObject button in GameObject.FindGameObjectsWithTag("OptionButtonDrum"))
                    {
                        Destroy(button);
                    }
                    optionsShownFlag = false;
                }
                break;

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
