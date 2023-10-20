using GraphProcessor;
using Sirenix.OdinInspector;

namespace Model
{
    [System.Serializable, NodeMenuItem("执行/动作节点")]
    public class P_ActionNode : P_BaseNode
    {
        [DetailedInfoBox("详细描述", "执行一个动作")]
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        [ValueDropdown("GetLogicCallValues", ExpandAllMenuItems = true)]
        [LabelText("执行描述")]
        public string CallType;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}