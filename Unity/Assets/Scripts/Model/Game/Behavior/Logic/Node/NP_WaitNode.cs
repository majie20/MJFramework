using NPBehave;

namespace Model
{
    public class NP_WaitNode : Decorator
    {
        private string blackboardKey = null;
        private float seconds = -1f;
        private float randomVariance;

        public float RandomVariance
        {
            get
            {
                return randomVariance;
            }
            set
            {
                randomVariance = value;
            }
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

        public virtual void Init(float seconds, float randomVariance, Node decorate)
        {
            base.Init(decorate);
            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            this.seconds = seconds;
            this.randomVariance = randomVariance;
        }

        public virtual void Init(string blackboardKey, float randomVariance, Node decorate)
        {
            base.Init(decorate);
            this.blackboardKey = blackboardKey;
            this.randomVariance = randomVariance;
        }

        public override void Dispose()
        {
            base.Dispose();
            blackboardKey = null;
        }

        protected override void DoStart()
        {
            float seconds = this.seconds;
            if (this.blackboardKey != null)
            {
                seconds = Blackboard.Get<float>(this.blackboardKey);
            }
            //            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            if (seconds < 0)
            {
                seconds = 0;
            }

            if (randomVariance >= 0f)
            {
                Clock.AddTimer(seconds, randomVariance, 0, onTimer);
            }
            else
            {
                Clock.AddTimer(seconds, 0, onTimer);
            }
        }

        protected override void DoStop()
        {
            if (Clock.HasTimer(onTimer))
            {
                Clock.RemoveTimer(onTimer);
                Stopped(false);
            }
            else
            {
                Decoratee.Stop();
            }
        }

        private void onTimer()
        {
            Clock.RemoveTimer(onTimer);
            Decoratee.Start();
        }

        protected override void DoChildStopped(Node child, bool succeeded)
        {
            Stopped(succeeded);
        }
    }
}