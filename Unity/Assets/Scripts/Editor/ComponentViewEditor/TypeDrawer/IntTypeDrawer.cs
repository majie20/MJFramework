using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class IntTypeDrawer : ITypeDrawer
{
    [TypeDrawer]
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(int).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        return EditorGUILayout.IntField(fieldName, (int)value,GUILayout.ExpandWidth(true));
    }
}