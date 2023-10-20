//using CatJson;
//using Model;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//using UnityEditor;
//using UnityEngine;

//public class UIPrefabToAtlasSettingsWindow : EditorWindow
//{
//    [MenuItem("Tools/资源收集/UI预制体和图集绑定器", false, 1)]
//    public static void ShowWindow()
//    {
//        UIPrefabToAtlasSettingsWindow window = GetWindow<UIPrefabToAtlasSettingsWindow>("UI预制体和图集绑定器", true);
//        window.minSize = new Vector2(800, 600);
//    }

//    public void OnEnable()
//    {
//        _infoDic = new Dictionary<GameObject, DefaultAsset>();

//        if (File.Exists(EditorConst.UI_PREFAB_TO_ATLAS_INFO))
//        {
//            var infoDic = JsonParser.Default.ParseJson<Dictionary<string, UIPrefabToAtlasInfo>>(AssetDatabase.LoadAssetAtPath<TextAsset>(EditorConst.UI_PREFAB_TO_ATLAS_INFO)
//               .text);

//            foreach (var info in infoDic)
//            {
//                _infoDic.Add(AssetDatabase.LoadAssetAtPath<GameObject>(info.Key), AssetDatabase.LoadAssetAtPath<DefaultAsset>($"{EditorConst.UI_SPRITE_PATH}{info.Value.Path}"));
//            }
//        }
//    }

//    private GameObject   _operateObj;
//    private DefaultAsset _operateDir;

//    private Dictionary<GameObject, DefaultAsset> _infoDic;

//    private void OnGUI()
//    {
//        EditorGUILayout.BeginHorizontal();
//        var obj = EditorGUILayout.ObjectField(_operateObj, typeof(GameObject), false);
//        var dir = EditorGUILayout.ObjectField(_operateDir, typeof(DefaultAsset), false);

//        if (dir != null && dir != _operateDir)
//        {
//            if (AssetDatabase.GetAssetPath(dir).StartsWith(EditorConst.UI_SPRITE_PATH))
//            {
//                _operateDir = dir as DefaultAsset;
//            }
//        }

//        if (obj != null && obj != _operateObj)
//        {
//            if (AssetDatabase.GetAssetPath(obj).StartsWith(EditorConst.UI_PREFAB_PATH))
//            {
//                _operateObj = obj as GameObject;

//                if (_operateObj != null && _infoDic.ContainsKey(_operateObj))
//                {
//                    _operateDir = _infoDic[_operateObj];
//                }
//            }
//        }

//        if (_operateObj != null && _operateDir != null)
//        {
//            if (GUILayout.Button("Add", GUILayout.Width(100)))
//            {
//                if (_infoDic.ContainsKey(_operateObj))
//                {
//                    if (_infoDic[_operateObj] != _operateDir)
//                    {
//                        _infoDic[_operateObj] = _operateDir;
//                        Save();
//                    }
//                }
//                else
//                {
//                    _infoDic.Add(_operateObj, _operateDir);
//                    Save();
//                }
//            }
//        }

//        EditorGUILayout.EndHorizontal();

//        EditorGUILayout.Space(20);
//        EditorGUILayout.LabelField("收集器：");

//        EditorGUILayout.BeginVertical();
//        GameObject temp = null;

//        foreach (var info in _infoDic)
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.ObjectField(info.Key, typeof(GameObject), false);
//            EditorGUILayout.ObjectField(info.Value, typeof(DefaultAsset), false);

//            if (GUILayout.Button("X", GUILayout.Width(100)))
//            {
//                temp = info.Key;
//            }

//            EditorGUILayout.EndHorizontal();
//        }

//        if (temp != null)
//        {
//            _infoDic.Remove(temp);
//            Save();
//        }

//        EditorGUILayout.EndVertical();
//    }

//    public void OnDestroy()
//    {
//        Save();
//    }

//    public void Save()
//    {
//        var infoDic = new Dictionary<string, UIPrefabToAtlasInfo>();

//        foreach (var info in _infoDic)
//        {
//            List<string> pathList = new List<string>();
//            var path = AssetDatabase.GetAssetPath(info.Value);
//            EditorHelper.GetAssetPath(pathList, path);

//            infoDic.Add(AssetDatabase.GetAssetPath(info.Key),
//                new UIPrefabToAtlasInfo() { Path = Regex.Replace(path, EditorConst.UI_SPRITE_PATH, ""), IsEmpty = pathList.Count == 0 });
//        }

//        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(EditorConst.UI_PREFAB_TO_ATLAS_INFO))
//        {
//            sw.WriteLine(JsonParser.Default.ToJson(infoDic));
//        }

//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }
//}