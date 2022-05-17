using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using vietlabs.fr2;

public class AssetReferenceEditor
{
    [MenuItem("Assets/资源引用/查看UI预制体引用图集", priority = 0)]
    private static void CheckUIPrefabReferenceAtlas()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!assetPath.StartsWith(EditorConfig.UI_PREFAB_PATH))
        {
            Debug.LogError("不是UI预制体!"); // MDEBUG:
            return;
        }
        var list = AssetDatabase.GetDependencies(assetPath, true);
        var textureList = new List<string>();
        for (int i = 0; i < list.Length; i++)
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(list[i]) is Texture && !list[i].StartsWith(EditorConfig.DYNAMIC_SPRITE_PATH))
            {
                textureList.Add(list[i]);
            }
        }

        if (textureList.Count == 0)
        {
            Debug.LogError("没有引用静态图集!"); // MDEBUG:
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

        StringBuilder sb = new StringBuilder();
        sb.Append("引用的图集：\n");
        for (int i = 0; i < atlasList.Count; i++)
        {
            sb.Append(atlasList[i]);
            sb.Append("\n");
        }
        Debug.Log(sb); // MDEBUG:
    }
}