using Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
//using FMODUnity;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public class EditorHelper
{
    public static System.Diagnostics.Process CreateShellExProcess(string cmd, string args, string workingDir = "")
    {
        var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = true;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;

        if (!string.IsNullOrEmpty(workingDir))
            pStartInfo.WorkingDirectory = workingDir;

        return System.Diagnostics.Process.Start(pStartInfo);
    }

    public static void RunBat(string batfile, string args, string workingDir = "")
    {
        var p = CreateShellExProcess(batfile, args, workingDir);
        p.Close();
    }

    public static void RunMyBat(string batFile, string workingDir)
    {
        if (!System.IO.Directory.Exists(workingDir))
        {
            Debug.LogError($"不存在的路径：{workingDir}");
        }
        else if (!System.IO.File.Exists($"{workingDir}{batFile}"))
        {
            Debug.LogError($"不存在的bat文件：{workingDir}{batFile}");
        }
        else
        {
            var path = FormatPath(workingDir);
            EditorHelper.RunBat(batFile, "", path);
        }
    }

    public static string FormatPath(string path)
    {
        path = path.Replace("/", "\\");

        if (Application.platform == RuntimePlatform.OSXEditor)
            path = path.Replace("\\", "/");

        return path;
    }

    public static void AddDefineSymbols(string str, BuildTargetGroup group)
    {
        var defineTexts = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();

        if (!defineTexts.Contains(str))
        {
            defineTexts.Add(str);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < defineTexts.Count; i++)
            {
                sb.Append(defineTexts[i]);

                if (i != defineTexts.Count - 1)
                {
                    sb.Append(";");
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, sb.ToString());
        }
    }

    public static void RemoveDefineSymbols(string str, BuildTargetGroup group)
    {
        var defineTexts = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();

        if (defineTexts.Contains(str))
        {
            defineTexts.Remove(str);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < defineTexts.Count; i++)
            {
                sb.Append(defineTexts[i]);

                if (i != defineTexts.Count - 1)
                {
                    sb.Append(";");
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, sb.ToString());
        }
    }

    public static void GetAssetPath(List<string> pathList, string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录

            for (int i = 0; i < fileInfo.Length; i++)
            {
                var info = fileInfo[i];

                //判断是否文件夹
                if (info is DirectoryInfo)
                {
                    GetAssetPath(pathList, info.FullName);
                }
                else
                {
                    if (!string.IsNullOrEmpty(info.Extension) && info.Extension != ".meta")
                    {
                        pathList.Add(FileHelper.AbsoluteSwitchRelativelyPath(info.FullName));
                    }
                }
            }
        }
        else
        {
            pathList.Add(FileHelper.AbsoluteSwitchRelativelyPath(path));
        }
    }

    public static string GetPackageName()
    {
        return AssetBundleCollectorSettingData.Setting.Packages[0].PackageName;
    }

    private static MethodInfo _clearConsoleMethod;

    public static void ClearConsole()
    {
        if (_clearConsoleMethod == null)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            _clearConsoleMethod = logEntries.GetMethod("Clear");
        }

        _clearConsoleMethod.Invoke(new object(), null);
    }

    /// <summary>
    /// 检测构建结果
    /// </summary>
    public static void CheckBuildMapContent(BuildMapContext buildMapContext)
    {
        List<string> infoList = new List<string>();

        foreach (var bundleInfo in buildMapContext.Collection)
        {
            // 注意：原生文件资源包只能包含一个原生文件
            bool isRawFile = bundleInfo.IsRawFile;

            if (isRawFile)
            {
                if (bundleInfo.AllMainAssets.Count != 1)
                    throw new System.Exception($"The bundle does not support multiple raw asset : {bundleInfo.BundleName}");

                continue;
            }

            // 注意：原生文件不能被其它资源文件依赖
            foreach (var assetInfo in bundleInfo.AllMainAssets)
            {
                if (assetInfo.AllDependAssetInfos != null)
                {
                    foreach (var dependAssetInfo in assetInfo.AllDependAssetInfos)
                    {
                        if (dependAssetInfo.IsRawAsset)
                            throw new System.Exception($"{assetInfo.AssetPath} can not depend raw asset : {dependAssetInfo.AssetPath}");

                        if (!dependAssetInfo.AssetPath.StartsWith(EditorConst.RES_PATH))
                        {
                            infoList.Add($"{assetInfo.AssetPath} : {dependAssetInfo.AssetPath}");
                        }
                    }
                }
            }
        }

        if (infoList.Count > 0)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("引用外部资源情况表.json"))
            {
                sw.WriteLine(CatJson.JsonParser.Default.ToJson(infoList));
            }

            UnityEngine.Debug.LogError($"请查看文件：引用外部资源情况表.json"); // MDEBUG:
        }
        else
        {
            UnityEngine.Debug.Log($"没有引用外部资源"); // MDEBUG:
        }
    }
}