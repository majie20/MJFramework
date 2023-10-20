using Model;
using UnityEditor;

namespace M.ProductionPipeline
{
    public class GenCodeJsonStep : IStep
    {
        public void Run()
        {
            FileHelper.DelectDir(EditorConst.JSON_CONFIG);
            EditorHelper.RunMyBat("gen_code_json一键导出.bat", "../Excel/");
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            UnityEditor.EditorUtility.RequestScriptReload();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public string EnterText()
        {
            return $"导出Excel表，普通格式 开始！";
        }

        public string ExitText()
        {
            return $"导出Excel表，普通格式 结束！";
        }

        public bool IsTriggerCompile()
        {
            return true;
        }
    }
}