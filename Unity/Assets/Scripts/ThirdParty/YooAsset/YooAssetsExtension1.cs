namespace YooAsset
{
    public static partial class YooAssets
    {
        public static void UnloadUnusedAssets()
        {
            DebugCheckDefaultPackageValid();
            _defaultPackage.UnloadUnusedAssets();
        }

        public static void ForceUnloadAllAssets()
        {
            DebugCheckDefaultPackageValid();
            _defaultPackage.ForceUnloadAllAssets();
        }

        public static ClearUnusedCacheFilesOperation ClearPackageUnusedCacheFilesAsync()
        {
            DebugCheckDefaultPackageValid();

            return _defaultPackage.ClearUnusedCacheFilesAsync();
        }
    }
}