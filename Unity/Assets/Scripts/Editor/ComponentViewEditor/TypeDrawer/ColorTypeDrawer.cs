using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class ColorTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(Color).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.ColorField(fieldName, (Color)value);
    }
}