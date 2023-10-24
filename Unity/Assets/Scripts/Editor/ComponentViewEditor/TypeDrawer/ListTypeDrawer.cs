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

    private Dictionary<object, (int, bool)> map = new();

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
        map.TryGetValue(value, out var data);
        data.Item2 = EditorGUILayout.Foldout(data.Item2, $"{fieldName}:");
        data.Item1 = EditorGUILayout.IntField("第几页：", data.Item1);
        map[value] = data;
        GUILayout.EndHorizontal();

        //绘制成功就继续绘制
        if (data.Item2)
        {
            EditorGUI.indentLevel++;

            for (int j = data.Item1 * 5; j < (data.Item1 + 1) * 5 && j < list.Count; j++)
            {
                var o = list[j];

                typeDrawer1.DrawAndGetNewValue(type, $"Element_{j}", o, null);
            }

            EditorGUI.indentLevel--;
        }

        return null;
    }
}