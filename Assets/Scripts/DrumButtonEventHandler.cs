﻿/*============================================================================== 
 Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.
 
 Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using Vuforia;

public class DrumButtonEventHandler : MonoBehaviour,
                                         IVirtualButtonEventHandler
{
    #region PUBLIC_MEMBERS
    public Material m_ButtonMaterial;
    public Material m_ButtonMaterialPressed;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    VirtualButtonBehaviour[] virtualButtonBehaviours;
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

        SetButtonMaterial(m_ButtonMaterialPressed);
        Debug.Log(vb.name);

        float pitch = GetComponent<AudioSource>().pitch;

        switch (vb.VirtualButtonName)
        {
            case "drum":
                GetComponent<AudioSource>().Play();
                break;

            case "pitchUp":
                pitch += 0.1f;
                GetComponentInChildren<TextMesh>().text = pitch.ToString();
                GetComponent<AudioSource>().pitch = pitch;
                break;

            case "pitchDown":
                pitch -= 0.1f;
                GetComponentInChildren<TextMesh>().text = pitch.ToString();
                GetComponent<AudioSource>().pitch = pitch;
                break;

        }
    }

    /// <summary>
    /// Called when the virtual button has just been released:
    /// </summary>
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);

        SetButtonMaterial(m_ButtonMaterial);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS
    void SetButtonMaterial(Material material)
    {
        // Set the Virtual Button material
        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            if (material != null)
            {
                virtualButtonBehaviours[i].GetComponent<MeshRenderer>().sharedMaterial = material;
            }
        }
    }
    #endregion // PRIVATE METHODS
}
