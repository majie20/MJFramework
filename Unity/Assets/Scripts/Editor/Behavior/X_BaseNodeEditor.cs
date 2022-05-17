using Model;
using System;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(X_BaseNode))]
public class X_BaseNodeEditor : NodeEditor
{
    public override void OnHeaderGUI()
    {
        string content = "";
        Type type = target.GetType();
        if (type == typeof(X_StartNode))
        {
            content = "开始节点";
        }
        else if (type == typeof(X_EndNode))
        {
            content = "结束节点";
        }
        else if (type == typeof(X_LogNode))
        {
            content = "打印节点";
        }
        else if (type == typeof(X_ActionNode))
        {
            content = "动作节点";
        }
        else if (type == typeof(X_WaitNode_1))
        {
            content = "延迟节点1";
        }
        else if (type == typeof(X_WaitNode_2))
        {
            content = "延迟节点2";
        }
        else if (type == typeof(X_IfNode))
        {
            content = "判断节点";
        }

        GUILayout.Label(content, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }
}