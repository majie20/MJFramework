using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace M.ProductionPipeline
{
    public class EnableUnity_Logs_ViewerStep : IStep
    {
        public void Run()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(EditorConst.GAME_UNITY);
            EditorApplication.ExecuteMenuItem("Tools/Reporter/Create");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public string EnterText()
        {
            return $"激活Unity-Logs-Viewer 开始！";
        }

        public string ExitText()
        {
            return $"激活Unity-Logs-Viewer 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}