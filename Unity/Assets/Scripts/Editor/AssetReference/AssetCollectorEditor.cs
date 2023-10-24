using Model;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetCollectorEditor
{
    [MenuItem("Assets/资源收集/将收集到的数据导出", priority = 0)]
    private static void ExportAssetCollector()
    {
        for (int i = Selection.assetGUIDs.Length - 1; i >= 0; i--)
        {
            ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i])));
        }
    }

    [MenuItem("Tools/资源收集/导出所有资源收集器", priority = 1)]
    private static void ExportAllAssetCollector()
    {
        var step = new M.ProductionPipeline.ExportAllAssetCollectorStep();
        Debug.Log(step.EnterText()); // MDEBUG:
        step.Run();
        Debug.Log(step.ExitText()); // MDEBUG:
    }

    /// <summary>
    /// 忽略的文件类型
    /// </summary>
    public static readonly List<string> IgnoreFileExtensions = new()
    {
        //"",
        //".so",
        //".dll",
        //".cs",
        //".js",
        //".boo",
        //".meta",
        ".cginc"
    };

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

            settings.InstantlyAssetDataList = new List<AssetReferenceSettings.Info>();
            settings.RearAssetDataList = new List<AssetReferenceSettings.Info>();

            try
            {
                Add(collector.list1, settings.InstantlyAssetDataList);
                Add(collector.list2, settings.RearAssetDataList, false);
            }

            catch (NullReferenceException)
            {
                throw new NullReferenceException($"配置文件：{collector.name}有空引用！");
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

    private static string GetTypeName(Object o)
    {
        if (o is Texture2D)
        {
            var source = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(o)) as TextureImporter;

            if (source.textureType == TextureImporterType.Sprite)
            {
                return typeof(Sprite).FullName;
            }
        }

        return o.GetType().FullName;
    }

    private static void Add(List<AssetCollector.CollectorInfo1> list, List<AssetReferenceSettings.Info> assetPathList, bool isSetObj = true)
    {
        List<string> pathList = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Obj == null)
            {
                throw new NullReferenceException();
            }

            EditorHelper.GetAssetPath(pathList, AssetDatabase.GetAssetPath(list[i].Obj));

            for (int j = 0; j < pathList.Count; j++)
            {
                var fileExtension = Path.GetExtension(pathList[j]);

                if (IgnoreFileExtensions.Contains(fileExtension) || Path.GetFileNameWithoutExtension(pathList[j]).StartsWith("~"))
                {
                    continue;
                }

                assetPathList.Add(new AssetReferenceSettings.Info(GetTypeName(AssetDatabase.LoadAssetAtPath<Object>(pathList[j])),
                    isSetObj ? AssetDatabase.LoadAssetAtPath<Object>(pathList[j]) : null, pathList[j], list[i].Type));
            }

            pathList.Clear();
        }
    }
}