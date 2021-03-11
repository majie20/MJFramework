#if UNITY_EDITOR

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
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (FileStream fs = new FileStream(HotfixFilePath, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                InitILRuntime(domain);

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

    private static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        // 注册重定向函数

        // 注册委托

        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //domain.RegisterCrossBindingAdaptor(new TestClassBaseAdapter());
        //domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
    }
}

#endif