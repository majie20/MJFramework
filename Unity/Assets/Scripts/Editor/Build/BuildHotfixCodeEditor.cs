using Model;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    private static string ScriptAssembliesDir = "Library/ScriptAssemblies";
    private const  string HotfixDll           = "Unity.Hotfix.dll";
    private const  string HotfixPdb           = "Unity.Hotfix.pdb";

    static Startup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            AssetCollectorEditor.ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(EditorConst.GAME_LOAD_COMPLETE_AC));
            AssetCollectorEditor.ExportAssetCollector(AssetDatabase.LoadAssetAtPath<Object>(EditorConst.INIT_AC));
        }
        else
        {
            FileHelper.CreateDir(EditorConst.CODE_DIR_PATH);

            if (File.Exists($"{ScriptAssembliesDir}/{HotfixDll}") && File.Exists($"{ScriptAssembliesDir}/{HotfixDll}"))
            {
                File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(EditorConst.CODE_DIR_PATH, "Hotfix.dll.bytes"), true);
                File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(EditorConst.CODE_DIR_PATH, "Hotfix.pdb.bytes"), true);
                Debug.Log($"复制Hotfix.dll、Hotfix.pdb到{EditorConst.CODE_DIR_PATH}，成功！");
            }
            else
            {
                Debug.LogWarning($"复制Hotfix.dll、Hotfix.pdb到{EditorConst.CODE_DIR_PATH}，失败！");
            }
        }
    }
}