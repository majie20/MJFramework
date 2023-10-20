using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("一般/结束节点")]
    public class P_EndNode : P_BaseNode
    {
        [Input]
        public Empty In;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}