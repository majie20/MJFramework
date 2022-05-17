using YooAsset.Editor;

public class PackShader : IPackRule
{
    string IPackRule.GetBundleName(PackRuleData data)
    {
        return "myshaders";
    }
}