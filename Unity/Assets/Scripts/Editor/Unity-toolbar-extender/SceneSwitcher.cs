using M.ProductionPipeline;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Compile", "编译Unity代码（F4）")))
            {
                Compile();
            }

            if (GUILayout.Button(new GUIContent("Refresh", "刷新Unity（F6）")))
            {
                Refresh();
            }

            if (GUILayout.Button(new GUIContent("ClearStep", "清理生产管线（F7）")))
            {
                ClearStep();
            }
        }

        private static void Compile()
        {
            //ScriptCompileReloadTools.ManualReload();
            UnityEditor.EditorApplication.UnlockReloadAssemblies();
            UnityEditor.EditorUtility.RequestScriptReload();
            Debug.LogError("-----------------编译Unity代码完成！-----------------"); // MDEBUG:
        }

        [MenuItem("Edit/Refresh _F6")]
        private static void Refresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.LogError("-----------------刷新Unity完成！-----------------"); // MDEBUG:
        }

        [MenuItem("Edit/ClearStep _F7")]
        private static void ClearStep()
        {
            StepSaveSettings.Clear();
            Debug.LogError("-----------------清理生产管线！-----------------"); // MDEBUG:
        }
    }
}