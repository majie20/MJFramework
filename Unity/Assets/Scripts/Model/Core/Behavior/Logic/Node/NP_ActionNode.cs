using NPBehave;

namespace Model
{
    public class NP_ActionNode : Decorator
    {
        private System.Action<NP_BaseBehaviorTree> Call;

        public NP_ActionNode(System.Action<NP_BaseBehaviorTree> call, Node decoratee) : base("NP_ActionNode", decoratee)
        {
            this.Call = call;
        }

        public void Init(System.Action<NP_BaseBehaviorTree> call, Node decoratee)
        {
            base.Init(decoratee);
            this.Call = call;
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