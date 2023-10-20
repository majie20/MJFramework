using Model;
using UnityEditor;
using YooAsset;

namespace M.ProductionPipeline
{
    public class DevelopPlayModeStep : IStep
    {
        public void Run()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);
            settings.PlayMode = EPlayMode.EditorSimulateMode;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public string EnterText()
        {
            return $"切换为开发运行模式 开始！";
        }

        public string ExitText()
        {
            return $"切换为开发运行模式 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}