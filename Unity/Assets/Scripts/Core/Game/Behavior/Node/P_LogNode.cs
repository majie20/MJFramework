using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("一般/打印节点")]
    public class P_LogNode : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        public string Content;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}