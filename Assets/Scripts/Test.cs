using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject prefab;
    public GameObject[] arrayPrefab;

    // Use this for initialization
    void Start () {
        arrayPrefab = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(3*i, 5+3*i, i*1);
            Quaternion rotation = new Quaternion(1, 2, 1, 1);
            GameObject obj = Instantiate(prefab, position, rotation) as GameObject;
            arrayPrefab[i] = obj;
        }
        this.changedMethode(arrayPrefab);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void changedMethode(GameObject[] array)
    {   //
        array[0].transform.position = new Vector3(0,0,0);
        var startPos = array[0].transform.position;

        foreach (GameObject test in array)
        {
            test.transform.rotation = Quaternion.FromToRotation(Vector3.left, new Vector3(0, 0, 0));
            print(test.transform.rotation);
            print(startPos - test.transform.position);
            test.transform.Translate(0, startPos.y - test.transform.position.y,0);
        }
    }
}
