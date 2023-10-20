using Model;

namespace NPBehave
{
    [NP_NodeTypeBind(typeof(P_BlackboardConditionNode1))]
    [NP_NodeTypeBind(typeof(P_BlackboardConditionNode2))]
    public class NP_BlackboardConditionNode : ObservingDecorator
    {
        private float interval = -1.0f;
        private float randomVariation;

        private System.Func<NP_BaseBehaviorTree, bool> call;

        public NP_BlackboardConditionNode() : base("NP_BlackboardConditionNode")
        {
        }

        public NP_BlackboardConditionNode(System.Func<NP_BaseBehaviorTree, bool> call, Stops stopsOnChange, Node decoratee) : base("NP_BlackboardConditionNode", stopsOnChange, decoratee)
        {
            this.call = call;
        }

        public void Init(System.Func<NP_BaseBehaviorTree, bool> call, Stops stopsOnChange, Node decoratee)
        {
            base.Init(stopsOnChange, decoratee);
            this.call = call;
            this.interval = -1f;
        }

        public void Init(float interval, float randomVariation, System.Func<NP_BaseBehaviorTree, bool> call, Stops stopsOnChange, Node decoratee)
        {
            base.Init(stopsOnChange, decoratee);
            this.call = call;
            this.interval = interval;
            this.randomVariation = randomVariation;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            if (node is P_BlackboardConditionNode1 node1)
            {
                Init(NP_LogicValue.JudgeUseCalls[node1.JudgeType], node1.Stops, children[0]);
            }
            else if (node is P_BlackboardConditionNode2 node2)
            {
                Init(node2.interval, node2.randomVariation, NP_LogicValue.JudgeUseCalls[node2.JudgeType], node2.Stops, children[0]);
            }
        }

        public override void Dispose()
        {
            call = null;
            base.Dispose();
        }

        protected override void StartObserving()
        {
            if (this.interval <= 0f)
            {
                this.Clock.AddUpdateObserver(Evaluate);
            }
            else
            {
                this.Clock.AddTimer(interval, randomVariation, -1, Evaluate);
            }
        }

        protected override void StopObserving()
        {
            if (this.interval <= 0f)
            {
                this.Clock.RemoveUpdateObserver(this);
            }
            else
            {
                this.Clock.RemoveTimer(this);
            }
        }

        //private void onValueChanged()
        //{
        //    Evaluate();
        //}

        protected override bool IsConditionMet()
        {
            return this.call(this.RootNode.Tree);
        }
    }
}