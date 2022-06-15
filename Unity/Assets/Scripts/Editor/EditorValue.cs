using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

[Serializable]
public class ConfigInfo
{
    [OnValueChanged("OnValueChanged")]
    public UnityEngine.Object target;

    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<Object> list;

    private void OnValueChanged()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}