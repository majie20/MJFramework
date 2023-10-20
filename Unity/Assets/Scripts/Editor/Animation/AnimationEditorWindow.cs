using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationEditorWindow : EditorWindow
{
    [MenuItem("Tools/动画/动画复制界面", false, 0)]
    public static void ShowWindow()
    {
        AnimationEditorWindow window = GetWindow<AnimationEditorWindow>("动画复制界面", true);
        window.minSize = new Vector2(600, 400);
    }

    private AnimatorController _animatorController;
    private string _path;

    private List<AnimationClip> _animationClips = new List<AnimationClip>();
    private List<string> _animationClipNameList = new List<string>();

    private int _value;
    private Vector2 _vec;

    private void OnGUI()
    {
        var ac = EditorGUILayout.ObjectField(_animatorController, typeof(AnimatorController), false) as AnimatorController;

        if (ac == null)
        {
            return;
        }
        else
        {
            if (ac != _animatorController)
            {
                _value = 7400000;
                _animationClipNameList.Clear();
                _animationClips.Clear();
                _animatorController = ac;
                _path = AssetDatabase.GetAssetPath(_animatorController);
                ReadAnimationInfo();
            }
        }

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("现有动画：");
        for (int i = _animationClipNameList.Count - 1; i >= 0; i--)
        {
            stringBuilder.Append(_animationClipNameList[i]);
            stringBuilder.Append(", ");
        }
        EditorGUILayout.LabelField(stringBuilder.ToString(), new GUIStyle() { alignment = TextAnchor.MiddleLeft }, GUILayout.Width(60));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("拖入动画：", new GUIStyle() { alignment = TextAnchor.MiddleLeft }, GUILayout.Width(60));
        _vec = EditorGUILayout.BeginScrollView(_vec);
        EditorGUILayout.BeginVertical();

        int index = -1;
        for (int i = _animationClips.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_animationClips[i], typeof(AnimationClip), false);
            if (GUILayout.Button("X", GUILayout.Width(100)))
            {
                index = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (index != -1)
        {
            _animationClips.RemoveAt(index);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

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
                    if (o is AnimationClip clip && !_animationClipNameList.Contains(clip.name.ToLower()))
                    {
                        _animationClips.Add(clip);
                    }
                }
            }

            Event.current.Use();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("添加", GUILayout.Width(100), GUILayout.Height(50)) && _animationClips.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = _animationClips.Count - 1; i >= 0; i--)
            {
                var str = File.ReadAllText(AssetDatabase.GetAssetPath(_animationClips[i]));
                var pattern = @"(--- !u![(0-9-)( &\r\n)]*AnimationClip[-\r\n _a-zA-Z0-9:{}\[\],\.]*" + _animationClips[i].name + @"[-\r\n _a-zA-Z0-9:{}\[\],\.]*)(\z|---)";
                Match match = Regex.Match(str, pattern);
                sb.Append(Regex.Replace(match.Groups[1].Value, "74[0-9]{5}", (++_value).ToString()));
            }

            using (StreamWriter file = new StreamWriter(_path, true))
            {
                file.Write(sb);
            }

            _value = 7400000;
            _animationClips.Clear();
            _animationClipNameList.Clear();
            ReadAnimationInfo();

            EditorUtility.SetDirty(_animatorController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void ReadAnimationInfo()
    {
        var str = File.ReadAllText(_path);
        var subStrList = str.Split(new[] { "--- !u!" }, StringSplitOptions.None);

        for (int i = subStrList.Length - 1; i >= 0; i--)
        {
            Match match = Regex.Match(subStrList[i], "[(0-9-)( &)]+[\r\n]*([a-zA-Z]+):");
            if (match.Success && match.Result("$1") == "AnimationClip")
            {
                Match subMatch = Regex.Match(subStrList[i], "m_Name: ([a-zA-Z0-9_ ]*)[\r\n]*");
                _animationClipNameList.Add(subMatch.Result("$1").ToLower());
                var value = int.Parse(Regex.Match(subStrList[i], "(74[0-9]{5})").Groups[1].Value);
                if (_value < value)
                {
                    _value = value;
                }
            }
        }
    }
}