#if UNITY_EDITOR && ILRuntime

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    private const string HotfixFilePath = "Assets/Res/Text/Hotfix.dll.bytes";

    [MenuItem("ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    private static void GenerateCLRBindingByAnalysis()
    {
        if (File.Exists(HotfixFilePath))
        {
            var path = "Assets/Scripts/Model/ILBinding";
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    var p = path.Substring(0, i + 1);
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                }
            }

            //GenerateCLRBinding(path);

            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (FileStream fs = new FileStream(HotfixFilePath, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                MGame.Model.ILHelper.InitILRuntime(domain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, path);
            }
            AssetDatabase.Refresh();

            Debug.Log("自动分析热更DLL生成CLR绑定成功！");
        }
        else
        {
            Debug.LogWarning("自动分析热更DLL生成CLR绑定失败！");
        }
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