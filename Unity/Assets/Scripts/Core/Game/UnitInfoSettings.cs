using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public enum UnitType : byte
    {
        None = 0,
        Player,
        Monster,
        Boss,
    }

    [Serializable]
    public class UnitInfo
    {
        public string   AssetReferenceSettingsPath;
        public UnitType Type;
    }

    [CreateAssetMenu(fileName = "UnitInfoSettings", menuName = "ScriptableObjects/UnitInfoSettings", order = 3)]
    public class UnitInfoSettings : SerializedScriptableObject
    {
        [ReadOnly]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<string, UnitInfo> UnitInfoMap;
    }
}