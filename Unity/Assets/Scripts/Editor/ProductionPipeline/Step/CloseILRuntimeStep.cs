using UnityEditor;

namespace M.ProductionPipeline
{
    public class CloseILRuntimeStep : IStep
    {
        public void Run()
        {
            if (System.IO.Directory.Exists(EditorConst.THIRDPARTY_ILRUNTIME))
            {
                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.THIRDPARTY_ILRUNTIME, EditorConst.THIRDPARTY_ILRUNTIME_);
                UnityEditor.AssetDatabase.DeleteAsset(EditorConst.THIRDPARTY_ILRUNTIME);
            }

            if (System.IO.Directory.Exists(EditorConst.PLUGINS_ILRUNTIME))
            {
                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.PLUGINS_ILRUNTIME, EditorConst.PLUGINS_ILRUNTIME_);
                UnityEditor.AssetDatabase.DeleteAsset(EditorConst.PLUGINS_ILRUNTIME);
            }
        }

        public string EnterText()
        {
            return $"关闭ILRuntime 开始！";
        }

        public string ExitText()
        {
            return $"关闭ILRuntime 结束！";
        }

        public bool IsTriggerCompile()
        {
            return System.IO.Directory.Exists(EditorConst.THIRDPARTY_ILRUNTIME);
        }
    }
}