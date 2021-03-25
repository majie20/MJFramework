using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleTest
{
    [MenuItem("Tools/Build AssetBundle")]
    public static void CreateAssetBundle()
    {
        string path = "./AssetBundleRes";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Debug.Log($"完成AssetBundle打包，文件夹{path}");
    }
}