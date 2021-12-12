using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetsCiteMatchConfigSettings", menuName = "ScriptableObjects/AssetsCiteMatchConfigSettings", order = 3)]
public class AssetsCiteMatchConfigSettings : ScriptableObject
{
    public List<string> PathList = new List<string>();
    public List<Object> AssetsList = new List<Object>();
}