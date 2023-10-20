using UnityEditor;

namespace M.ProductionPipeline
{
    public class SetAllResFormatStep : IStep
    {
        public void Run()
        {
            ImportSettingsEditor.SetAllRes();
        }

        public string EnterText()
        {
            return $"设置所有资源格式 开始！";
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