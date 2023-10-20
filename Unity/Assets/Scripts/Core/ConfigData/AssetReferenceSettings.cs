using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Model
{
    public enum LoadType
    {
        Normal,
        Sub,
        RawFile
    }

    [CreateAssetMenu(fileName = "AssetReferenceSettings", menuName = "ScriptableObjects/AssetReferenceSettings", order = 2)]
    public class AssetReferenceSettings : ScriptableObject
    {
        [Serializable]
        public struct Info
        {
            public  string   TypeName;
            [SerializeField]
            private Object   Obj;
            public  string   Path;
            public  LoadType Type;

            public Info(string typeName, Object obj, string path, LoadType type)
            {
                TypeName = typeName;
                Obj = obj;
                Path = path;
                Type = type;
            }
        }

        [ReadOnly]
        [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
        public List<Info> InstantlyAssetDataList;

        [ReadOnly]
        [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
        public List<Info> RearAssetDataList;
    }
}