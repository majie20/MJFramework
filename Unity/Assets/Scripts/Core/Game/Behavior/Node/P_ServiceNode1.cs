using GraphProcessor;
using Sirenix.OdinInspector;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/Service_1")]
    public class P_ServiceNode1 : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        public float interval;
        public float randomVariance;

        [ValueDropdown("GetUpdateCallValues", ExpandAllMenuItems = true)]
        [LabelText("更新方法")]
        public string UpdateCall;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}