using System.IO;
using UnityEditor;
using UnityEngine;

public class UtilsEditor : EditorWindow
{
    [MenuItem("Tools/UtilsEditor/Excel导出(用于导出帧数表)")]
    public static void ExcelExportJson()
    {
        if (Directory.Exists("./Excel"))
        {
            foreach (var filePath in Directory.GetFiles("./Excel"))
            {
                ExcelExportJsonEditor.ExportJson(filePath);
            }
        }
        else
        {
            Debug.Log("无");
        }
    }

    [MenuItem("Tools/UtilsEditor/ModelEditorPanel")]
    public static void OpenModelUtilsPanel()
    {
        CreateInstance<ModelEditor>().Show();
    }

    [MenuItem("Tools/UtilsEditor/Build AssetBundle")]
    public static void CreateAssetBundle()
    {
        //string path = "./AssetBundleRes";
        string path = "./Assets/StreamingAssets/AssetBundleRes";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
        Debug.Log($"完成AssetBundle打包，文件夹{path}");
    }
}