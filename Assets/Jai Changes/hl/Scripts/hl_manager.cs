using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class hl_manager : MonoBehaviour
{
    public hl_data hl_data;

    GameObject[] objects;

    public Material noTex;

    public hl_channel activeTrigger;
    //Coroutine aod;
    // Start is called before the first frame update
    void Start()
    {
        LoadObjectData();
        //NOTES://
        //add entity script to door
        //add key/right click to convert to entity
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            StartCoroutine(ApplyObjectData());
        }
    }
    public enum hl_channel
    {
        none = 0,
        alpha = 1,
        beta = 2,
        charlie = 3,
        delta = 4,
        echo = 5,
        foxtrot = 6,
        golf = 7,
        hotel = 8,
        india = 9,
        juliet = 10
    }

    #region Object Data
    public void LoadObjectData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "hl_config.json");

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            hl_data = JsonUtility.FromJson<hl_data>(jsonString);

            //print(hl_data.root.objects[0].texture);
        }
        else
        {
            Debug.LogError("DATA FILE NONEXISTANT!");
        }
    }

    public IEnumerator ApplyObjectData()
    {
        objects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            //print(hl_data.data.names.Length);
            for (int i = 0; i < hl_data.data.names.Length; i++)
            {
                //print(hl_data.data.names[i]);
                if (obj.name == hl_data.data.names[i])
                {
                    //print(obj.name + " accepted");
                    obj.transform.localScale = new Vector3(hl_data.data.objects[i].scale.x, hl_data.data.objects[i].scale.y, hl_data.data.objects[i].scale.z);

                    //load texture, apply to mat
                    Material[] allmat = Resources.LoadAll<Material>("hl_materials");
                    Material mat = null;

                    for (int j = 0; j < allmat.Length; j++)
                    {
                        //print(allmat[j].name +" == " + hl_data.data.objects[i].texture +" ");
                        if (allmat[j].name == hl_data.data.objects[i].texture)
                        {
                            //print("setting mat as [" + (allmat[j].name + " == " + hl_data.data.objects[i].texture));
                            mat = allmat[j];
                            //print("mat is " + mat.name);
                        }

                        if (mat == null)
                        {
                            mat = noTex;
                        }

                        obj.GetComponent<MeshRenderer>().material = mat;
                    }
                }
            }
        }
        yield return null;
    }
    #endregion


    
}
