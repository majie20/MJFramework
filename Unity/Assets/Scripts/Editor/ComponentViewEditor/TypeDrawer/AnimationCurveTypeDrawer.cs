using System;
using UnityEditor;
using UnityEngine;

[TypeDrawer]
public class AnimationCurveTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.FullName == typeof(AnimationCurve).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.CurveField(memberName, (AnimationCurve)value);
    }
}