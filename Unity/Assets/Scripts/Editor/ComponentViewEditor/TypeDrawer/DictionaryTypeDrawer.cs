using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class DictionaryTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().FullName == typeof(Dictionary<,>).FullName;
    }

    private Dictionary<object, (int, bool)> map = new();

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        var types = memberType.GetGenericArguments();

        var typeDrawers = ComponentViewHelper.TypeDrawers;
        ITypeDrawer typeDrawer1 = null;
        ITypeDrawer typeDrawer2 = null;

        for (int i = 0; i < typeDrawers.Count; i++)
        {
            var typeDrawer = typeDrawers[i];

            if (!typeDrawer.HandlesType(types[0]))
            {
                continue;
            }

            typeDrawer1 = typeDrawer;

            break;
        }

        for (int i = 0; i < typeDrawers.Count; i++)
        {
            var typeDrawer = typeDrawers[i];

            if (!typeDrawer.HandlesType(types[1]))
            {
                continue;
            }

            typeDrawer2 = typeDrawer;

            break;
        }

        if (typeDrawer1 == null)
        {
            typeDrawer1 = new ObjectTypeDrawer();
        }

        if (typeDrawer2 == null)
        {
            typeDrawer2 = new ObjectTypeDrawer();
        }

        var dictionary = (value as IDictionary);

        GUILayout.BeginHorizontal();
        map.TryGetValue(value, out var data);
        data.Item2 = EditorGUILayout.Foldout(data.Item2, $"{fieldName}:");
        data.Item1 = EditorGUILayout.IntField("第几页：", data.Item1);
        map[value] = data;
        GUILayout.EndHorizontal();

        //绘制成功就继续绘制
        if (data.Item2)
        {
            EditorGUI.indentLevel++;
            var result1 = fieldName == "_assetOperationDic";
            var result2 = fieldName == "_subAssetOperationDic";
            string result3 = null;
            int j = 0;

            foreach (var k in dictionary.Keys)
            {
                if (j >= data.Item1 * 5 && j < (data.Item1 + 1) * 5 && j < dictionary.Count)
                {
                    var v = dictionary[k];
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    typeDrawer1.DrawAndGetNewValue(types[0], $"Key_{j}", k, null);
                    typeDrawer2.DrawAndGetNewValue(types[1], $"Value_{j}", v, null);
                    GUILayout.EndVertical();

                    if ((result1 || result2) && GUILayout.Button("卸载", GUILayout.ExpandHeight(true), GUILayout.Width(100)))
                    {
                        result3 = (string)k;
                    }

                    GUILayout.EndHorizontal();

                    var color = GUI.backgroundColor;
                    GUI.backgroundColor = Color.black;
                    GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(5));
                    GUI.backgroundColor = color;
                }

                j++;
            }

            if (!string.IsNullOrEmpty(result3))
            {
                if (result1)
                {
                    Model.Game.Instance.Scene.GetComponent<AssetsComponent>().Unload(result3);
                }
                else if (result2)
                {
                    Model.Game.Instance.Scene.GetComponent<AssetsComponent>().UnloadSub(result3);
                }
            }

            EditorGUI.indentLevel--;
        }

        return null;
    }
}