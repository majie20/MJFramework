using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class OtherEditor
{
    [MenuItem("Assets/工具/复制资源路径", priority = 0)]
    private static void CopyAssetPath()
    {
        var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        //if (File.Exists(path))
        //{
        //    GUIUtility.systemCopyBuffer =
        //        Regex.Replace(
        //            FileHelper.AbsoluteSwitchRelativelyPath(path),
        //            FileValue.FILE_EXTENSION_PATTERN, "");
        //    return;
        //}
        GUIUtility.systemCopyBuffer = Model.FileHelper.AbsoluteSwitchRelativelyPath(path);
    }

    [MenuItem("Edit/HitUI #X")]
    private static void HitUI()
    {
        if (!EditorApplication.isPlaying) return;
        PointerEventData pData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pData, results);
        if (results.Count > 0)
        {
            UnityEditor.EditorGUIUtility.PingObject(results[0].gameObject);
        }
    }

    [MenuItem("Tools/打开Game场景")]
    private static void OpenGameScene()
    {
        if (EditorApplication.isPlaying) return;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Res/Scenes/Game.unity");
    }
}