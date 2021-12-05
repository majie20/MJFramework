using UnityEditor;
using UnityEngine;

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
        GUILayout.BeginHorizontal();
        GUILayout.Label("AB包保存地址:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
        m_target.AssetBundleSavePath = EditorGUILayout.TextField(m_target.AssetBundleSavePath);
        if (GUILayout.Button("选择"))
        {
            m_target.AssetBundleSavePath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("压缩的AB包保存地址:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 150 });
        m_target.ZipAssetBundleSavePath = EditorGUILayout.TextField(m_target.ZipAssetBundleSavePath);
        if (GUILayout.Button("选择"))
        {
            m_target.ZipAssetBundleSavePath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("加密密码:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
        m_target.EncryptPassword = EditorGUILayout.TextField(m_target.EncryptPassword);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("ZIP密码:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
        m_target.ZipPassword = EditorGUILayout.TextField(m_target.ZipPassword);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("AB扩展名:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
        m_target.ABExtension = EditorGUILayout.TextField(m_target.ABExtension);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("ZIP扩展名:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
        m_target.ZipExtension = EditorGUILayout.TextField(m_target.ZipExtension);
        GUILayout.EndHorizontal();

        EditorUtility.SetDirty(target);
        if (GUILayout.Button("保存"))
        {
            AssetDatabase.SaveAssets();
        }
    }
}