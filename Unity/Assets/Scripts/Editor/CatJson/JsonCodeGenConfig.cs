using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace CatJson.Editor
{
    public static class JsonCodeGenConfig
    {
        public static string ParseJsonCodeDirPath = Application.dataPath + "/Scripts/Model/Generate/GenParseJsonCode";
        public static string ParseClassCodeTemplateFilePath = Application.dataPath + "/Scripts/Editor/CatJson/ParseClassCodeTemplate.txt";
        public static string ParseStructCodeTemplateFilePath = Application.dataPath + "/Scripts/Editor/CatJson/ParseStructCodeTemplate.txt";

        public static string ToJsonCodeDirPath = Application.dataPath + "/Scripts/Model/Generate/GenToJsonCode";
        public static string ToJsonCodeTemplateFilePaht = Application.dataPath + "/Scripts/Editor/CatJson/ToJsonCodeTemplate.txt";

        public static string StaticInitTemplateFilePath = Application.dataPath + "/Scripts/Editor/CatJson/StaticInitTemplate.txt";

        /// <summary>
        /// 需要生成解析/转换代码的json数据类的程序集名字
        /// </summary>
        public static string[] Assemblies =
        {
            "Unity.Model",
        };

        /// <summary>
        /// 使用JsonParser.Extension里的扩展而不生成解析/转换代码的类型，比如DateTime
        /// </summary>
        public static HashSet<Type> UseExtensionFuncTypes = new HashSet<Type>()
        {
            typeof(DateTime),
        };
    }

}
