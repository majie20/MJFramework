using Model;
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
            //AssetsConfigSettingsInspector.Export(UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsConfigSettings>("Assets/Scripts/Editor/AssetBundle/Config/AssetsConfig/AssetsConfigSettings.asset"));
            //AssetsConfigSettingsInspector.Export(UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsConfigSettings>("Assets/Scripts/Editor/AssetBundle/Config/AssetsConfig/NoBuildAssetsConfigSettings.asset"));
        }
        else
        {
            FileHelper.CreateDir(CodeDir);

            if (File.Exists($"{ScriptAssembliesDir}/{HotfixDll}") && File.Exists($"{ScriptAssembliesDir}/{HotfixDll}"))
            {
                File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
                File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
                Debug.Log($"复制Hotfix.dll、Hotfix.pdb到{CodeDir}，成功！");

                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning($"复制Hotfix.dll、Hotfix.pdb到{CodeDir}，失败！");
            }
        }
    }
}