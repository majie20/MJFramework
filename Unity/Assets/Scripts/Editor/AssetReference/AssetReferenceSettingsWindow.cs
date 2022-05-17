using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetReferenceSettingsWindow : EditorWindow
{
    [MenuItem("Tools/资源收集/资源收集编辑器", false, 102)]
    public static void ShowWindow()
    {
        AssetReferenceSettingsWindow window = GetWindow<AssetReferenceSettingsWindow>("资源收集编辑器", true);
        window.minSize = new Vector2(800, 600);
    }

    private AssetReferenceSettings _settings;

    [SerializeField] //必须要加
    protected List<Object> _assetLst;

    private void OnGUI()
    {
        var settings = EditorGUILayout.ObjectField(new GUIContent("Target："), _settings, typeof(AssetReferenceSettings), false) as AssetReferenceSettings;
        if (settings == null)
        {
            return;
        }
        if (_settings != settings)
        {
            if (_settings != null)
            {
                Save();
            }
            _settings = settings;
            _assetLst = new List<Object>();
            for (int i = 0; i < _settings.AssetPathList.Count; i++)
            {
                _assetLst.Add(AssetDatabase.LoadAssetAtPath<Object>(_settings.AssetPathList[i].path));
            }
        }

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < _assetLst.Count; i++)
        {
            var value = i;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_assetLst[value], typeof(Object), false);
            if (GUILayout.Button("X", GUILayout.Width(100)))
            {
                _assetLst.RemoveAt(value);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        var eventType = Event.current.type;
        //在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var o in DragAndDrop.objectReferences)
                {
                    var path = AssetDatabase.GetAssetPath(o);
                    if (Directory.Exists(path))
                    {
                        List<string> pathList = new List<string>();
                        EditorHelper.GetAssetPath(pathList, path);
                        for (int i = 0; i < pathList.Count; i++)
                        {
                            Add(AssetDatabase.LoadAssetAtPath<Object>(pathList[i]));
                        }
                    }
                    else
                    {
                        Add(o);
                    }
                }
            }

            Event.current.Use();
        }
    }

    public void OnDestroy()
    {
        Save();
    }

    private void Save()
    {
        _settings.AssetPathList = new List<AssetReferenceSettings.Info>();
        for (int i = 0; i < _assetLst.Count; i++)
        {
            _settings.AssetPathList.Add(new AssetReferenceSettings.Info() { typeName = _assetLst[i].GetType().FullName, path = AssetDatabase.GetAssetPath(_assetLst[i]) });
        }
        EditorUtility.SetDirty(_settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void Add(Object o)
    {
        if (!_assetLst.Contains(o))
        {
            _assetLst.Add(o);
        }
    }
}