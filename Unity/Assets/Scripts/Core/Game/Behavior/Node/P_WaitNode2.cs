using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("等待/延迟节点2")]
    public class P_WaitNode2 : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        public string blackboardKey;
        public float  randomVariance;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}