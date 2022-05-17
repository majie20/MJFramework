using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class UIPrefabToAtlasSettingsWindow : EditorWindow
{
    [MenuItem("Tools/资源收集/UI预制体和图集绑定器", false, 102)]
    public static void ShowWindow()
    {
        UIPrefabToAtlasSettingsWindow window = GetWindow<UIPrefabToAtlasSettingsWindow>("UI预制体和图集绑定器", true);
        window.minSize = new Vector2(800, 600);
    }

    private UIPrefabToAtlasSettings _settings;

    private void OnEnable()
    {
        _settings = AssetDatabase.LoadAssetAtPath<UIPrefabToAtlasSettings>(EditorConfig.UI_PREFAB_TO_ATLAS_SETTINGS_PATH);
        if (_settings == null)
        {
            _settings = ScriptableObject.CreateInstance<UIPrefabToAtlasSettings>();
            AssetDatabase.CreateAsset(_settings, EditorConfig.UI_PREFAB_TO_ATLAS_SETTINGS_PATH);
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        _infoDic = new Dictionary<GameObject, SpriteAtlas>();
        foreach (var info in _settings.InfoDic)
        {
            _infoDic.Add(AssetDatabase.LoadAssetAtPath<GameObject>(info.Key), AssetDatabase.LoadAssetAtPath<SpriteAtlas>(info.Value));
        }
    }

    private GameObject _operateObj;
    private SpriteAtlas _operateAtlas;

    private Dictionary<GameObject, SpriteAtlas> _infoDic;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        var obj = EditorGUILayout.ObjectField(_operateObj, typeof(GameObject), false);
        var atlas = EditorGUILayout.ObjectField(_operateAtlas, typeof(SpriteAtlas), false);
        if (atlas != null && atlas != _operateAtlas)
        {
            if (AssetDatabase.GetAssetPath(atlas).StartsWith(EditorConfig.ATLAS_PATH))
            {
                _operateAtlas = atlas as SpriteAtlas;
            }
        }
        if (obj != null && obj != _operateObj)
        {
            if (AssetDatabase.GetAssetPath(obj).StartsWith(EditorConfig.UI_PREFAB_PATH))
            {
                _operateObj = obj as GameObject;
                if (_operateObj != null && _infoDic.ContainsKey(_operateObj))
                {
                    _operateAtlas = _infoDic[_operateObj];
                }
            }
        }
        if (_operateObj != null && _operateAtlas != null)
        {
            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                if (!_infoDic.ContainsKey(_operateObj) && !_infoDic.ContainsValue(_operateAtlas))
                {
                    _infoDic.Add(_operateObj, _operateAtlas);
                    Save();
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("收集器：");

        EditorGUILayout.BeginVertical();
        GameObject temp = null;
        foreach (var info in _infoDic)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(info.Key, typeof(GameObject), false);
            EditorGUILayout.ObjectField(info.Value, typeof(SpriteAtlas), false);
            if (GUILayout.Button("X", GUILayout.Width(100)))
            {
                temp = info.Key;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (temp != null)
        {
            _infoDic.Remove(temp);
            Save();
        }

        EditorGUILayout.EndVertical();
    }

    public void OnDestroy()
    {
        Save();
    }

    private void Save()
    {
        _settings.InfoDic = new Dictionary<string, string>();
        foreach (var info in _infoDic)
        {
            _settings.InfoDic.Add(AssetDatabase.GetAssetPath(info.Key), AssetDatabase.GetAssetPath(info.Value));
        }
        EditorUtility.SetDirty(_settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}