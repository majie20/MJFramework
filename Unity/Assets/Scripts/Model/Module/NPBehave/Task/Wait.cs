namespace NPBehave
{
    public class Wait : Task
    {
        private System.Func<float> function = null;
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

        public Wait(float seconds, float randomVariance) : base("Wait")
        {
            this.Init(seconds, randomVariance);
        }

        public Wait(float seconds) : base("Wait")
        {
            this.Init(seconds, this.seconds * 0.05f);
        }

        public Wait(string blackboardKey, float randomVariance = 0f) : base("Wait")
        {
            this.Init(blackboardKey, randomVariance);
        }

        public Wait(System.Func<float> function, float randomVariance = 0f) : base("Wait")
        {
            this.function = function;
            this.randomVariance = randomVariance;
        }

        public virtual void Init(float seconds, float randomVariance)
        {
            UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            this.seconds = seconds;
            this.randomVariance = randomVariance;
        }

        public virtual void Init(string blackboardKey, float randomVariance = 0f)
        {
            this.blackboardKey = blackboardKey;
            this.randomVariance = randomVariance;
        }

        public override void Dispose()
        {
            base.Dispose();
            function = null;
            blackboardKey = null;
        }

        protected override void DoStart()
        {
            float seconds = this.seconds;
            if (seconds < 0)
            {
                if (this.blackboardKey != null)
                {
                    seconds = Blackboard.Get<float>(this.blackboardKey);
                }
                else if (this.function != null)
                {
                    seconds = this.function();
                }
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
            Clock.RemoveTimer(this);
            this.Stopped(false);
        }

        private void onTimer()
        {
            Clock.RemoveTimer(this);
            this.Stopped(true);
        }
    }
}