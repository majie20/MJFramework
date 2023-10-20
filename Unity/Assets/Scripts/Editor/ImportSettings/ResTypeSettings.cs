using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ResTypeSettings", menuName = "ScriptableObjects/ResTypeSettings", order = 2)]
public class ResTypeSettings : ScriptableObject
{
    [ShowInInspector]
    [InfoBox("UI图片相关的资源设置配置")]
    public ConfigInfo UITextureResConfig = new ConfigInfo();

    [ShowInInspector]
    [InfoBox("UI图集相关的资源设置配置")]
    public ConfigInfo UIAtlasResConfig = new ConfigInfo();
}