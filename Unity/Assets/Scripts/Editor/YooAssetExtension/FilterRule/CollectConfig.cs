using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public class CollectConfig : IFilterRule
{
    public bool IsCollectAsset(FilterRuleData data)
    {
        if (CollectAll.IsIgnoreAsset(data))
        {
            return false;
        }
        return AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath) == typeof(TextAsset) || Path.GetExtension(data.AssetPath) == ".asset";
    }

    public static bool IsIgnoreAsset(FilterRuleData data)
    {
        return Path.GetFileNameWithoutExtension(data.AssetPath).StartsWith("~");
    }
}