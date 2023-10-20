using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAtlasSettings", menuName = "ScriptableObjects/SpriteAtlasSettings", order = 5)]
public class SpriteAtlasSettings : SerializedScriptableObject
{
    public DefaultAsset Folder;

    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<Object> CollectorList;
}