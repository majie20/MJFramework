using System.Linq;
using UnityEditor;
using UnityEngine;

namespace M.ProductionPipeline
{
    public class AddDefineStep : IStep
    {
        public string Name;

        public void Run()
        {
            EditorHelper.AddDefineSymbols(Name, BuildTargetGroup.Standalone);
            EditorHelper.AddDefineSymbols(Name, BuildTargetGroup.iOS);
            EditorHelper.AddDefineSymbols(Name, BuildTargetGroup.Android);
            EditorHelper.AddDefineSymbols(Name, BuildTargetGroup.WebGL);
        }

        public string EnterText()
        {
            return $"添加 {Name} 宏定义开始！";
        }

        public string ExitText()
        {
            return $"添加 {Name} 宏定义结束！";
        }

        public bool IsTriggerCompile()
        {
            var defineTexts = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();

            return !defineTexts.Contains(Name);
        }
    }
}