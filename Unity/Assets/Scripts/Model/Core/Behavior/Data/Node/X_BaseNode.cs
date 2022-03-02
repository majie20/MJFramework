using System.Collections;
using Sirenix.OdinInspector;
using XNode;

namespace Model
{
    public abstract class X_BaseNode : Node
    {
        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public abstract void InitNode(NPBehave.Node node, params NPBehave.Node[] nodes);

        public abstract NPBehave.Node CreateNode(params NPBehave.Node[] nodes);

#if UNITY_EDITOR
        public static IEnumerable GetLogicIfValues = new ValueDropdownList<string>()
        {
            {"判断和目标的距离",NP_LogicValue.If_JudgeAndTargetDistance},
        };

        public static IEnumerable GetLogicCallValues = new ValueDropdownList<string>()
        {
            {"打个屁",NP_LogicValue.Action_Test},
        };
#endif
    }
}