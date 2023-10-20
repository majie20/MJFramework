#if ILRuntime
using Model;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding : EditorWindow
{
    [MenuItem("Tools/ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    public static void GenerateClrBindingByAnalysis()
    {
        if (File.Exists(EditorConst.HotfixFilePath))
        {
            for (int i = 0; i < EditorConst.ILBINDING.Length; i++)
            {
                if (EditorConst.ILBINDING[i] == '/')
                {
                    var p = EditorConst.ILBINDING.Substring(0, i + 1);

                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                }
            }

            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();

            using (FileStream fs = new FileStream(EditorConst.HotfixFilePath, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                Model.ILHelper.InitILRuntime(domain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, EditorConst.ILBINDING);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var str = Encoding.UTF8.GetString(FileHelper.LoadFileByStream(EditorConst.IL_HELPER_CS));

            if (!Regex.IsMatch(str, @"ILRuntime\.Runtime\.Generated\.CLRBindings\.Initialize\(appdomain\);"))
            {
                str = Regex.Replace(str, @"appdomain.RegisterCLRMethodRedirection\(assert_22, Assert_22\);",
                    "appdomain.RegisterCLRMethodRedirection(assert_22, Assert_22);\n            \n            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);");
                FileHelper.SaveFileByStream(EditorConst.IL_HELPER_CS, Encoding.UTF8.GetBytes(str));

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log("自动分析热更DLL生成CLR绑定成功！");
        }
        else
        {
            Debug.LogWarning("自动分析热更DLL生成CLR绑定失败！");
        }
    }

    [MenuItem("Tools/ILRuntime/删除所有绑定代码")]
    public static void DeleteAllAndGenerateClrBindingByAnalysis()
    {
        if (Directory.Exists(EditorConst.ILBINDING))
        {
            string[] files = System.IO.Directory.GetFiles(EditorConst.ILBINDING);

            foreach (string s in files)
            {
                System.IO.File.Delete(s);
            }
        }

        var str = Encoding.UTF8.GetString(FileHelper.LoadFileByStream(EditorConst.IL_HELPER_CS));
        str = Regex.Replace(str, @"\n            \n            ILRuntime\.Runtime\.Generated\.CLRBindings\.Initialize\(appdomain\);", "");
        FileHelper.SaveFileByStream(EditorConst.IL_HELPER_CS, Encoding.UTF8.GetBytes(str));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif