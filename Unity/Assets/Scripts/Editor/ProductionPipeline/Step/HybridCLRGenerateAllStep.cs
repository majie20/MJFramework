using UnityEditor;
using UnityEditor.Build;

namespace M.ProductionPipeline
{
    public class HybridCLRGenerateAllStep : IStep
    {
        public void Run()
        {
            //Il2CppCodeGeneration.OptimizeSize
        }

        public string EnterText()
        {
            return $"执行HybridCLR 开始！";
        }

        public string ExitText()
        {
            return $"设置所有资源格式 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}