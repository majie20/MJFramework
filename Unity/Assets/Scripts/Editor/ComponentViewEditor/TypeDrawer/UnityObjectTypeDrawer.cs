using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class UnityObjectTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(UnityEngine.Object).FullName || type.IsSubclassOf(typeof(UnityEngine.Object));
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        //EditorGUI.ObjectField()
        return EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)value, memberType, true);
        //return null;
    }
}