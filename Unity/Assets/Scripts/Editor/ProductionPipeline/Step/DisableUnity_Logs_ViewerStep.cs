using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace M.ProductionPipeline
{
    public class DisableUnity_Logs_ViewerStep : IStep
    {
        public void Run()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(EditorConst.GAME_UNITY);
            UnityEngine.Object.DestroyImmediate(GameObject.Find("Reporter"));
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public string EnterText()
        {
            return $"禁用Unity-Logs-Viewer 开始！";
        }

        public string ExitText()
        {
            return $"禁用Unity-Logs-Viewer 结束！";
        }

        public bool IsTriggerCompile()
        {
            return false;
        }
    }
}