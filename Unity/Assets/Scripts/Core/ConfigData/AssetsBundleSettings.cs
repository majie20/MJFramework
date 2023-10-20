using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YooAsset;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Model
{
    public enum EncryptType
    {
        None,
        Empty,
        Offset,
    }

    public enum HotfixMode
    {
        None,
        ILRunTime,
        HybridCLR,
    }

    public enum GameType
    {
        NormalGame,
        WXGame,
        TTGame,
        QQGame,
        GoogleGame,
        FacebookGame,
    }

    ///// <summary>
    ///// 首包资源文件的拷贝方式
    ///// </summary>
    //public enum ECopyBuildinFileOption
    //{
    //    /// <summary>
    //    /// 不拷贝任何文件
    //    /// </summary>
    //    None = 0,

    //    /// <summary>
    //    /// 先清空已有文件，然后拷贝所有文件
    //    /// </summary>
    //    ClearAndCopyAll,

    //    /// <summary>
    //    /// 先清空已有文件，然后按照资源标签拷贝文件
    //    /// </summary>
    //    ClearAndCopyByTags,

    //    /// <summary>
    //    /// 不清空已有文件，直接拷贝所有文件
    //    /// </summary>
    //    OnlyCopyAll,

    //    /// <summary>
    //    /// 不清空已有文件，直接按照资源标签拷贝文件
    //    /// </summary>
    //    OnlyCopyByTags,		

    //    /// <summary>
    //    /// 清空已有文件
    //    /// </summary>
    //    ClearAll,
    //}

    //if (buildParametersContext.Parameters.CopyBuildinFileOption != ECopyBuildinFileOption.None && buildParametersContext.Parameters.CopyBuildinFileOption != ECopyBuildinFileOption.ClearAll)
    //{
    //    CopyBuildinFilesToStreaming(buildParametersContext, manifestContext);
    //}
    //else if (buildParametersContext.Parameters.CopyBuildinFileOption == ECopyBuildinFileOption.ClearAll)
    //{
    //    string streamingAssetsDirectory = buildParametersContext.GetStreamingAssetsDirectory();
    //    EditorTools.ClearFolder(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(streamingAssetsDirectory)));
    //     刷新目录
    //    AssetDatabase.Refresh();
    //}

    [CreateAssetMenu(fileName = "AssetsBundleSettings", menuName = "ScriptableObjects/AssetsBundleSettings", order = 1)]
    public class AssetsBundleSettings : ScriptableObject
    {
        [LabelText("游戏类型")]
        [OnValueChanged("OnChanged_GameType")]
        [ValueDropdown("GetGameTypeValues", ExpandAllMenuItems = true)]
        public GameType GameType;

#if UNITY_EDITOR

        [LabelText("构建平台")]
        [HorizontalGroup("BuildTarget")]
        [OnValueChanged("OnChanged_BuildTarget")]
        [ValueDropdown("GetBuildTargetValues", ExpandAllMenuItems = true)]
        public BuildTarget BuildTarget;

        [HorizontalGroup("BuildTarget", 150)]
        [Button("手动触发判断执行", 30)]
        private void RunBuildTarget()
        {
            OnChanged_BuildTarget();
        }

#endif

        [LabelText("是否热更")]
        public bool IsHotfix;

        [LabelText("热更模式")]
        [HorizontalGroup("002")]
        [OnValueChanged("OnChanged_HotfixMode")]
        [ValueDropdown("GetHotfixModeValues", ExpandAllMenuItems = true)]
        public HotfixMode HotfixMode;

#if UNITY_EDITOR

        [HorizontalGroup("002", 150)]
        [Button("手动触发判断执行", 30)]
        private void RunHotfixMode()
        {
            OnChanged_HotfixMode();
        }

#endif

        [LabelText("加密类型")]
        public EncryptType EncryptType;

        [LabelText("加密密码")]
        public string EncryptPassword;

        [LabelText("加密偏移量")]
        public ulong EncryptOffsetVolume;

        [ValueDropdown("GetPlayModeValues", ExpandAllMenuItems = true)]
        [LabelText("运行模式")]
        [ReadOnly]
        public EPlayMode PlayMode;

        [HorizontalGroup("PackageName")]
        [LabelText("包名称")]
        [ReadOnly]
        public string PackageName;

#if UNITY_EDITOR
        [HorizontalGroup("PackageName", 150)]
        [Button("获取包名", 30)]
        private void GetPackageName()
        {
            PackageName = Assembly.Load("Unity.Editor").GetType("EditorHelper").InvokeMember("GetPackageName",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null) as string;
        }
#endif

#if UNITY_EDITOR
        [HorizontalGroup("005")]
        [Button("切换到构建模式", 30)]
        private void GotoBuildMode()
        {
            var assembly = Assembly.Load("Unity.Editor");
            assembly.GetType("BuildEditor").InvokeMember("GotoBuildMode", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

        [HorizontalGroup("005")]
        [Button("切换到开发模式", 30)]
        private void GotoDevelopMode()
        {
            var assembly = Assembly.Load("Unity.Editor");
            assembly.GetType("BuildEditor").InvokeMember("GotoDevelopMode", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

        [HorizontalGroup("005")]
        [Button("打开ab包构建面板", 30)]
        private void OpenAssetBundleBuilderSetting()
        {
            var assembly = Assembly.Load("Unity.Editor");
            assembly.GetType("BuildEditor").InvokeMember("OpenAssetBundleBuilderSetting", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

        [PropertySpace(SpaceBefore = 30)]
        [HorizontalGroup("006")]
        [Button("打开Build Settings面板", 30)]
        private void OpenBuildSettings()
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
            //BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, $"../../bin/{PlatformHelper.GetPlatformSign()}", EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        }

        [PropertySpace(SpaceBefore = 30)]
        [HorizontalGroup("007")]
        [Button("打开微信小程序转换面板", 30)]
        private void OpenWXTransform()
        {
            EditorApplication.ExecuteMenuItem("微信小游戏/转换小游戏");
        }

#endif

#if UNITY_EDITOR

        public static IEnumerable GetPlayModeValues = new ValueDropdownList<EPlayMode>()
        {
            {"编辑器模拟模式", EPlayMode.EditorSimulateMode}, {"单机模式", EPlayMode.OfflinePlayMode}, {"联机模式", EPlayMode.HostPlayMode}, {"WebGL模式", EPlayMode.WebPlayMode},
        };

        public static IEnumerable GetHotfixModeValues
        {
            get
            {
                var settings = Resources.Load<AssetsBundleSettings>("AssetsBundleSettings");

                if (settings.IsHotfix)
                {
                    yield return new ValueDropdownItem<HotfixMode>("ILRunTime", HotfixMode.ILRunTime);
                    yield return new ValueDropdownItem<HotfixMode>("HybridCLR", HotfixMode.HybridCLR);
                }
                else
                {
                    yield return new ValueDropdownItem<HotfixMode>("None", HotfixMode.None);
                }
            }
        }

        public static IEnumerable GetGameTypeValues
        {
            get
            {
                yield return new ValueDropdownItem<GameType>("普通游戏", GameType.NormalGame);
                yield return new ValueDropdownItem<GameType>("微信小游戏", GameType.WXGame);
                yield return new ValueDropdownItem<GameType>("抖音小游戏", GameType.TTGame);
                yield return new ValueDropdownItem<GameType>("QQ小游戏", GameType.QQGame);
                yield return new ValueDropdownItem<GameType>("Google免安装游戏", GameType.GoogleGame);
                yield return new ValueDropdownItem<GameType>("Facebook小游戏", GameType.FacebookGame);
            }
        }

        public static IEnumerable GetBuildTargetValues
        {
            get
            {
                var settings = Resources.Load<AssetsBundleSettings>("AssetsBundleSettings");

                switch (settings.GameType)
                {
                    case GameType.NormalGame:
                        yield return new ValueDropdownItem<BuildTarget>("PC-64", BuildTarget.StandaloneWindows64);
                        yield return new ValueDropdownItem<BuildTarget>("Android", BuildTarget.Android);
                        yield return new ValueDropdownItem<BuildTarget>("IOS", BuildTarget.iOS);
                        yield return new ValueDropdownItem<BuildTarget>("WebGL", BuildTarget.WebGL);
                        yield return new ValueDropdownItem<BuildTarget>("XboxOne", BuildTarget.XboxOne);
                        yield return new ValueDropdownItem<BuildTarget>("PS4", BuildTarget.PS4);
                        yield return new ValueDropdownItem<BuildTarget>("PS5", BuildTarget.PS5);
                        yield return new ValueDropdownItem<BuildTarget>("Switch", BuildTarget.Switch);

                        break;
                    case GameType.WXGame:
                        yield return new ValueDropdownItem<BuildTarget>("WebGL", BuildTarget.WebGL);

                        break;
                    case GameType.TTGame:
                        yield return new ValueDropdownItem<BuildTarget>("Android", BuildTarget.Android);
                        yield return new ValueDropdownItem<BuildTarget>("WebGL", BuildTarget.WebGL);

                        break;
                    case GameType.QQGame:
                        yield return new ValueDropdownItem<BuildTarget>("Android", BuildTarget.Android);
                        yield return new ValueDropdownItem<BuildTarget>("WebGL", BuildTarget.WebGL);

                        break;
                    case GameType.GoogleGame:
                        yield return new ValueDropdownItem<BuildTarget>("Android", BuildTarget.Android);

                        break;
                    case GameType.FacebookGame:
                        yield return new ValueDropdownItem<BuildTarget>("WebGL", BuildTarget.WebGL);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnChanged_HotfixMode()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            var assembly = Assembly.Load("Unity.Editor");
            assembly.GetType("BuildEditor").InvokeMember("ToggleHotfixMode", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

        private void OnChanged_GameType()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private void OnChanged_BuildTarget()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            BuildTargetGroup buildTargetGroup;

            switch (BuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    EditorUserBuildSettings.compressFilesInPackage = true;
                    EditorUserBuildSettings.SetCompressionType(buildTargetGroup, Compression.Lz4HC);

                    break;

                case BuildTarget.iOS:
                    buildTargetGroup = BuildTargetGroup.iOS;
                    EditorUserBuildSettings.compressFilesInPackage = true;
                    EditorUserBuildSettings.SetCompressionType(buildTargetGroup, Compression.Lz4HC);

                    break;

                case BuildTarget.Android:
                    buildTargetGroup = BuildTargetGroup.Android;
                    EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32BitDownscaled;
                    EditorUserBuildSettings.compressFilesInPackage = true;
                    EditorUserBuildSettings.SetCompressionType(buildTargetGroup, Compression.Lz4HC);

                    break;

                case BuildTarget.StandaloneWindows64:
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    EditorUserBuildSettings.compressFilesInPackage = true;

                    break;

                case BuildTarget.WebGL:
                    buildTargetGroup = BuildTargetGroup.WebGL;
                    EditorUserBuildSettings.compressFilesInPackage = true;

                    if (GameType != GameType.WXGame)
                    {
                        PlayerSettings.WebGL.template = "APPLICATION:Default";
                    }

                    PlayerSettings.WebGL.memorySize = 1200;

                    break;

                case BuildTarget.WSAPlayer:
                    buildTargetGroup = BuildTargetGroup.WSA;

                    break;

                case BuildTarget.StandaloneLinux64:
                    buildTargetGroup = BuildTargetGroup.Standalone;

                    break;

                case BuildTarget.PS4:
                    buildTargetGroup = BuildTargetGroup.PS4;

                    break;

                case BuildTarget.XboxOne:
                    buildTargetGroup = BuildTargetGroup.XboxOne;

                    break;

                case BuildTarget.tvOS:
                    buildTargetGroup = BuildTargetGroup.tvOS;

                    break;

                case BuildTarget.Switch:
                    buildTargetGroup = BuildTargetGroup.Switch;

                    break;

                case BuildTarget.Lumin:
                    buildTargetGroup = BuildTargetGroup.Lumin;

                    break;

                case BuildTarget.Stadia:
                    buildTargetGroup = BuildTargetGroup.Stadia;

                    break;

                case BuildTarget.GameCoreXboxOne:
                    buildTargetGroup = BuildTargetGroup.GameCoreXboxOne;

                    break;

                case BuildTarget.PS5:
                    buildTargetGroup = BuildTargetGroup.PS5;

                    break;

                case BuildTarget.EmbeddedLinux:
                    buildTargetGroup = BuildTargetGroup.EmbeddedLinux;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (BuildTarget == BuildTarget.WebGL)
            {
                EncryptType = EncryptType.None;
            }
            else
            {
                EncryptType = EncryptType.Offset;
            }

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            var assembly = Assembly.Load("Unity.Editor");
            assembly.GetType("BuildEditor").InvokeMember("TogglePlatform", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

#endif
    }
}