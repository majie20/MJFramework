using Model;
using UnityEditor;
using YooAsset;

namespace M.ProductionPipeline
{
    public class BuildPlayModeStep : IStep
    {
        public void Run()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                settings.PlayMode = EPlayMode.WebPlayMode;
            }
            else
            {
                if (settings.IsHotfix)
                {
                    settings.PlayMode = EPlayMode.HostPlayMode;
                }
                else
                {
                    settings.PlayMode = EPlayMode.OfflinePlayMode;
                }
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public string EnterText()
        {
            return $"切换为构建运行模式 开始！";
        }

        public string ExitText()
        {
            return $"切换为构建运行模式 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}