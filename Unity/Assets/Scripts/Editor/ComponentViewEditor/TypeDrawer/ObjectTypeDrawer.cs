using Model;
using NPBehave;
using System;
using UnityEditor;
using UnityEngine;

//[TypeDrawer]
public class ObjectTypeDrawer : ITypeDrawer
{
    [TypeDrawer]
    public bool HandlesType(Type type)
    {
        return true;
    }

    public object DrawAndGetNewValue(Type memberType, string fieldName, object value, object target)
    {
        GUILayout.BeginHorizontal();

        EditorGUILayout.TextField(fieldName, $"{value}", GUILayout.ExpandWidth(true));

        if (value is NP_BaseBehaviorTree instance && GUILayout.Button("Debuger"))
        {
            Debug.Log(memberType); // MDEBUG:
            DebuggerWindow.BehaviorTree = instance.Root;
            DebuggerWindow.ShowWindow();
        }

        GUILayout.EndHorizontal();

        return null;
    }
}