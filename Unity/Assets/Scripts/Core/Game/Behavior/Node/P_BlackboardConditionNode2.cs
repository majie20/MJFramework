using GraphProcessor;
using Sirenix.OdinInspector;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/BlackboardCondition2")]
    public class P_BlackboardConditionNode2 : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        [ValueDropdown("GetIfCallValues", ExpandAllMenuItems = true)]
        [LabelText("判断描述")]
        public string JudgeType;

        public NPBehave.Stops Stops;

        public float interval;
        public float randomVariation;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}