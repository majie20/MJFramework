using System;
using UnityEditor;

[TypeDrawer]
public class UnityObjectTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(UnityEngine.Object).FullName ||
                type.IsSubclassOf(typeof(UnityEngine.Object));
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true);
    }
}