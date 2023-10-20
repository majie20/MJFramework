using Model;

namespace NPBehave
{
    [NP_NodeTypeBind(typeof(P_ServiceNode1))]
    [NP_NodeTypeBind(typeof(P_ServiceNode2))]
    public class Service : Decorator
    {
        private System.Action<NP_BaseBehaviorTree> serviceMethod;

        private float interval = -1.0f;
        private float randomVariation;

        public Service(float interval, float randomVariation, System.Action<NP_BaseBehaviorTree> service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = randomVariation;

            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(float interval, System.Action<NP_BaseBehaviorTree> service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = interval * 0.05f;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(System.Action<NP_BaseBehaviorTree> service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.Label = "every tick";
        }

        public void Init(float interval, float randomVariance, System.Action<NP_BaseBehaviorTree> service, Node decorate)
        {
            base.Init(decorate);
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = randomVariance;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public void Init(float interval, System.Action<NP_BaseBehaviorTree> service, Node decorate)
        {
            base.Init(decorate);
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = interval * 0.05f;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public void Init(System.Action<NP_BaseBehaviorTree> service, Node decorate)
        {
            base.Init(decorate);
            this.interval = -1f;
            this.randomVariation = 0f;
            this.serviceMethod = service;
            this.Label = "every tick";
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            if (node is P_ServiceNode1 node1)
            {
                Init(node1.interval, node1.randomVariance, NP_LogicValue.UpdateUseCalls[node1.UpdateCall], children[0]);
            }
            else if (node is P_ServiceNode2 node2)
            {
                Init(NP_LogicValue.UpdateUseCalls[node2.UpdateCall], children[0]);
            }
        }

        protected override void DoStart()
        {
            if (this.interval <= 0f)
            {
                this.Clock.AddUpdateObserver(InvokeServiceMethodWithRandomVariation);
            }
            else
            {
                this.Clock.AddTimer(interval, randomVariation, -1, InvokeServiceMethodWithRandomVariation);
            }

            InvokeServiceMethodWithRandomVariation();
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (this.interval <= 0f)
            {
                this.Clock.RemoveUpdateObserver(this);
            }
            else
            {
                this.Clock.RemoveTimer(this);
            }

            Stopped(result);
        }

        private void InvokeServiceMethodWithRandomVariation()
        {
            serviceMethod(this.RootNode.Tree);
        }
    }
}