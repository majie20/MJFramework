using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetReferenceSettings", menuName = "ScriptableObjects/AssetReferenceSettings", order = 2)]
public class AssetReferenceSettings : SerializedScriptableObject
{
    [Serializable]
    public class Info
    {
        public string typeName;
        public string path;
    }

    [ReadOnly]
    [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
    public List<Info> AssetPathList;
}