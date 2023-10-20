using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("一般/开始节点")]
    public class P_StartNode : P_BaseNode
    {
        [Output]
        public Empty Out;
    }
}