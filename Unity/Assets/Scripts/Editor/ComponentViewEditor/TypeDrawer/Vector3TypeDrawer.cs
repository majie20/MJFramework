using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class Vector3TypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(Vector3).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.Vector3Field(memberName, (Vector3)value);
    }
}