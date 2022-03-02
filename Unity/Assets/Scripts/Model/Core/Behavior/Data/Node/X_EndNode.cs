using XNode;

namespace Model
{
    [CreateNodeMenu("一般/End")]
    [NP_NodeTypeBind(Type = typeof(NP_EndNode))]
    public class X_EndNode : X_BaseNode
    {
        [Input] public Empty In;

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
            
        }

        public override NPBehave.Node CreateNode(params NPBehave.Node[] nodes)
        {
            return new NP_EndNode();
        }
    }
}