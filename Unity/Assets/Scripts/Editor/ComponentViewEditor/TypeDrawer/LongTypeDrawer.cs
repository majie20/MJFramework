using System;
using UnityEditor;

[TypeDrawer]
public class LongTypeDrawer : ITypeDrawer
{
    [TypeDrawer]
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(long).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.LongField(memberName, (long)value);
    }
}