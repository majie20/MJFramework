#if UNITY_EDITOR && ILRuntime

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding : EditorWindow
{
    private const string HotfixFilePath = "Assets/Res/Text/Hotfix.dll.bytes";
    private const string GenerateFilePath = "Assets/Scripts/Model/ILBinding";

    [MenuItem("Tools/ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    public static void GenerateCLRBindingByAnalysis()
    {
        if (File.Exists(HotfixFilePath))
        {
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

            //GenerateCLRBinding(GenerateFilePath);

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

    [MenuItem("Tools/ILRuntime/删除所以绑定代码，通过自动分析热更DLL生成CLR绑定")]
    public static void DeleteAllAndGenerateCLRBindingByAnalysis()
    {
        if (Directory.Exists(GenerateFilePath))
        {
            string[] files = System.IO.Directory.GetFiles(GenerateFilePath);

            foreach (string s in files)
            {
                System.IO.File.Delete(s);
            }
        }

        AssetDatabase.Refresh();

        GenerateCLRBindingByAnalysis();
    }

    /// <summary>
    /// CLR绑定
    /// </summary>
    private static void GenerateCLRBinding(string path)
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(long));
        types.Add(typeof(object));
        types.Add(typeof(string));
        types.Add(typeof(Array));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(RectTransform));
        types.Add(typeof(Time));
        types.Add(typeof(Debug));
        //所有DLL内的类型的真实C#类型都是ILTypeInstance
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, path);
        AssetDatabase.Refresh();
    }
}

#endif