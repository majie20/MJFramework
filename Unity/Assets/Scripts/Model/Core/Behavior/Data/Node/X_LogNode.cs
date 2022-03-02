using XNode;

namespace Model
{
    [CreateNodeMenu("一般/Log")]
    [NP_NodeTypeBind(Type = typeof(NP_LogNode))]
    public class X_LogNode : X_BaseNode
    {
        [Input] public Empty In;
        [Output] public Empty Out;

        public string Content;

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
            (node as NP_LogNode)?.Init(Content, nodes[0]);
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_LogNode(Content, nodes[0]);
        }
    }
}