using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class ImportSettingsEditor
{
    private static TextureImporterFormat UI_ANDROID_FORMAT  = TextureImporterFormat.ETC2_RGBA8;
    private static TextureImporterFormat UI_ANDROID_FORMAT2 = TextureImporterFormat.ETC2_RGB4;

    private static TextureImporterFormat UI_IOS_FORMAT  = TextureImporterFormat.PVRTC_RGBA4;
    private static TextureImporterFormat UI_IOS_FORMAT2 = TextureImporterFormat.PVRTC_RGB4;

    private static TextureImporterFormat UI_STANDALONE_FORMAT  = TextureImporterFormat.DXT5;
    private static TextureImporterFormat UI_STANDALONE_FORMAT2 = TextureImporterFormat.DXT1;

    [MenuItem("Tools/ImportSettings/设置所有资源")]
    public static void SetAllRes()
    {
        ResTypeSettings settings = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH);

        SetUITextureRes(settings.UITextureResConfig);
        SetUIAtlasRes(settings.UIAtlasResConfig);
        UnityEngine.Debug.Log($"设置所有资源完成！"); // MDEBUG:
    }

    #region UI资源设置

    [MenuItem("Tools/ImportSettings/设置UI图片资源")]
    public static void SetUITextureResMenu()
    {
        SetUITextureRes();
    }

    public static void SetUITextureRes(ConfigInfo info = null)
    {
        if (info == null)
        {
            info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UITextureResConfig;
        }

        List<string> pathList = new List<string>();

        for (int i = 0; i < info.list.Count; i++)
        {
            EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(info.list[i]));
        }

        SetUITextureResByList(pathList, info);
        Debug.Log("设置UI图片资源--完成！"); // MDEBUG:
    }

    public static void SetUITextureResByList(List<string> pathList, ConfigInfo info = null)
    {
        if (info == null)
        {
            info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UITextureResConfig;
        }

        var source = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(info.target)) as TextureImporter;

        TextureImporterSettings settings = new TextureImporterSettings();
        source.ReadTextureSettings(settings);

        TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
        source.GetPlatformTextureSettings("Android").CopyTo(androidSettings);

        TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
        source.GetPlatformTextureSettings("iPhone").CopyTo(iosSettings);

        TextureImporterPlatformSettings standaloneSettings = new TextureImporterPlatformSettings();
        source.GetPlatformTextureSettings("Standalone").CopyTo(standaloneSettings);

        TextureImporterPlatformSettings webglSettings = new TextureImporterPlatformSettings();
        source.GetPlatformTextureSettings("WebGL").CopyTo(webglSettings);

        for (int i = 0; i < pathList.Count; i++)
        {
            var temp = AssetImporter.GetAtPath(pathList[i]) as TextureImporter;

            TextureImporterPlatformSettings android = new TextureImporterPlatformSettings();
            androidSettings.CopyTo(android);
            android.format = temp.DoesSourceTextureHaveAlpha() ? UI_ANDROID_FORMAT : UI_ANDROID_FORMAT2;

            TextureImporterPlatformSettings ios = new TextureImporterPlatformSettings();
            iosSettings.CopyTo(ios);
            ios.format = temp.DoesSourceTextureHaveAlpha() ? UI_IOS_FORMAT : UI_IOS_FORMAT2;

            TextureImporterPlatformSettings standalone = new TextureImporterPlatformSettings();
            standaloneSettings.CopyTo(standalone);
            standalone.format = temp.DoesSourceTextureHaveAlpha() ? UI_STANDALONE_FORMAT : UI_STANDALONE_FORMAT2;

            TextureImporterPlatformSettings webgl = new TextureImporterPlatformSettings();
            webglSettings.CopyTo(webgl);
            webglSettings.format = temp.DoesSourceTextureHaveAlpha() ? UI_STANDALONE_FORMAT : UI_STANDALONE_FORMAT2;

            //temp.SetTextureSettings(settings);
            temp.SetPlatformTextureSettings(android);
            temp.SetPlatformTextureSettings(ios);
            temp.SetPlatformTextureSettings(standalone);
            temp.SetPlatformTextureSettings(webglSettings);

            EditorUtility.SetDirty(temp);
            AssetDatabase.ImportAsset(pathList[i]);
            AssetDatabase.WriteImportSettingsIfDirty(pathList[i]);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ImportSettings/设置选中的UI图片资源", priority = 1)]
    public static void SelectSetUITextureResMenu()
    {
        List<string> pathList = GetSelectObjectPathList();

        var info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UITextureResConfig;

        List<string> tempList = new List<string>();

        for (int i = 0; i < pathList.Count; i++)
        {
            var result = false;

            for (int j = 0; j < info.list.Count; j++)
            {
                if (pathList[i].StartsWith(AssetDatabase.GetAssetPath(info.list[j])))
                {
                    result = true;

                    break;
                }
            }

            if (result)
            {
                tempList.Add(pathList[i]);
            }
        }

        SetUITextureResByList(pathList, info);
        Debug.Log("设置选中的UI图片资源--完成！"); // MDEBUG:
    }

    #endregion UI资源设置

    #region Atlas资源设置

    [MenuItem("Tools/ImportSettings/设置UI图集资源")]
    public static void SetUIAtlasResMenu()
    {
        SetUIAtlasRes();
    }

    public static void SetUIAtlasRes(ConfigInfo info = null)
    {
        if (info == null)
        {
            info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UIAtlasResConfig;
        }

        List<string> pathList = new List<string>();

        for (int i = 0; i < info.list.Count; i++)
        {
            EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(info.list[i]));
        }

        SetUIAtlasResByList(pathList, info);
        Debug.Log("设置UI图集资源--完成！"); // MDEBUG:
    }

    public static void SetUIAtlasResByList(List<string> pathList, ConfigInfo info = null)
    {
        if (info == null)
        {
            info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UIAtlasResConfig;
        }

        var source = info.target as SpriteAtlas;

        SpriteAtlasTextureSettings textureSettings = source.GetTextureSettings();
        SpriteAtlasPackingSettings packingSettings = source.GetPackingSettings();

        TextureImporterPlatformSettings androidSettings = source.GetPlatformSettings("Android");

        TextureImporterPlatformSettings iosSettings = source.GetPlatformSettings("iPhone");

        TextureImporterPlatformSettings standaloneSettings = source.GetPlatformSettings("Standalone");

        TextureImporterPlatformSettings webglSettings = source.GetPlatformSettings("WebGL");

        AssetDatabase.StartAssetEditing();
        List<SpriteAtlas> atlasList = new List<SpriteAtlas>(pathList.Count);

        for (int i = 0; i < pathList.Count; i++)
        {
            var temp = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(pathList[i]);

            temp.SetTextureSettings(textureSettings);
            temp.SetPackingSettings(packingSettings);
            temp.SetPlatformSettings(androidSettings);
            temp.SetPlatformSettings(iosSettings);
            temp.SetPlatformSettings(standaloneSettings);
            temp.SetPlatformSettings(webglSettings);
            temp.SetIncludeInBuild(true);
            temp.SetIsVariant(false);

            EditorUtility.SetDirty(temp);
            atlasList.Add(temp);
        }

        SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ImportSettings/设置选中的UI图集资源", priority = 1)]
    public static void SelectSetUIAtlasResMenu()
    {
        List<string> pathList = GetSelectObjectPathList();

        var info = AssetDatabase.LoadAssetAtPath<ResTypeSettings>(EditorConst.RES_TYPE_SETTINGS_PATH).UIAtlasResConfig;

        List<string> tempList = new List<string>();

        for (int i = 0; i < pathList.Count; i++)
        {
            var result = false;

            for (int j = 0; j < info.list.Count; j++)
            {
                if (pathList[i].StartsWith(AssetDatabase.GetAssetPath(info.list[j])))
                {
                    result = true;

                    break;
                }
            }

            if (result)
            {
                tempList.Add(pathList[i]);
            }
        }

        SetUIAtlasResByList(pathList, info);

        Debug.Log("设置选中的UI图集资源--完成！"); // MDEBUG:
    }

    #endregion Atlas资源设置

    public static List<string> GetSelectObjectPathList()
    {
        List<string> pathList = new List<string>();

        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
            EditorHelper.GetAssetPath(pathList, path);
        }

        return pathList;
    }
}