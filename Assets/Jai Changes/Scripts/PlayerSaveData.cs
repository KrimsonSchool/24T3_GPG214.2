using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerSaveData
{
    public bool hasData;
    public string playerName;
    public string scene;
    public int kills;
    public int health;
    public Vector3 playerPosition;
    public string weaponSkin;
    public List<string> ownedDlc;

    public long time;

    public int[] achievmentLevels;
    public string[] achievments;
}
