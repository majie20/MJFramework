using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AssetCollector", menuName = "ScriptableObjects/AssetCollector", order = 2)]
public class AssetCollector : SerializedScriptableObject
{
    [Serializable]
    public class CollectorInfo
    {
        public Object Obj;
        public LoadType Type;
    }

    [OnValueChanged("OnValueChanged")]
    public UnityEngine.Object target;

    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<CollectorInfo> list;

    private void OnValueChanged()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}