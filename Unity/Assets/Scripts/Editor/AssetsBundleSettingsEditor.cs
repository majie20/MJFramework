using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(AssetsBundleSettings))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
[CanEditMultipleObjects]
public class AssetsBundleSettingsEditor : Editor
{
    private AssetsBundleSettings m_target;

    /// <summary>
    /// 滑动条位置
    /// </summary>
    private Vector2 m_ScrollPosition;

    private void OnEnable()
    {
        this.m_target = (AssetsBundleSettings)target;
    }

    public override void OnInspectorGUI()
    {
        var fileDirABList = m_target.fileDirABList;

        if (GUILayout.Button("添加"))
        {
            fileDirABList.Add(new FileDirABName());
        }

        GUILayout.Space(10);

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

        for (int i = 0; i < fileDirABList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ABName:", new GUIStyle { alignment = TextAnchor.LowerLeft, fixedWidth = 100 });
            fileDirABList[i].ABName = EditorGUILayout.TextField(fileDirABList[i].ABName);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("扩展名:", new GUIStyle { alignment = TextAnchor.LowerLeft, fixedWidth = 100 });
            fileDirABList[i].Extension = EditorGUILayout.TextField(fileDirABList[i].Extension);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("文件夹:", new GUIStyle { alignment = TextAnchor.LowerLeft, fixedWidth = 100 });
            Object dir = EditorGUILayout.ObjectField(fileDirABList[i].Dir, typeof(Object), false);

            if (Directory.Exists(AssetDatabase.GetAssetPath(dir)) || dir == null)
            {
                fileDirABList[i].Dir = dir;
            }

            if (GUILayout.Button("X"))
            {
                //将元素添加进删除list
                fileDirABList.RemoveAt(i);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        EditorGUILayout.EndScrollView();  //结束 ScrollView 窗口

        EditorUtility.SetDirty(target);
    }
}