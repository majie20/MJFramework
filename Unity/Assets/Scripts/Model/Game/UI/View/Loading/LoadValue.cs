namespace Model
{
    public enum LoadProgressType
    {
        None,
        UpdateStaticVersion,
        UpdatePatchManifest,
        DownloadHotAssets,
        LoadAssets,
    }

    public enum LoadUseType
    {
        Hot,
        Normal,
    }
}