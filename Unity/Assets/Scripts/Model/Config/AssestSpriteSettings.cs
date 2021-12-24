using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "AssestSpriteSettings", menuName = "ScriptableObjects/AssestSpriteSettings", order = 5)]
public class AssestSpriteSettings : ScriptableObject
{
    public List<string> atlasPathList = new List<string>();
    public List<string> atlasNameList = new List<string>();

    public List<string> imageNameList = new List<string>();
    public List<string> atlasCommonPathList = new List<string>();

    [HideInInspector]
    public Object atlasFile;
}