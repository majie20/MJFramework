using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class DictionaryTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().FullName == typeof(Dictionary<,>).FullName;
    }

    public Dictionary<object, int> Map = new();

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
        EditorGUILayout.LabelField($"{fieldName}:");
        Map.TryGetValue(value, out var num);
        num = EditorGUILayout.IntField("µÚ¼¸Ò³£º", num);
        Map[value] = num;
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        int j = 0;

        foreach (var k in dictionary.Keys)
        {
            if (j >= num * 5 && j < (num + 1) * 5 && j < dictionary.Count)
            {
                var v = dictionary[k];
                typeDrawer1.DrawAndGetNewValue(types[0], $"Key_{j}", k, null);
                typeDrawer2.DrawAndGetNewValue(types[1], $"Value_{j}", v, null);
                var color = GUI.backgroundColor;
                GUI.backgroundColor = Color.black;
                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUI.backgroundColor = color;
            }

            j++;
        }

        EditorGUI.indentLevel--;

        return null;
    }
}