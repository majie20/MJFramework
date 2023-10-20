using GraphProcessor;
using Sirenix.OdinInspector;

namespace Model
{
    [System.Serializable, NodeMenuItem("判断/判断节点")]
    public class P_IfNode : P_BaseNode
    {
        [DetailedInfoBox("详细描述", "通过下面条件类型进行判断")]
        [Input]
        public Empty In;

        [Output]
        public Empty True;

        [Output]
        public Empty False;

        [ValueDropdown("GetIfCallValues", ExpandAllMenuItems = true)]
        [LabelText("判断描述")]
        public string JudgeType;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}