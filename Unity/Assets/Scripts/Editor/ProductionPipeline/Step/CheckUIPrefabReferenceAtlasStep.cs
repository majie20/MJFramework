using UnityEditor;

namespace M.ProductionPipeline
{
    public class CheckUIPrefabReferenceAtlasStep : IStep
    {
        public void Run()
        {
            AssetReferenceEditor.CheckUIPrefabReferenceAtlasAll();
        }

        public string EnterText()
        {
            return $"检查所有UI预制体引用图集情况 开始！";
        }

        public string ExitText()
        {
            return $"检查所有UI预制体引用图集情况 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}