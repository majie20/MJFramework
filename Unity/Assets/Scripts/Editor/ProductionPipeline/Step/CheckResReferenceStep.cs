using Model;
using UnityEditor;
using YooAsset.Editor;

namespace M.ProductionPipeline
{
    public class CheckResReferenceStep : IStep
    {
        public void Run()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

            BuildParameters buildParameters = new BuildParameters();
            buildParameters.StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            buildParameters.BuildMode = EBuildMode.SimulateBuild;
            buildParameters.PackageName = settings.PackageName;
            buildParameters.PackageVersion = "Simulate";
            buildParameters.EnableLog = false;

            //var buildContext = new BuildContext();

            //var buildParametersContext = new BuildParametersContext(buildParameters);
            //buildContext.SetContextObject(buildParametersContext);

            var taskGetBuildMap = new TaskGetBuildMap();
            var buildMapContext = taskGetBuildMap.CreateBuildMap(buildParameters);
            EditorHelper.CheckBuildMapContent(buildMapContext);
        }

        public string EnterText()
        {
            return $"检查资源引用情况 开始！";
        }

        public string ExitText()
        {
            return $"检查资源引用情况 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}