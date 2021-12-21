using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "AssetsAtlasSettings", menuName = "ScriptableObjects/AssetsAtlasSettings", order = 5)]
public class AssetsAtlasSettings : ScriptableObject
{
    public List<Object> atlasList = new List<Object>();
    public List<string> atlasNameList = new List<string>();

    public List<string> imageNameList = new List<string>();
    public List<Object> atlasCommonList = new List<Object>();

    [HideInInspector]
    public Object atlasFile;
}