using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class hl_entity : MonoBehaviour
{
    public hl_type type;

    public hl_manager.hl_channel channel = hl_manager.hl_channel.none;
    public hl_manager.hl_channel exitChannel = hl_manager.hl_channel.none;

    public float speed;
    float cycle;

    bool triggered;
    bool secondaryTriggered;

    Vector3 pos;
    Vector3 end;
    // Start is called before the first frame update
    void Start()
    {
         pos = transform.position;
         end = transform.position + transform.up * 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<hl_manager>().activeTrigger == channel && FindObjectOfType<hl_manager>().activeTrigger != hl_manager.hl_channel.none)
        {
            Triggered();
        }
        if (FindObjectOfType<hl_manager>().activeTrigger == exitChannel && FindObjectOfType<hl_manager>().activeTrigger != hl_manager.hl_channel.none)
        {
            SecondaryTriggered();
        }

        if (triggered)
        {
            cycle += Time.deltaTime * speed;

            if (type == hl_type.door)
            {
                if (transform.position != end)
                {
                    transform.position = Vector3.Lerp(pos, end, cycle);
                }
                else
                {
                    triggered = false;
                    cycle = 0;
                }
            }
        }

        if (secondaryTriggered)
        {
            cycle += Time.deltaTime * speed;

            if (type == hl_type.door)
            {
                if (transform.position != pos)
                {
                    transform.position = Vector3.Lerp(end, pos, cycle);
                }
                else
                {
                    secondaryTriggered = false;
                    cycle = 0;
                }
            }
        }
    }

    public enum hl_type
    {
        door,
        spawn
    }

    private void OnCollisionEnter(Collision collision)
    {
        Triggered();
    }

    public void Triggered()
    {
        secondaryTriggered = false;
        cycle = 0;

        //print("Triggered!");
        FindObjectOfType<hl_manager>().activeTrigger = hl_manager.hl_channel.none;
        triggered = true;
    }

    public void SecondaryTriggered()
    {
        triggered = false;
        cycle = 0;

        //print("Secondary Triggered!");
        FindObjectOfType<hl_manager>().activeTrigger = hl_manager.hl_channel.none;
        secondaryTriggered = true;
    }
}
