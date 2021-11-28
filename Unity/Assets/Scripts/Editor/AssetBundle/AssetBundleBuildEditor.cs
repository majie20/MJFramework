using System;
using System.Collections.Generic;
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

        AssetsBundleSettings settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>("Assets/Resources/AssetsBundleSettings.asset");
        List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

        for (int i = 0; i < settings.fileDirABList.Count; i++)
        {
            var config = settings.fileDirABList[i];
            var pathList = new List<string>();
            Add(pathList, AssetDatabase.GetAssetPath(config.Dir), config);

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = config.ABName;
            build.assetNames = pathList.ToArray();
            buildList.Add(build);
        }

        BuildPipeline.BuildAssetBundles(AB_PATH, buildList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        Debug.Log($"完成AssetBundle打包，文件夹{AB_PATH}");
    }

    private static void Add(List<string> pathList, string path, FileDirABName config)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo info in fileInfo)
        {
            //判断是否文件夹
            if (info is DirectoryInfo)
            {
                Add(pathList, info.FullName, config);
            }
            else
            {
                if (info.Extension != ".meta")
                {
                    if (string.IsNullOrEmpty(config.Extension) || config.Extension.Contains(info.Extension))
                    {
                        string mp = info.FullName;
                        mp = mp.Substring(mp.IndexOf("Assets", StringComparison.Ordinal));
                        mp = mp.Replace('\\', '/');
                        pathList.Add(mp);
                    }
                }
            }
        }
    }

    [MenuItem("Tools/AssetBundle/Zip AssetBundle")]
    public static void ZipAssetBundle()
    {
        //if (Directory.Exists(AB_ZIP_PATH))
        //{
        //    Model.FileHelper.DelectDir(AB_ZIP_PATH);
        //}
        //else
        //{
        //    Directory.CreateDirectory(AB_ZIP_PATH);
        //}

        //ZipWrapper.Zip(new[] { AB_PATH }, $"{AB_ZIP_PATH}/{ZIP_NAME}.ma", ZIP_PASSWORD);

        //Debug.Log($"完成AssetBundle包的压缩，文件{AB_ZIP_PATH}/{ZIP_NAME}.ma");

        //EditorUtility.RevealInFinder($"{AB_ZIP_PATH}");
    }

    [MenuItem("Tools/AssetBundle/Build And Zip AssetBundle")]
    public static void BuildAndZipAssetBundle()
    {
        BuildAssetBundle();
        ZipAssetBundle();
    }
}