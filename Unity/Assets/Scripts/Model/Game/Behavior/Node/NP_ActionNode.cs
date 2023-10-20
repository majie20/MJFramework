using NPBehave;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_ActionNode))]
    public class NP_ActionNode : Decorator
    {
        private System.Action<NP_BaseBehaviorTree> Call;

        public NP_ActionNode() : base("NP_ActionNode")
        {
        }

        public NP_ActionNode(System.Action<NP_BaseBehaviorTree> call, Node decoratee) : base("NP_ActionNode", decoratee)
        {
            this.Call = call;
        }

        public void Init(System.Action<NP_BaseBehaviorTree> call, Node decoratee)
        {
            base.Init(decoratee);
            this.Call = call;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            var n = node as P_ActionNode;
            Init(NP_LogicValue.ActionUseCalls[n.CallType], children[0]);
        }

        public override void Dispose()
        {
            base.Dispose();
            Call = null;
        }

        protected override void DoStart()
        {
            this.Call.Invoke(this.RootNode.Tree);
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