using UnityEditor;

namespace M.ProductionPipeline
{
    public class CloseHybridCLRStep : IStep
    {
        public void Run()
        {
            OtherEditor.SetHybridCLREnable(false);
            UnityEditor.PlayerSettings.gcIncremental = true;
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            UnityEditor.EditorUtility.RequestScriptReload();
        }

        public string EnterText()
        {
            return $"关闭HybridCLR 开始！";
        }

        public string ExitText()
        {
            return $"关闭HybridCLR 结束！";
        }

        public bool IsTriggerCompile()
        {
            return true;
        }
    }
}