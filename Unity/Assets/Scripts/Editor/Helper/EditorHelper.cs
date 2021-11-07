using UnityEngine;

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
            Debug.LogError("bat文件不存在：" + workingDir);
        }
        else
        {
            var path = EditorHelper.FormatPath(workingDir);
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
}