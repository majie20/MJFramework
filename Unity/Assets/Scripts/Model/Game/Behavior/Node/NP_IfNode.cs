using NPBehave;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_IfNode))]
    public class NP_IfNode : Composite
    {
        private System.Func<NP_BaseBehaviorTree, bool> Call;
        private bool                                   Result;

        public NP_IfNode() : base("NP_IfNode")
        {
        }

        public NP_IfNode(System.Func<NP_BaseBehaviorTree, bool> call, params Node[] children) : base("NP_IfNode", children)
        {
            this.Call = call;
        }

        public void Init(System.Func<NP_BaseBehaviorTree, bool> call, Node[] children)
        {
            base.Init(children);
            this.Call = call;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            var n = node as P_IfNode;
            Init(NP_LogicValue.JudgeUseCalls[n.JudgeType], children);
        }

        public override void Dispose()
        {
            Call = null;
            base.Dispose();
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