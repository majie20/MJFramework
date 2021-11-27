using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuildEditor : EditorWindow
{
    private const string AB_PATH = "./Assets/StreamingAssets/AssetBundleRes";

    //private const string AB_PATH = "./AssetBundleRes/AssetBundleRes";
    private const string AB_ZIP_PATH = "./AssetBundleRes/Output";

    private const string ZIP_NAME = "AssetBundleRes";
    private const string ZIP_PASSWORD = "majie";

    [MenuItem("Tools/AssetBundle/Build AssetBundle")]
    public static void BuildAssetBundle()
    {
        if (Directory.Exists(AB_PATH))
        {
            Model.FileHelper.DelectDir(AB_PATH);
        }
        else
        {
            Directory.CreateDirectory(AB_PATH);
        }

        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(AB_PATH, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        Debug.Log($"完成AssetBundle打包，文件夹{AB_PATH}");
    }

    [MenuItem("Tools/AssetBundle/Zip AssetBundle")]
    public static void ZipAssetBundle()
    {
        if (Directory.Exists(AB_ZIP_PATH))
        {
            Model.FileHelper.DelectDir(AB_ZIP_PATH);
        }
        else
        {
            Directory.CreateDirectory(AB_ZIP_PATH);
        }

        ZipWrapper.Zip(new[] { AB_PATH }, $"{AB_ZIP_PATH}/{ZIP_NAME}.ma", ZIP_PASSWORD);

        Debug.Log($"完成AssetBundle包的压缩，文件{AB_ZIP_PATH}/{ZIP_NAME}.ma");

        EditorUtility.RevealInFinder($"{AB_ZIP_PATH}");
    }

    [MenuItem("Tools/AssetBundle/Build And Zip AssetBundle")]
    public static void BuildAndZipAssetBundle()
    {
        BuildAssetBundle();
        ZipAssetBundle();
    }
}