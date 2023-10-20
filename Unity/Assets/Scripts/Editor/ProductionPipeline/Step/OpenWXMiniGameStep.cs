using UnityEditor;

namespace M.ProductionPipeline
{
    public class OpenWXMiniGameStep : IStep
    {
        public void Run()
        {
            if (System.IO.Directory.Exists(EditorConst.WX_WEBGL_TEMPLATES_))
            {
                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.WX_WEBGL_TEMPLATES_, EditorConst.WX_WEBGL_TEMPLATES);
                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.WX_WEBGL_TEMPLATES_);
            }

            if (System.IO.Directory.Exists(EditorConst.WX_WASM_SDK_V2_))
            {
                UnityEditor.FileUtil.ReplaceDirectory(EditorConst.WX_WASM_SDK_V2_, EditorConst.WX_WASM_SDK_V2);
                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.WX_WASM_SDK_V2_);
            }
        }

        public string EnterText()
        {
            return $"开启微信小游戏 开始！";
        }

        public string ExitText()
        {
            return $"开启微信小游戏 结束！";
        }

        public bool IsTriggerCompile()
        {
            return System.IO.Directory.Exists(EditorConst.WX_WEBGL_TEMPLATES_);
        }
    }
}