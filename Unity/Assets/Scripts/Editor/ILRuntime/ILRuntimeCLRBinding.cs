using System.IO;
using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding : EditorWindow
{
    private const string HotfixFilePath = "Assets/Res/Text/Hotfix.dll.bytes";
    private const string GenerateFilePath = "Assets/Scripts/Model/Generate/ILBinding";

    [MenuItem("Tools/ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    public static void GenerateCLRBindingByAnalysis()
    {
        if (File.Exists(HotfixFilePath))
        {
            EditorHelper.AddDefineSymbols("ILRuntime", BuildTargetGroup.Standalone);
            EditorHelper.AddDefineSymbols("ILRuntime", BuildTargetGroup.iOS);
            EditorHelper.AddDefineSymbols("ILRuntime", BuildTargetGroup.Android);

            for (int i = 0; i < GenerateFilePath.Length; i++)
            {
                if (GenerateFilePath[i] == '/')
                {
                    var p = GenerateFilePath.Substring(0, i + 1);
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                }
            }

            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (FileStream fs = new FileStream(HotfixFilePath, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                Model.ILHelper.InitILRuntime(domain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, GenerateFilePath);
            }

            AssetDatabase.Refresh();
            Debug.Log("自动分析热更DLL生成CLR绑定成功！");
        }
        else
        {
            Debug.LogWarning("自动分析热更DLL生成CLR绑定失败！");
        }
    }

    [MenuItem("Tools/ILRuntime/删除所有绑定代码")]
    public static void DeleteAllAndGenerateCLRBindingByAnalysis()
    {
        EditorHelper.RemoveDefineSymbols("ILRuntime", BuildTargetGroup.Standalone);
        EditorHelper.RemoveDefineSymbols("ILRuntime", BuildTargetGroup.iOS);
        EditorHelper.RemoveDefineSymbols("ILRuntime", BuildTargetGroup.Android);

        if (Directory.Exists(GenerateFilePath))
        {
            string[] files = System.IO.Directory.GetFiles(GenerateFilePath);

            foreach (string s in files)
            {
                System.IO.File.Delete(s);
            }
        }

        AssetDatabase.Refresh();
    }
}