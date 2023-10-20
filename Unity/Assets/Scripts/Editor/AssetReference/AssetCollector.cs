using Model;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AssetCollector", menuName = "ScriptableObjects/AssetCollector", order = 4)]
public class AssetCollector : SerializedScriptableObject
{
    [Serializable]
    public class CollectorInfo1
    {
        public Object   Obj;
        public LoadType Type;
    }

    //[Serializable]
    //public class CollectorInfo2
    //{
    //    public string   Path;
    //    public LoadType Type;
    //}

    [OnValueChanged("OnValueChanged")]
    public UnityEngine.Object target;

    [LabelText("直接加载资源列表")]
    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<CollectorInfo1> list1;

    [LabelText("后台加载资源列表")]
    [ListDrawerSettings(ShowPaging = false, Expanded = true)]
    public List<CollectorInfo1> list2;

    private void OnValueChanged()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}