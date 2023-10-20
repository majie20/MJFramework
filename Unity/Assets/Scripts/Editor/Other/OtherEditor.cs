using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class OtherEditor
{
    //[MenuItem("Assets/工具/复制资源路径", priority = 1)]
    //private static void CopyAssetPath()
    //{
    //    var path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //    //if (File.Exists(path))
    //    //{
    //    //    GUIUtility.systemCopyBuffer =
    //    //        Regex.Replace(
    //    //            FileHelper.AbsoluteSwitchRelativelyPath(path),
    //    //            FileValue.FILE_EXTENSION_PATTERN, "");
    //    //    return;
    //    //}
    //    GUIUtility.systemCopyBuffer = Model.FileHelper.AbsoluteSwitchRelativelyPath(path);
    //}

    [MenuItem("Assets/工具/打印动画时长", priority = 2)]
    private static void ShowAnimationClipLength()
    {
        if (Selection.activeObject is AnimationClip clip)
        {
            Debug.Log($"动画（{clip.name}）的时长是：{clip.length}"); // MDEBUG:
        }
    }

    [MenuItem("Edit/HitUI #X")]
    private static void HitUI()
    {
        if (!EditorApplication.isPlaying) return;
        PointerEventData pData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pData.position = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
        var results = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pData, results);

        if (results.Count > 0)
        {
            UnityEditor.EditorGUIUtility.PingObject(results[0].gameObject);
        }
    }

    [MenuItem("Tools/打开Game场景 _F1", priority = 1)]
    private static void OpenGameScene()
    {
        if (EditorApplication.isPlaying) return;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(EditorConst.GAME_UNITY);
    }

    [MenuItem("Tools/启动Game _F5", priority = 4)]
    public static void Play()
    {
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    public static void SetHybridCLREnable(bool enable)
    {
        HybridCLR.Editor.HybridCLRSettings.Instance.enable = enable;
    }
}