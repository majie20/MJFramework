using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetCollectorEditor
{
    [MenuItem("Assets/资源收集/将收集到的数据导出", priority = 0)]
    private static void ExportAssetCollector()
    {
        ExportAssetCollector(Selection.activeObject);
    }

    public static void ExportAssetCollector(Object obj)
    {
        if (obj is AssetCollector collector)
        {
            AssetReferenceSettings settings = (AssetReferenceSettings)collector.target;
            if (settings == null)
            {
                Debug.LogError($"导出收集器：{AssetDatabase.GetAssetPath(settings)}的target为空！"); // MDEBUG:
                return;
            }
            settings.AssetPathList = new List<AssetReferenceSettings.Info>();

            List<string> pathList = new List<string>();
            for (int i = 0; i < collector.list.Count; i++)
            {
                if (collector.list[i].Obj == null)
                {
                    Debug.LogError($"配置第{i}行有空引用！"); // MDEBUG:
                    continue;
                }
                EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(collector.list[i].Obj));
                for (int j = 0; j < pathList.Count; j++)
                {
                    settings.AssetPathList.Add(new AssetReferenceSettings.Info()
                    {
                        typeName = AssetDatabase.LoadAssetAtPath<Object>(pathList[j]).GetType().FullName,
                        path = pathList[j],
                        type = collector.list[i].Type
                    });
                }
                pathList.Clear();
            }

            EditorUtility.SetDirty(settings);
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            Debug.Log($"导出收集器：{AssetDatabase.GetAssetPath(obj)}成功！"); // MDEBUG:
        }
        else
        {
            Debug.LogError($"导出收集器：{AssetDatabase.GetAssetPath(obj)}失败！"); // MDEBUG:
        }
    }
}