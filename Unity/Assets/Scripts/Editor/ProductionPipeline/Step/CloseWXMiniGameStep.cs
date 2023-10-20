using UnityEditor;

namespace M.ProductionPipeline
{
    public class CloseWXMiniGameStep : IStep
    {
        public void Run()
        {
            if (System.IO.Directory.Exists(EditorConst.WX_WEBGL_TEMPLATES))
            {
                UnityEditor.FileUtil.CopyFileOrDirectory(EditorConst.WX_WEBGL_TEMPLATES, EditorConst.WX_WEBGL_TEMPLATES_);
                UnityEditor.FileUtil.DeleteFileOrDirectory($"{EditorConst.WX_WEBGL_TEMPLATES}.meta");
                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.WX_WEBGL_TEMPLATES);
            }

            if (System.IO.Directory.Exists(EditorConst.WX_WASM_SDK_V2))
            {
                UnityEditor.FileUtil.CopyFileOrDirectory(EditorConst.WX_WASM_SDK_V2, EditorConst.WX_WASM_SDK_V2_);
                UnityEditor.FileUtil.DeleteFileOrDirectory($"{EditorConst.WX_WASM_SDK_V2}.meta");
                UnityEditor.FileUtil.DeleteFileOrDirectory(EditorConst.WX_WASM_SDK_V2);
            }
        }

        public string EnterText()
        {
            return $"关闭微信小游戏 开始！";
        }

        public string ExitText()
        {
            return $"关闭微信小游戏 结束！";
        }

        public bool IsTriggerCompile()
        {
            return System.IO.Directory.Exists(EditorConst.WX_WEBGL_TEMPLATES);
        }
    }
}