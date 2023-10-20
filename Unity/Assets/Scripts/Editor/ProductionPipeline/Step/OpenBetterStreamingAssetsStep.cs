using UnityEditor;

namespace M.ProductionPipeline
{
    public class OpenBetterStreamingAssetsStep : IStep
    {
        public void Run()
        {
            if (System.IO.Directory.Exists(EditorConst.BETTER_STREAMING_ASSETS_))
            {
                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.BETTER_STREAMING_ASSETS_, EditorConst.BETTER_STREAMING_ASSETS);
                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.BETTER_STREAMING_ASSETS_);
            }
        }

        public string EnterText()
        {
            return $"开启BetterStreamingAssets 开始！";
        }

        public string ExitText()
        {
            return $"开启BetterStreamingAssets 结束！";
        }

        public bool IsTriggerCompile()
        {
            return System.IO.Directory.Exists(EditorConst.BETTER_STREAMING_ASSETS_);
        }
    }
}