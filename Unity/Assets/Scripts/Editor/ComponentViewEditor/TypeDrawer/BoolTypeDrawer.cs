using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class BoolTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(bool).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.Toggle(fieldName, (bool)value, GUILayout.ExpandWidth(true));
    }
}