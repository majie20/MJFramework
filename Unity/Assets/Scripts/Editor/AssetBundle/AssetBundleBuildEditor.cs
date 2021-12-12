using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuildEditor : EditorWindow
{
    private static string AssetsBundleConfigSettingsPath = "Assets/Scripts/Editor/AssetBundle/Config/AssetsBundleConfigSettings.asset";
    private static string AssetsBundleSettingsPath = "Assets/Resources/AssetsBundleSettings.asset";

    [MenuItem("Tools/AssetBundle/Build AssetBundle")]
    public static void BuildAssetBundle()
    {
        AssetsBundleConfigSettings cSettings = AssetDatabase.LoadAssetAtPath<AssetsBundleConfigSettings>(AssetsBundleConfigSettingsPath);
        AssetsBundleSettings settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(AssetsBundleSettingsPath);
        if (Directory.Exists(settings.AssetBundleSavePath))
        {
            Model.FileHelper.DelectDir(settings.AssetBundleSavePath);
        }
        else
        {
            Directory.CreateDirectory(settings.AssetBundleSavePath);
        }

        AssetDatabase.Refresh();

        List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

        for (int i = 0; i < cSettings.FileDirABList.Count; i++)
        {
            var config = cSettings.FileDirABList[i];
            var pathList = new List<string>();
            for (int j = 0; j < config.DirList.Count; j++)
            {
                AddResourcePath(pathList, AssetDatabase.GetAssetPath(config.DirList[j]), config);
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = $"{config.ABName}{settings.ABExtension}";
            build.assetNames = pathList.ToArray();
            buildList.Add(build);
        }

        BuildPipeline.BuildAssetBundles(settings.AssetBundleSavePath, buildList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        Encrypt(settings);
        AssetDatabase.Refresh();

        Debug.Log($"完成AssetBundle打包，文件夹{settings.AssetBundleSavePath}");
    }

    private static void Encrypt(AssetsBundleSettings settings)
    {
        List<ABConfig> configs = new List<ABConfig>();
        MD5 md5 = MD5.Create();
        DirectoryInfo dir = new DirectoryInfo(settings.AssetBundleSavePath);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        for (int i = 0; i < fileInfo.Length; i++)
        {
            var info = fileInfo[i];
            if (!(info is DirectoryInfo) && info.Extension == settings.ABExtension)
            {
                byte[] buffer;
                using (FileStream stream = File.OpenRead(info.FullName))
                {
                    buffer = new byte[stream.Length];

                    stream.Read(buffer, 0, buffer.Length);
                }
                using (FileStream stream = File.Create(info.FullName))
                {
                    buffer = AESHelper.Encrypt(buffer, settings.EncryptPassword);

                    configs.Add(new ABConfig { ABName = $"{Path.GetFileNameWithoutExtension(info.FullName)}{settings.ZipExtension}", Size = buffer.Length, CRC = Convert.ToBase64String(md5.ComputeHash(buffer)) });

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        StringBuilder builder = new StringBuilder();
        builder.Append("[");
        for (int i = 0; i < configs.Count; i++)
        {
            builder.Append(JsonConvert.SerializeObject(configs[i]));
            if (i < configs.Count - 1)
            {
                builder.Append(",");
            }
        }
        builder.Append("]");

        FileHelper.SaveFileByStream($"{settings.AssetBundleSavePath}/{HotConfig.AB_CONFIG_NAME}.json", Encoding.UTF8.GetBytes(builder.ToString()));
    }

    private static void AddResourcePath(List<string> pathList, string path, FileDirABName config)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo info in fileInfo)
        {
            //判断是否文件夹
            if (info is DirectoryInfo)
            {
                AddResourcePath(pathList, info.FullName, config);
            }
            else
            {
                if (info.Extension != ".meta")
                {
                    if (string.IsNullOrEmpty(config.Extension) || config.Extension.Contains(info.Extension))
                    {
                        pathList.Add(FileHelper.AbsoluteSwitchRelativelyPath(info.FullName));
                    }
                }
            }
        }
    }

    [MenuItem("Tools/AssetBundle/Zip AssetBundle")]
    public static void ZipAssetBundle()
    {
        AssetsBundleSettings settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(AssetsBundleSettingsPath);

        if (Directory.Exists(settings.ZipAssetBundleSavePath))
        {
            Model.FileHelper.DelectDir(settings.ZipAssetBundleSavePath);
        }
        else
        {
            Directory.CreateDirectory(settings.ZipAssetBundleSavePath);
        }

        DirectoryInfo dir = new DirectoryInfo(settings.AssetBundleSavePath);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo info in fileInfo)
        {
            if (!(info is DirectoryInfo) && info.Extension == settings.ABExtension)
            {
                ZipWrapper.Zip(new[] { info.FullName }, $"{settings.ZipAssetBundleSavePath}/{Path.GetFileNameWithoutExtension(info.FullName)}{settings.ZipExtension}", settings.ZipPassword);
                Debug.Log($"完成AssetBundle包的压缩，文件{settings.ZipAssetBundleSavePath}/{Path.GetFileNameWithoutExtension(info.FullName)}{settings.ZipExtension}");
            }
        }

        FileHelper.SaveFileByStream($"{settings.ZipAssetBundleSavePath}/{HotConfig.AB_CONFIG_NAME}.json", FileHelper.LoadFileByStream($"{settings.AssetBundleSavePath}/{HotConfig.AB_CONFIG_NAME}.json"));

        //ZipWrapper.Zip(new[] { $"{settings.AssetBundleSavePath}/{HotConfig.AB_CONFIG_NAME}.json" }, $"{settings.ZipAssetBundleSavePath}/{HotConfig.AB_CONFIG_NAME}{settings.ZipExtension}", settings.ZipPassword);

        EditorUtility.RevealInFinder($"{settings.ZipAssetBundleSavePath}");
    }

    [MenuItem("Tools/AssetBundle/Build And Zip AssetBundle")]
    public static void BuildAndZipAssetBundle()
    {
        BuildAssetBundle();
        ZipAssetBundle();
    }
}