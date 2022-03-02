using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectEditor
{
    private static List<RaycastResult> results = new List<RaycastResult>();

    [MenuItem("Tools/FastSelect #X")]
    private static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        PointerEventData pData = new PointerEventData(EventSystem.current);
        pData.position = Input.mousePosition;
        results.Clear();
        EventSystem.current.RaycastAll(pData, results);
        if (results.Count > 0)
        {
            UnityEditor.EditorGUIUtility.PingObject(results[0].gameObject);
        }
    }
}