using System;
using UnityEditor;

[TypeDrawer]
public class IntTypeDrawer : ITypeDrawer
{
    [TypeDrawer]
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(int).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.IntField(memberName, (int)value);
    }
}