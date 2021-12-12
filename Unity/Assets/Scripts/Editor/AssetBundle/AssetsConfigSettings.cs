using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetsCiteMatchConfig
{
    public string Extension;
    public List<Object> DirList = new List<Object>();
}

[CreateAssetMenu(fileName = "AssetsConfigSettings", menuName = "ScriptableObjects/AssetsConfigSettings", order = 3)]
public class AssetsConfigSettings : ScriptableObject
{
    public List<AssetsCiteMatchConfig> FileDirABList = new List<AssetsCiteMatchConfig>();
    public AssetsCiteMatchConfigSettings Settings;
}