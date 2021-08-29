#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeMenu
{
    [MenuItem("Tools/ILRuntime/安装VS调试插件")]
    private static void InstallDebugger()
    {
        EditorUtility.OpenWithDefaultApp("Assets/Samples/ILRuntime/1.6.6/Demo/Debugger~/ILRuntimeDebuggerLauncher.vsix");
    }

    [MenuItem("Tools/ILRuntime/打开ILRuntime中文文档")]
    private static void OpenDocumentation()
    {
        Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
    }

    [MenuItem("Tools/ILRuntime/打开ILRuntime Github项目")]
    private static void OpenGithub()
    {
        Application.OpenURL("https://github.com/Ourpalm/ILRuntime");
    }
}

#endif