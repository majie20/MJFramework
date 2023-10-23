using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class DoubleTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(double).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.DoubleField(fieldName, (double)value, GUILayout.ExpandWidth(true));
    }
}