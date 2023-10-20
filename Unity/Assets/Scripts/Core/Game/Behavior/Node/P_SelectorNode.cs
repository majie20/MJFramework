using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/Selector")]
    public class P_SelectorNode : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}