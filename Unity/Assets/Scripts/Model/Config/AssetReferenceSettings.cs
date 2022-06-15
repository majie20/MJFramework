using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum LoadType
{
    Normal,
    Sub,
    RawFile
}

[CreateAssetMenu(fileName = "AssetReferenceSettings", menuName = "ScriptableObjects/AssetReferenceSettings", order = 2)]
public class AssetReferenceSettings : SerializedScriptableObject
{
    [Serializable]
    public class Info
    {
        public string typeName;
        public string path;
        public LoadType type;
    }

    [ReadOnly]
    [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
    public List<Info> AssetPathList;
}