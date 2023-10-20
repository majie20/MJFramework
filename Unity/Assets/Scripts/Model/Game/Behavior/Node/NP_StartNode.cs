using NPBehave;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_StartNode))]
    public class NP_StartNode : Decorator
    {
        public NP_StartNode() : base("NP_StartNode")
        {
        }

        public NP_StartNode(Node decoratee) : base("NP_StartNode", decoratee)
        {
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            var n = node as P_LogNode;
            Init(children[0]);
        }

        protected override void DoStart()
        {
            Decoratee.Start();
        }

        protected override void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}