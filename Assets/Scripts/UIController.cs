using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > 3)
        {
            DestroyStartMenu();
        }
	}

    public void DestroyStartMenu()
    {
        GameObject[] startMenuObjects = GameObject.FindGameObjectsWithTag("StartMenu");

        for (int i = 0; i < startMenuObjects.Length; i++)
        {
            Destroy(startMenuObjects[i]);
        }
    }
}
