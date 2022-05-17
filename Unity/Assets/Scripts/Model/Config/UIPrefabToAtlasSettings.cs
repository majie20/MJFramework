using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPrefabToAtlasSettings", menuName = "ScriptableObjects/UIPrefabToAtlasSettings", order = 2)]
public class UIPrefabToAtlasSettings : SerializedScriptableObject
{
    [ReadOnly]
    public Dictionary<string, string> InfoDic = new Dictionary<string, string>();
}