using System;
using M.ProductionPipeline;
using Model;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using System.Collections.ObjectModel;

public class BuildEditor
{
    private const string LOCAL_HOTFIX_SERVER_CLOSE = "Tools/AssetBundle/关闭本地热更服务器";
    private const string LOCAL_HOTFIX_SERVER_OPEN  = "Tools/AssetBundle/开启本地热更服务器";

    private const string GOTO_BUILD_MODE   = "Tools/Build/切换到构建模式";
    private const string GOTO_DEVELOP_MODE = "Tools/Build/切换到开发模式";

    [MenuItem(GOTO_BUILD_MODE)]
    public static void GotoBuildMode()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<StepCollector>(EditorConst.GOTO_BUILD_MODE);
    }

    [MenuItem(GOTO_DEVELOP_MODE)]
    public static void GotoDevelopMode()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<StepCollector>(EditorConst.GOTO_DEVELOP_MODE);
    }

    public static void OpenAssetBundleBuilderSetting()
    {
        var assetBundleBuilderSetting = AssetDatabase.LoadAssetAtPath<AssetBundleBuilderSetting>(EditorConst.ASSET_BUNDLE_BUILDER_SETTING_PATH);
        var assetsBundleSettings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

        switch (assetsBundleSettings.EncryptType)
        {
            case EncryptType.None:
                assetBundleBuilderSetting.EncyptionClassName = "EncryptionNone";

                break;
            case EncryptType.Empty:
                assetBundleBuilderSetting.EncyptionClassName = "EmptyEncrypt";

                break;
            case EncryptType.Offset:
                assetBundleBuilderSetting.EncyptionClassName = "OffsetEncrypt";

                break;
        }

        if (assetsBundleSettings.PlayMode == EPlayMode.WebPlayMode)
        {
            assetBundleBuilderSetting.CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll;
            assetBundleBuilderSetting.OutputNameStyle = EOutputNameStyle.BundleName_HashName;
        }
        else
        {
            assetBundleBuilderSetting.CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll;
            assetBundleBuilderSetting.OutputNameStyle = EOutputNameStyle.HashName;
        }

        UnityEditor.EditorUtility.SetDirty(assetBundleBuilderSetting);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        AssetBundleBuilderWindow.OpenWindow();
    }

    public static void ToggleHotfixMode()
    {
        var stepList = new List<IStep>();
        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

        if (settings.HotfixMode == HotfixMode.ILRunTime)
        {
            stepList.Add(new RemoveDefineStep() { Name = "HybridCLR" });
            stepList.Add(new CloseHybridCLRStep());
            stepList.Add(new OpenILRuntimeStep());
            stepList.Add(new AddDefineStep() { Name = "ILRuntime" });
            stepList.Add(new ILRuntimeCLRClearStep());
            stepList.Add(new ILRuntimeCLRBindingStep());
            stepList.Add(new HotfixEditorStep());
        }
        else if (settings.HotfixMode == HotfixMode.HybridCLR)
        {
            stepList.Add(new ILRuntimeCLRClearStep());
            stepList.Add(new RemoveDefineStep() { Name = "ILRuntime" });
            stepList.Add(new CloseILRuntimeStep());
            stepList.Add(new OpenHybridCLRStep());
            stepList.Add(new AddDefineStep() { Name = "HybridCLR" });
            stepList.Add(new HotfixAnyStep());
        }
        else
        {
            stepList.Add(new ILRuntimeCLRClearStep());
            stepList.Add(new RemoveDefineStep() { Name = "ILRuntime" });
            stepList.Add(new CloseILRuntimeStep());
            stepList.Add(new RemoveDefineStep() { Name = "HybridCLR" });
            stepList.Add(new CloseHybridCLRStep());
            stepList.Add(new HotfixEditorStep());
        }

        if (stepList.Count > 0)
        {
            StepEditor.RunStepGroup(stepList);
        }
    }

    public static void TogglePlatform()
    {
        var stepList = new List<IStep>();
        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);

        if (settings.BuildTarget == BuildTarget.WebGL)
        {
            stepList.Add(new SwitchPlatformStep());
            stepList.Add(new CloseBetterStreamingAssetsStep());
        }
        else
        {
            stepList.Add(new OpenBetterStreamingAssetsStep());
            stepList.Add(new SwitchPlatformStep());
        }

        switch (settings.GameType)
        {
            case GameType.NormalGame:
                stepList.Add(new RemoveDefineStep() { Name = "WX" });
                stepList.Add(new CloseWXMiniGameStep());
                stepList.Add(new RemoveDefineStep() { Name = "TT" });
                stepList.Add(new CloseTTMiniGameStep());

                break;
            case GameType.WXGame:
                stepList.Add(new RemoveDefineStep() { Name = "TT" });
                stepList.Add(new CloseTTMiniGameStep());
                stepList.Add(new OpenWXMiniGameStep());
                stepList.Add(new AddDefineStep() { Name = "WX" });

                break;
            case GameType.TTGame:
                stepList.Add(new RemoveDefineStep() { Name = "WX" });
                stepList.Add(new CloseWXMiniGameStep());
                stepList.Add(new OpenTTMiniGameStep());
                stepList.Add(new AddDefineStep() { Name = "TT" });

                break;
            case GameType.QQGame:
                stepList.Add(new RemoveDefineStep() { Name = "WX" });
                stepList.Add(new CloseWXMiniGameStep());
                stepList.Add(new RemoveDefineStep() { Name = "TT" });
                stepList.Add(new CloseTTMiniGameStep());

                break;
            case GameType.GoogleGame:
                stepList.Add(new RemoveDefineStep() { Name = "WX" });
                stepList.Add(new CloseWXMiniGameStep());
                stepList.Add(new RemoveDefineStep() { Name = "TT" });
                stepList.Add(new CloseTTMiniGameStep());

                break;
            case GameType.FacebookGame:
                stepList.Add(new RemoveDefineStep() { Name = "WX" });
                stepList.Add(new CloseWXMiniGameStep());
                stepList.Add(new RemoveDefineStep() { Name = "TT" });
                stepList.Add(new CloseTTMiniGameStep());

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (settings.PlayMode != EPlayMode.EditorSimulateMode)
        {
            stepList.AddRange(StepCollector.GetSteps(AssetDatabase.LoadAssetAtPath<StepCollector>(EditorConst.GOTO_BUILD_MODE)));
        }

        if (stepList.Count > 0)
        {
            StepEditor.RunStepGroup(stepList);
        }
    }

    [MenuItem("Tools/导出配置表 _F2", priority = 2)]
    public static void ExcelExport()
    {
#if MBuild
        new GenCodeBinStep().Run();
#else
        new GenCodeJsonStep().Run();
#endif

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("导出配置表!--完成");
    }

    [MenuItem("Tools/打开AssetsBundleSettings面板 _F3", priority = 3)]
    public static void OpenAssetsBundleSettings()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);
    }

    [MenuItem(LOCAL_HOTFIX_SERVER_CLOSE)]
    public static void CloseLocalHotfixServer()
    {
        Menu.SetChecked(LOCAL_HOTFIX_SERVER_CLOSE, true);
        Menu.SetChecked(LOCAL_HOTFIX_SERVER_OPEN, false);
        EditorHelper.RunMyBat("关闭本地热更服务器.bat", "../Tools/openresty-1.19.9.1-Hotfix/");

        Debug.Log("关闭本地热更服务器!--完成");
    }

    [MenuItem(LOCAL_HOTFIX_SERVER_OPEN)]
    public static void OpenLocalHotfixServer()
    {
        Menu.SetChecked(LOCAL_HOTFIX_SERVER_CLOSE, false);
        Menu.SetChecked(LOCAL_HOTFIX_SERVER_OPEN, true);
        EditorHelper.RunMyBat("开启本地热更服务器.bat", "../Tools/openresty-1.19.9.1-Hotfix/");

        Debug.Log("开启本地热更服务器!--完成");
    }
}