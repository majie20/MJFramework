using NPBehave;

namespace Model
{
    public class NP_IfNode : Composite
    {
        private System.Func<NP_BaseBehaviorTree, bool> Call;
        private bool Result;

        public NP_IfNode(System.Func<NP_BaseBehaviorTree, bool> call, params Node[] children) : base("NP_IfNode", children)
        {
            this.Init(call, children);
        }

        public virtual void Init(System.Func<NP_BaseBehaviorTree, bool> call, Node[] children)
        {
            base.Init(children);
            this.Call = call;
        }

        protected override void DoStart()
        {
            this.Result = this.Call(this.RootNode.Tree);
            if (this.Result)
            {
                Children[0].Start();
            }
            else
            {
                Children[1].Start();
            }
        }

        protected override void DoStop()
        {
            if (this.Result)
            {
                Children[0].Stop();
            }
            else
            {
                Children[1].Stop();
            }
        }

        public override void StopLowerPriorityChildrenForChild(Node child, bool immediateRestart)
        {
        }

        protected override void DoChildStopped(Node child, bool succeeded)
        {
            Stopped(succeeded);
        }
    }
}