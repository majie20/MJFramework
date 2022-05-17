using Model;
using UnityEditor;
using UnityEngine;
using YooAsset;

public class BuildEditor : EditorWindow
{
    [MenuItem("Tools/Build/切换到构建模式")]
    public static void GotoBuildTestMode()
    {
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.Standalone);
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.iOS);
        EditorHelper.AddDefineSymbols("MBuild", BuildTargetGroup.Android);

        ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();

        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        if (settings.IsOfflineGame)
        {
            settings.PlayMode = YooAssets.EPlayMode.OfflinePlayMode;
        }
        else
        {
            settings.PlayMode = YooAssets.EPlayMode.HostPlayMode;
        }
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        CatJson.Editor.JsonCodeGenerator.GenJsonCode();
        ImportSettingsEditor.SetAllRes();

        EditorHelper.RunMyBat("gen_code_bin一键导出.bat", "../Excel/");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("切换到构建模式!--完成");
    }

    [MenuItem("Tools/Build/切换到开发模式")]
    public static void GotoDevelopMode()
    {
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.Standalone);
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.iOS);
        EditorHelper.RemoveDefineSymbols("MBuild", BuildTargetGroup.Android);

        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        settings.PlayMode = YooAssets.EPlayMode.EditorPlayMode;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        CatJson.Editor.JsonCodeGenerator.ClearJsonCode();

        EditorHelper.RunMyBat("gen_code_json一键导出.bat", "../Excel/");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("切换到开发模式!--完成");
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