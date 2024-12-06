using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class hl_triggerLibrary : MonoBehaviour
{
    [SerializeField]
    GameObject[] triggers;
    // Start is called before the first frame update
    void Start()
    {
        //
    }
    void Update()
    {
        triggers = new GameObject[0];
        GameObject[] objects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objects)
        {
            if (obj.name == "hl_trigger")
            {
                //print("found a trigger, adding...");
                triggers = AddToArray(obj, triggers);
            }
        }
    }

    public GameObject[] AddToArray(GameObject from, GameObject[] to)
    {
        GameObject[] temp = new GameObject[to.Length + 1];
        //print("temp is [" + temp.Length + "] big.");
        for (int i = 0; i < to.Length; i++)
        {
            temp[i] = to[i];
        }
        //print("add at slot [" + to.Length + "]");
        temp[to.Length] = from;
        to = temp;

        return to;
    }
}
