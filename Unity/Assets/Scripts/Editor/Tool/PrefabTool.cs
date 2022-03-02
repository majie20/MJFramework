using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 动态修改预制体工具
/// </summary>
public class PrefabTool
{
    /// <summary>
    /// 用来复制的物体
    /// </summary>
    private static GameObject obj = null;
    /// <summary>
    /// 获取组件
    /// </summary>
    static Component[] copiedComponents;

    [MenuItem("GameObject/预制体保存/复制", priority = 0)]
    public static void Copy()
    {
        obj = Selection.activeGameObject;
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("Assets/预制体保存/粘贴", priority = 0)]
    public static void Paste()
    {
        if (obj)
        {
            PrefabID PrefabID1 = obj.GetComponent<PrefabID>();
            PrefabID PrefabID2 = Selection.activeGameObject.GetComponent<PrefabID>();
            if (PrefabID1&& PrefabID2&& PrefabID1.ID== PrefabID2.ID)
            {
                //Selection.activeGameObject.transform.position = obj.transform.position;
                //Selection.activeGameObject.transform.rotation = obj.transform.rotation;
                //Selection.activeGameObject.transform.localScale = obj.transform.localScale;

                foreach (var targetGameObject in Selection.gameObjects)
                {
                    if (!targetGameObject || copiedComponents == null) continue;

                    if (obj.GetComponent<RectTransform>() != null)
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(obj.GetComponent<RectTransform>());
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(targetGameObject.GetComponent<RectTransform>());
                    }
                    if (obj.GetComponent<Text>() != null)
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(obj.GetComponent<Text>());
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(targetGameObject.GetComponent<Text>());
                    }

                    //foreach (var copiedComponent in copiedComponents)
                    //{
                    //    if (!copiedComponent) continue;

                    //    UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                    //    UnityEditorInternal.ComponentUtility.PasteComponentValues(targetGameObject.GetComponent(copiedComponent.GetType()));
                    //}
                }

                obj = null;
            }
            else
            {
                return;
            }
        }        
    }
}
