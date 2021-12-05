using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileDirABName
{
    public string ABName;
    public string Extension;
    public List<Object> DirList = new List<Object>();
}

[CreateAssetMenu(fileName = "AssetsBundleConfigSettings", menuName = "ScriptableObjects/AssetsBundleConfigSettings", order = 2)]
public class AssetsBundleConfigSettings : ScriptableObject
{
    public List<FileDirABName> FileDirABList = new List<FileDirABName>();
}