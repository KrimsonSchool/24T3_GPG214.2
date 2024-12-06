using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void CallLoadScene(string scene)
    {
        if(sceneName != null)
        {
            scene = sceneName;
        }
        StartCoroutine(LoadScene(scene));
    }
}
