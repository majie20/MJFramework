using UnityEditor;

namespace M.ProductionPipeline
{
    public class OpenHybridCLRStep : IStep
    {
        public void Run()
        {
            OtherEditor.SetHybridCLREnable(true);
            UnityEditor.PlayerSettings.gcIncremental = false;
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            UnityEditor.EditorUtility.RequestScriptReload();
        }

        public string EnterText()
        {
            return $"开启HybridCLR 开始！";
        }

        public string ExitText()
        {
            return $"开启HybridCLR 结束！";
        }

        public bool IsTriggerCompile()
        {
            return true;
        }
    }
}