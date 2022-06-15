using Sirenix.OdinInspector;
using XNode;

namespace Model
{
    [CreateNodeMenu("执行/动作节点")]
    [NP_NodeTypeBind(Type = typeof(NP_ActionNode))]
    public class X_ActionNode : X_BaseNode
    {
        [DetailedInfoBox("详细描述", "执行一个动作")]

        [Input] public Empty In;
        [Output] public Empty Out;

        [ValueDropdown("GetLogicCallValues", ExpandAllMenuItems = true)]
        [LabelText("执行描述")]
        public string CallType;

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
            (node as NP_ActionNode)?.Init(NP_LogicValue.ActionUseCalls[CallType], nodes[0]);
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_ActionNode(NP_LogicValue.ActionUseCalls[CallType], nodes[0]);
        }
    }
}