using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Model
{
    [CreateNodeMenu("判断/If")]
    [NodeWidth(300)]
    [NP_NodeTypeBind(Type = typeof(NP_IfNode))]
    public class X_IfNode : X_BaseNode
    {
        [Input] public Empty In;
        [Output] public Empty True;
        [Output] public Empty False;

        [ValueDropdown("GetLogicIfValues", ExpandAllMenuItems = true)]
        [LabelText("判断描述")]
        public string JudgeType;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public override void InitNode(NPBehave.Node node, params NPBehave.Node[] nodes)
        {
            (node as NP_IfNode)?.Init(NP_LogicValue.JudgeUseCalls[JudgeType], nodes);
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_IfNode(NP_LogicValue.JudgeUseCalls[JudgeType], nodes);
        }
    }
}