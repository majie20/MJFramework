using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuildEditor : EditorWindow
{
    [MenuItem("Tools/UtilsEditor/Build AssetBundle")]
    public static void BuildAssetBundle()
    {
        //string path = "./AssetBundleRes";
        string path = "./Assets/StreamingAssets/AssetBundleRes";
        if (Directory.Exists(path))
        {
            FileHelper.DelectDir(path);
        }
        else
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log($"完成AssetBundle打包，文件夹{path}");
    }
}