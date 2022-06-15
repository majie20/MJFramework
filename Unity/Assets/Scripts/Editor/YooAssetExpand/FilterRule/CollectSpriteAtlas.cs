using UnityEditor;
using UnityEngine.U2D;
using YooAsset.Editor;

public class CollectSpriteAtlas : IFilterRule
{
    public bool IsCollectAsset(FilterRuleData data)
    {
        return AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath) == typeof(SpriteAtlas);
    }
}