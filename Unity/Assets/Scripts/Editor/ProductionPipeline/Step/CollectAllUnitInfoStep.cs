using UnityEditor;

namespace M.ProductionPipeline
{
    public class CollectAllUnitInfoStep : IStep
    {
        public void Run()
        {
            UnitInfoCollectorEditor.Run();
        }

        public string EnterText()
        {
            return $"收集所有Unit信息 开始！";
        }

        public string ExitText()
        {
            return $"收集所有Unit信息 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}