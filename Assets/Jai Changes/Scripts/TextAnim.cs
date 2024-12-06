using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnim : MonoBehaviour
{
    public float speed;
    public TMP_Text text;

    float timer;

    public string[] stages;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        if(text == null)
        {
            text = GetComponent<TMP_Text>();
            if(text == null)
            {
                text = gameObject.AddComponent<TMP_Text>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;

        if (timer>speed)
        {
            timer = 0;
            if (index < stages.Length-1)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            text.text = stages[index];
        }
    }
}
