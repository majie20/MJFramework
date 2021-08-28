using System.IO;
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

        for (int i = 0; i < CodeDir.Length; i++)
        {
            if (CodeDir[i] == '/')
            {
                var p = CodeDir.Substring(0, i + 1);
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
        }

        if (File.Exists($"{ScriptAssembliesDir}/{HotfixDll}") && File.Exists($"{ScriptAssembliesDir}/{HotfixDll}"))
        {
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"),
                true);
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"),
                true);
            Debug.Log($"复制Hotfix.dll、Hotfix.pdb到Assets/Res/Text，成功！");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning($"复制Hotfix.dll、Hotfix.pdb到Assets/Res/Text，失败！");
        }
    }
}