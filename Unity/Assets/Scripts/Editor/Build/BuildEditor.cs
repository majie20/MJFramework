using Cysharp.Threading.Tasks;
using FMODUnity;
using Model;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public class BuildEditor : EditorWindow
{
    [MenuItem("Tools/Build/切换到构建模式--执行1")]
    public static async UniTask GotoBuildTestMode1()
    {
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.Standalone);
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.iOS);
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.Android);

        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        if (settings.IsOfflineGame)
        {
            settings.PlayMode = YooAssets.EPlayMode.OfflinePlayMode;
        }
        else
        {
            settings.PlayMode = YooAssets.EPlayMode.HostPlayMode;
        }
        EditorUtility.SetDirty(Settings.Instance);

        Settings.Instance.ImportType = ImportType.AssetBundle;
        EditorUtility.SetDirty(Settings.Instance);

        CatJson.Editor.JsonCodeGenerator.GenJsonCode();

        await RunPiece1();

        EditorHelper.RunMyBat("gen_code_bin一键导出.bat", "../Excel/");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.LogError("切换到构建模式--执行1!--完成");
    }

    [MenuItem("Tools/Build/切换到构建模式--执行2")]
    public static async UniTask GotoBuildTestMode2()
    {
        await RunPiece2();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.LogError("切换到构建模式--执行2!--完成");
    }

    [MenuItem("Tools/Build/切换到开发模式--执行1")]
    public static async UniTask GotoDevelopMode1()
    {
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.Standalone);
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.iOS);
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.Android);

        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        settings.PlayMode = YooAssets.EPlayMode.EditorSimulateMode;
        EditorUtility.SetDirty(settings);

        Settings.Instance.ImportType = ImportType.StreamingAssets;
        EditorUtility.SetDirty(Settings.Instance);

        CatJson.Editor.JsonCodeGenerator.ClearJsonCode();

        await RunPiece1();

        EditorHelper.RunMyBat("gen_code_json一键导出.bat", "../Excel/");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.LogError("切换到开发模式--执行1!--完成");
    }

    [MenuItem("Tools/Build/切换到开发模式--执行2")]
    public static async UniTask GotoDevelopMode2()
    {
        await RunPiece2();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.LogError("切换到开发模式--执行2!--完成");
    }

    public static async UniTask RunPiece1()
    {
        ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
        SpriteAtlasEditor.ExportAllUISpriteAtlas();
        ImportSettingsEditor.SetAllRes();
        var window = new UIPrefabToAtlasSettingsWindow();
        window.OnEnable();
        window.OnDestroy();

        var buildMapContext = BuildMapCreater.CreateBuildMap(EBuildMode.ForceRebuild);
        CheckBuildMapContent(buildMapContext);
    }

    public static async UniTask RunPiece2()
    {
        AssetReferenceEditor.CheckUIPrefabReferenceAtlasAll();

        AssetCollectorEditor.ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(EditorConfig.GAME_LOAD_COMPLETE_AC));
        AssetCollectorEditor.ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(EditorConfig.INIT_AC));
    }

    /// <summary>
    /// 检测构建结果
    /// </summary>
    private static void CheckBuildMapContent(BuildMapContext buildMapContext)
    {
        List<string> infoList = new List<string>();
        foreach (var bundleInfo in buildMapContext.BundleInfos)
        {
            // 注意：原生文件资源包只能包含一个原生文件
            bool isRawFile = bundleInfo.IsRawFile;
            if (isRawFile)
            {
                if (bundleInfo.BuildinAssets.Count != 1)
                    throw new System.Exception($"The bundle does not support multiple raw asset : {bundleInfo.BundleName}");
                continue;
            }

            // 注意：原生文件不能被其它资源文件依赖
            foreach (var assetInfo in bundleInfo.BuildinAssets)
            {
                if (assetInfo.AllDependAssetInfos != null)
                {
                    foreach (var dependAssetInfo in assetInfo.AllDependAssetInfos)
                    {
                        if (dependAssetInfo.IsRawAsset)
                            throw new System.Exception($"{assetInfo.AssetPath} can not depend raw asset : {dependAssetInfo.AssetPath}");
                        if (!dependAssetInfo.AssetPath.StartsWith(EditorConfig.RES_PATH))
                        {
                            infoList.Add($"{assetInfo.AssetPath} : {dependAssetInfo.AssetPath}");
                        }
                    }
                }
            }
        }

        if (infoList.Count > 0)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("引用外部资源情况表.json"))
            {
                sw.WriteLine(CatJson.JsonParser.ToJson(infoList));
            }
            UnityEngine.Debug.LogError($"请查看文件：引用外部资源情况表.json"); // MDEBUG:
        }
        else
        {
            UnityEngine.Debug.Log($"没有引用外部资源"); // MDEBUG:
        }
    }

    [MenuItem("Tools/导出配置表")]
    public static void ExcelExport()
    {
#if MBuild
        EditorHelper.RunMyBat("gen_code_bin一键导出.bat", "../Excel/");
#else
        EditorHelper.RunMyBat("gen_code_json一键导出.bat", "../Excel/");
#endif

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("导出配置表!--完成");
    }

    [MenuItem("Tools/AssetBundle/关闭本地热更服务器")]
    public static void CloseLocalHotfixServer()
    {
        EditorHelper.RunMyBat("关闭本地热更服务器.bat", "../Tools/openresty-1.19.9.1-Hotfix/");

        Debug.Log("关闭本地热更服务器!--完成");
    }

    [MenuItem("Tools/AssetBundle/开启本地热更服务器")]
    public static void OpenLocalHotfixServer()
    {
        EditorHelper.RunMyBat("开启本地热更服务器.bat", "../Tools/openresty-1.19.9.1-Hotfix/");

        Debug.Log("开启本地热更服务器!--完成");
    }
}