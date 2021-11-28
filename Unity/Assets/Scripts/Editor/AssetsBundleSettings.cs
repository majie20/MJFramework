using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileDirABName
{
    public string ABName;
    public string Extension;
    public Object Dir;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AssetsBundleSettings", order = 1)]
public class AssetsBundleSettings : ScriptableObject
{
    public List<FileDirABName> fileDirABList = new List<FileDirABName>();
}