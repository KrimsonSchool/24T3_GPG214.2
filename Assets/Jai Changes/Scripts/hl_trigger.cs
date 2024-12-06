using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hl_trigger : MonoBehaviour
{
    public hl_manager.hl_channel channel = hl_manager.hl_channel.none;
    public hl_manager.hl_channel exitChannel = hl_manager.hl_channel.none;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<hl_manager>().activeTrigger = channel;
    }

    private void OnTriggerExit(Collider other)
    {
        FindObjectOfType<hl_manager>().activeTrigger = exitChannel;
    }
}
