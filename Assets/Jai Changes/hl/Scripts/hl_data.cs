using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class hl_data
{
    public Data data;
}
[System.Serializable]
public class Data
{
    public string[] names;
    public Objects[] objects;
}
[System.Serializable]
public class Objects
{
    public string texture;
    public Scale scale;
}
[System.Serializable]
public class Scale
{
    public float x;
    public float y;
    public float z;
}
