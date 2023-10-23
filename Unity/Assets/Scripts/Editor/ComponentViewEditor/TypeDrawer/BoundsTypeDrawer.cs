using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class BoundsTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(Bounds).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.BoundsField(fieldName, (Bounds)value);
    }
}