using Model;
using System;
using UnityEditor;

namespace M.ProductionPipeline
{
    public class SwitchPlatformStep : IStep
    {
        public BuildTargetGroup BuildTargetGroup;

        public void Run()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

            switch (settings.BuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    BuildTargetGroup = BuildTargetGroup.Standalone;

                    break;

                case BuildTarget.iOS:
                    BuildTargetGroup = BuildTargetGroup.iOS;

                    break;

                case BuildTarget.Android:
                    BuildTargetGroup = BuildTargetGroup.Android;

                    break;

                case BuildTarget.StandaloneWindows64:
                    BuildTargetGroup = BuildTargetGroup.Standalone;

                    break;

                case BuildTarget.WebGL:
                    BuildTargetGroup = BuildTargetGroup.WebGL;

                    break;

                case BuildTarget.WSAPlayer:
                    BuildTargetGroup = BuildTargetGroup.WSA;

                    break;

                case BuildTarget.StandaloneLinux64:
                    BuildTargetGroup = BuildTargetGroup.Standalone;

                    break;

                case BuildTarget.PS4:
                    BuildTargetGroup = BuildTargetGroup.PS4;

                    break;

                case BuildTarget.XboxOne:
                    BuildTargetGroup = BuildTargetGroup.XboxOne;

                    break;

                case BuildTarget.tvOS:
                    BuildTargetGroup = BuildTargetGroup.tvOS;

                    break;

                case BuildTarget.Switch:
                    BuildTargetGroup = BuildTargetGroup.Switch;

                    break;

                case BuildTarget.Lumin:
                    BuildTargetGroup = BuildTargetGroup.Lumin;

                    break;

                case BuildTarget.Stadia:
                    BuildTargetGroup = BuildTargetGroup.Stadia;

                    break;

                case BuildTarget.GameCoreXboxOne:
                    BuildTargetGroup = BuildTargetGroup.GameCoreXboxOne;

                    break;

                case BuildTarget.PS5:
                    BuildTargetGroup = BuildTargetGroup.PS5;

                    break;

                case BuildTarget.EmbeddedLinux:
                    BuildTargetGroup = BuildTargetGroup.EmbeddedLinux;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup, settings.BuildTarget);
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            UnityEditor.EditorUtility.RequestScriptReload();
        }

        public string EnterText()
        {
            return $"切换到 {nameof(BuildTargetGroup)} 开始！";
        }

        public string ExitText()
        {
            return $"切换到 {nameof(BuildTargetGroup)}结束！";
        }

        public bool IsTriggerCompile()
        {
            return true;
        }
    }
}