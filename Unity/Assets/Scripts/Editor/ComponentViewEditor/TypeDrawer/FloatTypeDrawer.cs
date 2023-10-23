using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class FloatTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(float).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.FloatField(fieldName, (float)value, GUILayout.ExpandWidth(true));
    }
}