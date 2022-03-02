using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetsCiteMatchConfigSettings", menuName = "ScriptableObjects/AssetsCiteMatchConfigSettings", order = 3)]
public class AssetsCiteMatchConfigSettings : ScriptableObject
{
    [HorizontalGroup("1")]
    [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
    public List<string> PathList = new List<string>();

    [HorizontalGroup("1")]
    [ReadOnly]
    [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
    public List<Object> AssetsList = new List<Object>();
}