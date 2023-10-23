using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class DateTimeTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(DateTime).FullName;
    }

    // Note: This is a very basic implementation. The ToString() method conversion will cut off milliseconds.
    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        var dateString = value.ToString();
        //EditorGUI.
        var newDateString = EditorGUILayout.TextField(fieldName, dateString, GUILayout.ExpandWidth(true));

        return newDateString != dateString
                ? DateTime.Parse(newDateString)
                : value;
    }
}