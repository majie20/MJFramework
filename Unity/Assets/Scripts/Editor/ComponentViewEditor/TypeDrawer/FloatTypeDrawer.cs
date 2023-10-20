using System;
using UnityEditor;

[TypeDrawer]
public class FloatTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(float).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.FloatField(memberName, (float)value);
    }
}