using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class RectTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(Rect).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.RectField(fieldName, (Rect)value);
    }
}