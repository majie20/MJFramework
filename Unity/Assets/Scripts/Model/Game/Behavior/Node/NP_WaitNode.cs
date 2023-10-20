using NPBehave;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_WaitNode1))]
    [NP_NodeTypeBind(typeof(P_WaitNode2))]
    public class NP_WaitNode : Decorator
    {
        private string blackboardKey = null;
        private float  seconds       = -1f;
        private float  randomVariance;

        public float RandomVariance
        {
            get { return randomVariance; }
            set { randomVariance = value; }
        }

        public NP_WaitNode() : base("NP_WaitNode")
        {
        }

        public NP_WaitNode(float seconds, float randomVariance, Node decorate) : base("NP_WaitNode", decorate)
        {
            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            this.seconds = seconds;
            this.randomVariance = randomVariance;
        }

        public NP_WaitNode(float seconds, Node decoratee) : base("NP_WaitNode", decoratee)
        {
            this.seconds = seconds;
            this.randomVariance = this.seconds * 0.05f;
        }

        public NP_WaitNode(string blackboardKey, float randomVariance, Node decorate) : base("NP_WaitNode", decorate)
        {
            this.blackboardKey = blackboardKey;
            this.randomVariance = randomVariance;
        }

        public void Init(float seconds, float randomVariance, Node decorate)
        {
            base.Init(decorate);
            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            this.seconds = seconds;
            this.randomVariance = randomVariance;
        }

        public void Init(string blackboardKey, float randomVariance, Node decorate)
        {
            base.Init(decorate);
            this.blackboardKey = blackboardKey;
            this.randomVariance = randomVariance;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            if (node is P_WaitNode1 node1)
            {
                Init(node1.seconds, node1.randomVariance, children[0]);
            }
            else if (node is P_WaitNode2 node2)
            {
                Init(node2.blackboardKey, node2.randomVariance, children[0]);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void DoStart()
        {
            float seconds = this.seconds;

            if (this.blackboardKey != null)
            {
                seconds = Blackboard.Get<float>(this.blackboardKey);
            }

            //UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            if (seconds < 0)
            {
                seconds = 0;
            }

            if (randomVariance >= 0f)
            {
                Clock.AddTimer(seconds, randomVariance, 1, onTimer);
            }
            else
            {
                Clock.AddTimer(seconds, 1, onTimer);
            }
        }

        protected override void DoStop()
        {
            if (Clock.HasTimer(this))
            {
                Clock.RemoveTimer(this);
                Stopped(false);
            }
            else
            {
                Decoratee.Stop();
            }
        }

        private void onTimer()
        {
            Clock.RemoveTimer(this);
            Decoratee.Start();
        }

        protected override void DoChildStopped(Node child, bool succeeded)
        {
            Stopped(succeeded);
        }
    }
}