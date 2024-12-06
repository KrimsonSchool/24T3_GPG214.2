using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FileData
{
    public string fileName;
    public string fileDestiniation;
    public long fileSize;
    public DateTime dateLastModified;
    public DateTime dateCreated;
    public string dateModifiedString;
    public string dateCreatedString;
}
