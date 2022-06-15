using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAtlasSettings", menuName = "ScriptableObjects/SpriteAtlasSettings", order = 2)]
public class SpriteAtlasSettings : SerializedScriptableObject
{
    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<Object> CollectorList;
}