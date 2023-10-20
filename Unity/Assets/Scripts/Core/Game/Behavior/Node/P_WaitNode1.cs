using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("等待/延迟节点1")]
    public class P_WaitNode1 : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        public float seconds;
        public float randomVariance;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}