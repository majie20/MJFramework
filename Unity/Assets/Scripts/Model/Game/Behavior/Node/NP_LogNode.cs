using NPBehave;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_LogNode))]
    public class NP_LogNode : Decorator
    {
        private string Content;

        public NP_LogNode() : base("NP_LogNode")
        {
        }

        public NP_LogNode(string content, Node decoratee) : base("NP_LogNode", decoratee)
        {
            this.Content = content;
        }

        public void Init(string content, Node decoratee)
        {
            base.Init(decoratee);
            this.Content = content;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            var n = node as P_LogNode;
            Init(n.Content, children[0]);
        }

        protected override void DoStart()
        {
            NLog.Log.Debug(this.Content); // MDEBUG:
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