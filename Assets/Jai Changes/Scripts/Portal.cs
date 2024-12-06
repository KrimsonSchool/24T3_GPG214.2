using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string scene;
    public Vector3 pos;

    GameObject oth;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnLevelWasLoaded(int level)
    {
        oth.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        oth = other.gameObject;
        if (scene == "")
        {
            oth.transform.position = pos;
        }
        else
        {
            DontDestroyOnLoad(this);
            SceneManager.LoadScene(scene);
        }
    }
}
