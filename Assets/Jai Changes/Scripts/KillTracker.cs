using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTracker : MonoBehaviour
{
    Damageable damageable;
    bool hasDied;
    // Start is called before the first frame update
    void Start()
    {
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damageable.currentHitPoints <= 0 && !hasDied)
        {
            print("Got Kill!");
            PlayerPrefs.SetInt("Achievment0", PlayerPrefs.GetInt("Achievment0") + 1);
            FindObjectOfType<Database>().currentData.achievmentLevels[0]++;
            hasDied = true;
        }
    }
}
