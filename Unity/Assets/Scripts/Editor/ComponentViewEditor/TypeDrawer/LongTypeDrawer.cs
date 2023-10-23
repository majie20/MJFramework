using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class LongTypeDrawer : ITypeDrawer
{
    [TypeDrawer]
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(long).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.LongField(fieldName, (long)value, GUILayout.ExpandWidth(true));
    }
}