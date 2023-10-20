using YooAsset.Editor;

public class PackShader : IPackRule
{
    public PackRuleResult GetPackRuleResult(PackRuleData data)
    {
        PackRuleResult result = new PackRuleResult("myshaders", DefaultPackRule.AssetBundleFileExtension);

        return result;
    }

    public bool IsRawFilePackRule()
    {
        return false;
    }
}