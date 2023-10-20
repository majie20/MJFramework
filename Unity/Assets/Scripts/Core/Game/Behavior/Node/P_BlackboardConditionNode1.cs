using GraphProcessor;
using Sirenix.OdinInspector;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/BlackboardCondition1")]
    public class P_BlackboardConditionNode1 : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        [ValueDropdown("GetIfCallValues", ExpandAllMenuItems = true)]
        [LabelText("判断描述")]
        public string JudgeType;

        public NPBehave.Stops Stops;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}