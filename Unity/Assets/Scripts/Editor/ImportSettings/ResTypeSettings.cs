using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "ResTypeSettings", menuName = "ScriptableObjects/ResTypeSettings", order = 2)]
public class ResTypeSettings : ScriptableObject
{
    [Serializable]
    public class DirectoryObj
    {
        public Object value;
    }

    [Serializable]
    public class ConfigInfo
    {
        [OnValueChanged("OnValueChanged")]
        public Object template;

        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public List<DirectoryObj> list;

        private void OnValueChanged()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [ShowInInspector]
    [InfoBox("UI图片相关的资源设置配置")]
    public ConfigInfo UITextureResConfig = new ConfigInfo();

    [ShowInInspector]
    [InfoBox("UI图集相关的资源设置配置")]
    public ConfigInfo UIAtlasResConfig = new ConfigInfo();
}