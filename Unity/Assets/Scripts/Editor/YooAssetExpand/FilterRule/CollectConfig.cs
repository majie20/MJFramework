using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public class CollectConfig : IFilterRule
{
    public bool IsCollectAsset(FilterRuleData data)
    {
        return AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath) == typeof(TextAsset) || Path.GetExtension(data.AssetPath) == ".asset";
    }
}