using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class Vector2TypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(Vector2).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.Vector2Field(fieldName, (Vector2)value);
    }
}