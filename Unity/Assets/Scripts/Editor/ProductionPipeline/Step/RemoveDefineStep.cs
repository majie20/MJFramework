using System.Linq;
using UnityEditor;

namespace M.ProductionPipeline
{
    public class RemoveDefineStep : IStep
    {
        public string Name;

        public void Run()
        {
            EditorHelper.RemoveDefineSymbols(Name, BuildTargetGroup.Standalone);
            EditorHelper.RemoveDefineSymbols(Name, BuildTargetGroup.iOS);
            EditorHelper.RemoveDefineSymbols(Name, BuildTargetGroup.Android);
            EditorHelper.RemoveDefineSymbols(Name, BuildTargetGroup.WebGL);
        }

        public string EnterText()
        {
            return $"移除 {Name} 宏定义开始！";
        }

        public string ExitText()
        {
            return $"移除 {Name} 宏定义结束！";
        }

        public bool IsTriggerCompile()
        {
            var defineTexts = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();

            return defineTexts.Contains(Name);
        }
    }
}