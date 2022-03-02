using XNode;

namespace Model
{
    [CreateNodeMenu("等待/Wait2")]
    [NodeWidth(250)]
    [NP_NodeTypeBind(Type = typeof(NP_WaitNode))]
    public class X_WaitNode_2 : X_BaseNode
    {
        [Input] public Empty In;
        [Output] public Empty Out;

        public string blackboardKey;
        public float randomVariance;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public override void InitNode(NPBehave.Node node, params NPBehave.Node[] nodes)
        {
            (node as NP_WaitNode)?.Init(blackboardKey, randomVariance, nodes[0]);
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_WaitNode(blackboardKey, randomVariance, nodes[0]);
        }
    }
}