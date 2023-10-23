using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class EnumTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.IsEnum;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        if (memberType.IsDefined(typeof(FlagsAttribute), false))
        {
            return EditorGUILayout.EnumFlagsField(fieldName, (Enum)value, GUILayout.ExpandWidth(true));
        }

        return EditorGUILayout.EnumPopup(fieldName, (Enum)value, GUILayout.ExpandWidth(true));
    }
}