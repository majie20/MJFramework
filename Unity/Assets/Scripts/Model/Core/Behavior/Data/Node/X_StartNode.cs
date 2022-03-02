using XNode;

namespace Model
{
    [CreateNodeMenu("一般/Start")]
    [NP_NodeTypeBind(Type = typeof(NP_StartNode))]
    public class X_StartNode : X_BaseNode
    {
        [Output] public Empty Out;

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
            (node as NP_StartNode)?.Init(nodes[0]);
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_StartNode(nodes[0]);
        }
    }
}