using CatJson;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using vietlabs.fr2;

public class AssetReferenceEditor
{
    [MenuItem("Assets/资源收集/查看UI预制体引用图集情况", priority = 0)]
    private static void CheckUIPrefabReferenceAtlas()
    {
        List<string> pathList = new List<string>();

        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            EditorHelper.GetAssetPath(pathList, AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
        }

        Dictionary<string, List<string>> infoDic = new Dictionary<string, List<string>>();

        for (int i = 0; i < pathList.Count; i++)
        {
            CheckUIPrefabReferenceAtlas(pathList[i], infoDic);
        }

        foreach (var info in infoDic)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{info.Key}引用的静态图集：\n");

            for (int i = 0; i < info.Value.Count; i++)
            {
                sb.Append(info.Value[i]);
                sb.Append("\n");
            }

            if (info.Value.Count > 1)
            {
                Debug.LogError(sb); // MDEBUG:
            }
            else
            {
                Debug.Log(sb); // MDEBUG:
            }
        }

        Debug.Log($"查看UI预制体引用图集情况--完成"); // MDEBUG:
    }

    [MenuItem("Tools/资源收集/检查所有UI预制体引用图集情况", priority = 0)]
    public static void CheckUIPrefabReferenceAtlasAll()
    {
        FR2_Cache.Api.Check4Changes(true);
        FR2_SceneCache.Api.SetDirty();

        Dictionary<string, List<string>> infoDic = new Dictionary<string, List<string>>();
        List<string> pathList = new List<string>();
        EditorHelper.GetAssetPath(pathList, EditorConst.UI_PREFAB_PATH);

        for (int i = 0; i < pathList.Count; i++)
        {
            CheckUIPrefabReferenceAtlas(pathList[i], infoDic);
        }

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter("UI预制体引用图集情况表.json"))
        {
            sw.WriteLine(JsonParser.Default.ToJson(infoDic));
        }

        Debug.Log($"检查所有UI预制体引用图集情况--完成"); // MDEBUG:
    }

    private static void CheckUIPrefabReferenceAtlas(string assetPath, Dictionary<string, List<string>> infoDic)
    {
        if (!assetPath.StartsWith(EditorConst.UI_PREFAB_PATH))
        {
            Debug.LogError($"{assetPath}不是UI预制体!"); // MDEBUG:

            return;
        }

        var list = AssetDatabase.GetDependencies(assetPath, true);
        var textureList = new List<string>();

        for (int i = 0; i < list.Length; i++)
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(list[i]) is Texture && !list[i].StartsWith(EditorConst.DYNAMIC_SPRITE_PATH))
            {
                textureList.Add(list[i]);
            }
        }

        if (textureList.Count == 0)
        {
            return;
        }

        var atlasList = new List<string>();

        for (int i = 0; i < textureList.Count; i++)
        {
            var dic = FR2_Ref.FindUsedBy(new[] { AssetDatabase.AssetPathToGUID(textureList[i]) });

            foreach (var value in dic.Values)
            {
                if (value.asset.extension == ".spriteatlas" && !atlasList.Contains(value.asset.assetPath))
                {
                    atlasList.Add(value.asset.assetPath);
                }
            }
        }

        infoDic.Add(assetPath, atlasList);
    }
}