using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[TypeDrawer]
public class ListTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return (type.IsGenericType && type.GetGenericTypeDefinition().FullName == typeof(List<>).FullName) || type.IsArray;
    }

    public Dictionary<object, int> Map = new();

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        Type type;

        if (memberType.IsArray)
        {
            type = memberType.GetElementType();
        }
        else
        {
            type = memberType.GetGenericArguments().First();
        }

        var typeDrawers = ComponentViewHelper.TypeDrawers;
        ITypeDrawer typeDrawer1 = null;

        for (int i = 0; i < typeDrawers.Count; i++)
        {
            var typeDrawer = typeDrawers[i];

            if (!typeDrawer.HandlesType(type))
            {
                continue;
            }

            typeDrawer1 = typeDrawer;

            break;
        }

        if (typeDrawer1 == null)
        {
            typeDrawer1 = new ObjectTypeDrawer();
        }

        var list = (value as IList);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{fieldName}:");
        Map.TryGetValue(value, out var num);
        num = EditorGUILayout.IntField("µÚ¼¸Ò³£º", num);
        Map[value] = num;
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        for (int j = num * 5; j < (num + 1) * 5 && j < list.Count; j++)
        {
            var o = list[j];

            typeDrawer1.DrawAndGetNewValue(type, $"Element_{j}", o, null);
        }

        EditorGUI.indentLevel--;

        return null;
    }
}