using System.IO;
using UnityEditor;
using UnityEngine.U2D;
using YooAsset.Editor;

public class CollectSpriteAtlas : IFilterRule
{
    public bool IsCollectAsset(FilterRuleData data)
    {
        if (CollectAll.IsIgnoreAsset(data))
        {
            return false;
        }

        return AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath) == typeof(SpriteAtlas);
    }
}