using UnityEngine;
using YooAsset;

namespace Model
{
    // 内置文件查询服务类
    public class QueryStreamingAssetsFileServices : IBuildinQueryServices
    {
        public bool QueryStreamingAssets(string packageName, string fileName)
        {
            //NLog.Log.Debug(FileHelper.RelativePath(Application.streamingAssetsPath, $"{buildinFolderName}/{packageName}/{fileName}"));
            //NLog.Log.Debug(Application.streamingAssetsPath);

#if UNITY_WEBGL
            return false;
#else
            // 注意：使用了BetterStreamingAssets插件，使用前需要初始化该插件！
            string buildinFolderName = YooAssets.GetPackage(packageName).GetPackageBuildinRootDirectory();
            return BetterStreamingAssets.FileExists(FileHelper.RelativePath(Application.streamingAssetsPath, $"{buildinFolderName}/{packageName}/{fileName}"));
#endif
        }
    }
}