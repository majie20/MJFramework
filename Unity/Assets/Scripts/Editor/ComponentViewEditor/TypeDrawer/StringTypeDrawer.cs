using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class StringTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(string).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.DelayedTextField(fieldName, (string)value, GUILayout.ExpandWidth(true));
    }
}