using System.IO;
using UnityEditor;
using UnityEngine;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(AssetsBundleConfigSettings))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
//[CanEditMultipleObjects]
public class AssetsBundleConfigSettingsInspector : Editor
{
    private AssetsBundleConfigSettings m_target;

    /// <summary>
    /// 滑动条位置
    /// </summary>
    private Vector2 m_ScrollPosition;

    private void OnEnable()
    {
        this.m_target = (AssetsBundleConfigSettings)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        var fileDirABList = m_target.FileDirABList;
        if (GUILayout.Button("添加新的AB包配置"))
        {
            fileDirABList.Add(new FileDirABName());
        }

        GUILayout.Space(10);

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

        for (int i = 0; i < fileDirABList.Count; i++)
        {
            GUILayout.Label($"{i}-----------------------------------------------------");
            GUILayout.BeginHorizontal();
            GUILayout.Label("ABName:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
            fileDirABList[i].ABName = EditorGUILayout.TextField(fileDirABList[i].ABName);
            if (GUILayout.Button("删除配置"))
            {
                fileDirABList.RemoveAt(i);
                continue;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("扩展名:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
            fileDirABList[i].Extension = EditorGUILayout.TextField(fileDirABList[i].Extension);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加AB包地址"))
            {
                fileDirABList[i].DirList.Add(null);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            for (int j = 0; j < fileDirABList[i].DirList.Count; j++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("文件夹:", new GUIStyle { alignment = TextAnchor.LowerLeft, fixedWidth = 100 });
                Object dir = EditorGUILayout.ObjectField(fileDirABList[i].DirList[j], typeof(Object), false);

                if (Directory.Exists(AssetDatabase.GetAssetPath(dir)) || dir == null)
                {
                    fileDirABList[i].DirList[j] = dir;
                }

                if (GUILayout.Button("X"))
                {
                    fileDirABList[i].DirList.RemoveAt(i);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }
        GUILayout.Space(20);

        EditorUtility.SetDirty(target);
        if (GUILayout.Button("保存"))
        {
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndScrollView();  //结束 ScrollView 窗口
    }
}