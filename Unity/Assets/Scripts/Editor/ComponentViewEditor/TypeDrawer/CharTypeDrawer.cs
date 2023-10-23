using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class CharTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(char).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        var str = EditorGUILayout.TextField(fieldName, ((char)value).ToString(), GUILayout.ExpandWidth(true));
        return str.Length > 0 ? str[0] : default(char);
    }
}