using System.IO;
using System.Text.RegularExpressions;
using Model;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
    private const string CodeDir = "Assets/Res/Text/";
    private const string HotfixDll = "Unity.Hotfix.dll";
    private const string HotfixPdb = "Unity.Hotfix.pdb";

    static Startup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        FileHelper.CreateDir(CodeDir);

        if (File.Exists($"{ScriptAssembliesDir}/{HotfixDll}") && File.Exists($"{ScriptAssembliesDir}/{HotfixDll}"))
        {
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
            Debug.Log($"复制Hotfix.dll、Hotfix.pdb到Assets/Res/Text，成功！");

#if ILRuntime
            //AssetBundleBuildEditor.BuildAssetBundle();
#endif
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning($"复制Hotfix.dll、Hotfix.pdb到Assets/Res/Text，失败！");
        }
    }
}