using System.IO;

namespace M.ProductionPipeline
{
    public class ILRuntimeCLRClearStep : IStep
    {
        public void Run()
        {
#if ILRuntime
            ILRuntimeCLRBinding.DeleteAllAndGenerateClrBindingByAnalysis();
#endif
        }

        public string EnterText()
        {
            return $"清理ILRuntime CLR绑定 开始！";
        }

        public string ExitText()
        {
            return $"清理ILRuntime CLR绑定 结束！";
        }

        public bool IsTriggerCompile()
        {
            if (Directory.Exists(EditorConst.ILBINDING))
            {
                string[] files = System.IO.Directory.GetFiles(EditorConst.ILBINDING);

                return files.Length > 0;
            }

            return false;
        }
    }
}