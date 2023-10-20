using Model;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasEditor
{
    [MenuItem("Tools/导出图集/导出所有UI图集", priority = 3)]
    public static void ExportAndSettingAllUISpriteAtlas()
    {
        ExportAllUISpriteAtlas(EditorConst.UI_SPRITE_ATLAS_SETTINGS);
    }

    [MenuItem("Tools/导出图集/导出所有角色和怪物图集", priority = 3)]
    public static void ExportAndSettingAllUnitSpriteAtlas()
    {
        ExportAllUISpriteAtlas(EditorConst.UNIT_SPRITE_ATLAS_SETTINGS);
    }

    public static void ExportAllUISpriteAtlas(string url)
    {
        SpriteAtlasSettings settings = AssetDatabase.LoadAssetAtPath<SpriteAtlasSettings>(url);
        var folderPath = AssetDatabase.GetAssetPath(settings.Folder);

        if (Directory.Exists(folderPath))
        {
            FileHelper.DelectDir(folderPath);
        }
        else
        {
            FileHelper.CreateDir(folderPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Dictionary<string, Object> dictionary = new Dictionary<string, Object>();

        for (int i = 0; i < settings.CollectorList.Count; i++)
        {
            var name = settings.CollectorList[i].name;

            if (dictionary.ContainsKey(settings.CollectorList[i].name))
            {
                Debug.LogError($"有名字相同的两个文件夹：\n{AssetDatabase.GetAssetPath(dictionary[name])}\n{AssetDatabase.GetAssetPath(settings.CollectorList[i])}"); // MDEBUG:

                return;
            }

            dictionary.Add(name, settings.CollectorList[i]);
        }

        SpriteAtlas template = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(EditorConst.UI_Atlas);
        List<string> pathList = new List<string>();

        for (int i = 0; i < settings.CollectorList.Count; i++)
        {
            EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(settings.CollectorList[i]));

            if (pathList.Count > 0)
            {
                SpriteAtlas spriteAtlas = Object.Instantiate(template);
                spriteAtlas.name = $"{settings.CollectorList[i].name}Atlas";
                spriteAtlas.Add(new[] { settings.CollectorList[i] });
                AssetDatabase.CreateAsset(spriteAtlas, $"{folderPath}/{spriteAtlas.name}.spriteatlas");
                EditorUtility.SetDirty(spriteAtlas);
                SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
            }
            else
            {
                Debug.Log($"{AssetDatabase.GetAssetPath(settings.CollectorList[i])} 文件夹为空！"); // MDEBUG:
            }

            pathList.Clear();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("导出图集--完成！"); // MDEBUG:
    }

    [MenuItem("Assets/工具/导出图集")]
    public static void ExportAndSettingUISpriteAtlasBySelect()
    {
        var settings1 = AssetDatabase.LoadAssetAtPath<SpriteAtlasSettings>(EditorConst.UI_SPRITE_ATLAS_SETTINGS);
        var settings2 = AssetDatabase.LoadAssetAtPath<SpriteAtlasSettings>(EditorConst.UNIT_SPRITE_ATLAS_SETTINGS);

        var template = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(EditorConst.UI_Atlas);

        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            var da = AssetDatabase.LoadAssetAtPath<DefaultAsset>(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));

            SpriteAtlasSettings settings = null;

            if (Selection.assetGUIDs[i].StartsWith(AssetDatabase.GetAssetPath(settings1.Folder)))
            {
                settings = settings1;
            }

            if (da != null && settings != null && settings.CollectorList.Contains(da))
            {
                List<string> pathList = new List<string>();

                EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(da));

                if (pathList.Count > 0)
                {
                    SpriteAtlas spriteAtlas = Object.Instantiate(template);
                    spriteAtlas.name = $"{da.name}Atlas";
                    spriteAtlas.Add(new Object[] { da });

                    var path = $"{AssetDatabase.GetAssetPath(settings.Folder)}/{spriteAtlas.name}.spriteatlas";

                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.CreateAsset(spriteAtlas, path);
                    EditorUtility.SetDirty(spriteAtlas);
                    SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
                }
                else
                {
                    Debug.Log($"{AssetDatabase.GetAssetPath(da)} 文件夹为空！"); // MDEBUG:
                }
            }
            else
            {
                Debug.LogError($"你选择的不是文件夹或者该文件夹没有配置到指定文件内！"); // MDEBUG:
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("导出图集--完成！"); // MDEBUG:
    }
}