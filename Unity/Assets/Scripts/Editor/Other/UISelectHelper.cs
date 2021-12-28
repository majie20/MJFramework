#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UISelectHelper{


    static List<RaycastResult> results = new List<RaycastResult>();

    [MenuItem("Tools/FastSelect #X")]
    static void Update()
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

#endif
