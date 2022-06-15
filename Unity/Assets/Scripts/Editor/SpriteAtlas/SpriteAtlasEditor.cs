using CatJson;
using Cysharp.Threading.Tasks;
using Model;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasEditor
{
    [MenuItem("Tools/导出UI图集")]
    public static void ExportAndSettingAllUISpriteAtlas()
    {
        ExportAllUISpriteAtlas();

        UniTask.Void(async () =>
        {
            await UniTask.Delay(1000);

            ImportSettingsEditor.SetUIAtlasResMenu();
        });
    }

    public static void ExportAllUISpriteAtlas()
    {
        SpriteAtlasSettings settings = AssetDatabase.LoadAssetAtPath<SpriteAtlasSettings>(EditorConfig.SPRITE_ATLAS_SETTINGS);

        if (Directory.Exists(EditorConfig.ATLAS_PATH))
        {
            FileHelper.DelectDir(EditorConfig.ATLAS_PATH);
        }
        else
        {
            FileHelper.CreateDir(EditorConfig.ATLAS_PATH);
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

        SpriteAtlas template = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(EditorConfig.UI_Atlas);
        List<SpriteAtlas> list = new List<SpriteAtlas>(settings.CollectorList.Count);
        List<string> pathList = new List<string>();
        Dictionary<string, string> infoDic = new Dictionary<string, string>();
        for (int i = 0; i < settings.CollectorList.Count; i++)
        {
            EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(settings.CollectorList[i]));
            if (pathList.Count > 0)
            {
                SpriteAtlas spriteAtlas = Object.Instantiate(template);
                spriteAtlas.name = $"{settings.CollectorList[i].name}Atlas";
                spriteAtlas.Add(new[] { settings.CollectorList[i] });
                AssetDatabase.CreateAsset(spriteAtlas, $"{EditorConfig.ATLAS_PATH}/{spriteAtlas.name}.spriteatlas");
                EditorUtility.SetDirty(spriteAtlas);
                SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

                list.Add(spriteAtlas);
                var path = AssetDatabase.GetAssetPath(spriteAtlas);
                var str = Path.GetFileNameWithoutExtension(Regex.Replace(path, EditorConfig.ATLAS_PATH, ""));
                for (int j = 0; j < pathList.Count; j++)
                {
                    if (!infoDic.ContainsKey(pathList[j]))
                    {
                        infoDic.Add(pathList[j], str);
                    }
                }
            }
            else
            {
                Debug.Log($"{AssetDatabase.GetAssetPath(settings.CollectorList[i])} 文件夹为空！"); // MDEBUG:
            }
            pathList.Clear();
        }

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(EditorConfig.UI_Sprite_Info))
        {
            sw.WriteLine(JsonParser.ToJson(infoDic));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("导出UI图集--完成！"); // MDEBUG:
    }

    [MenuItem("Assets/工具/导出UI图集")]
    public static void ExportAndSettingUISpriteAtlasBySelect()
    {
        var settings = AssetDatabase.LoadAssetAtPath<SpriteAtlasSettings>(EditorConfig.SPRITE_ATLAS_SETTINGS);
        var infoDic = JsonParser.ParseJson<Dictionary<string, string>>(AssetDatabase.LoadAssetAtPath<TextAsset>(EditorConfig.UI_Sprite_Info).text);
        var template = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(EditorConfig.UI_Atlas);

        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            var da = AssetDatabase.LoadAssetAtPath<DefaultAsset>(
                AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
            if (da != null && settings.CollectorList.Contains(da))
            {
                ExportAndSettingUISpriteAtlas(da, infoDic, template);
            }
            else
            {
                Debug.LogError($"你选择的不是文件夹或者该文件夹没有配置到>{EditorConfig.SPRITE_ATLAS_SETTINGS}"); // MDEBUG:
            }
        }

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(EditorConfig.UI_Sprite_Info))
        {
            sw.WriteLine(JsonParser.ToJson(infoDic));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("导出UI图集--完成！"); // MDEBUG:
    }

    public static void ExportAndSettingUISpriteAtlas(DefaultAsset da, Dictionary<string, string> infoDic, SpriteAtlas template)
    {
        List<string> pathList = new List<string>();

        EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(da));
        if (pathList.Count > 0)
        {
            SpriteAtlas spriteAtlas = Object.Instantiate(template);
            spriteAtlas.name = $"{da.name}Atlas";
            spriteAtlas.Add(new Object[] { da });

            var path = $"{EditorConfig.ATLAS_PATH}/{spriteAtlas.name}.spriteatlas";
            var str = Path.GetFileNameWithoutExtension(Regex.Replace(path, EditorConfig.ATLAS_PATH, ""));

            for (int i = 0; i < pathList.Count; i++)
            {
                if (!infoDic.ContainsKey(pathList[i]))
                {
                    infoDic.Add(pathList[i], str);
                }
            }

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
}