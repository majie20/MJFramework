using UnityEngine.Assertions;

namespace NPBehave
{
    public class Root : Decorator
    {
        private Node mainNode;

        //private Node inProgressNode;

        private Blackboard blackboard;

        public override Blackboard Blackboard
        {
            get { return blackboard; }
        }

        private Clock clock;

        public override Clock Clock
        {
            get { return clock; }
        }

        private Model.NP_BaseBehaviorTree tree;

        public Model.NP_BaseBehaviorTree Tree
        {
            private set { tree = value; }
            get { return tree; }
        }

#if UNITY_EDITOR
        public int TotalNumStartCalls   = 0;
        public int TotalNumStopCalls    = 0;
        public int TotalNumStoppedCalls = 0;
#endif

        public Root(Node mainNode) : base("Root", mainNode)
        {
            this.mainNode = mainNode;
            var component = Model.Game.Instance.Scene.GetComponent<Model.NPContextComponent>();
            this.blackboard = component.HatchBlackboard();
            this.clock = this.blackboard.GetClock();
            this.SetRoot(this);
        }

        public Root(Blackboard blackboard, Node mainNode) : base("Root", mainNode)
        {
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = blackboard.GetClock();
            this.SetRoot(this);
        }

        public Root(Blackboard blackboard, Clock clock, Node mainNode) : base("Root", mainNode)
        {
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = clock;
            this.SetRoot(this);
        }

        public override void Init(Node mainNode)
        {
            base.Init(mainNode);
            this.mainNode = mainNode;
            var component = Model.Game.Instance.Scene.GetComponent<Model.NPContextComponent>();
            this.blackboard = component.HatchBlackboard();
            this.clock = this.blackboard.GetClock();
            this.SetRoot(this);
        }

        public void Init(Blackboard blackboard, Node mainNode)
        {
            base.Init(mainNode);
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = blackboard.GetClock();
            this.SetRoot(this);
        }

        public void Init(Blackboard blackboard, Clock clock, Node mainNode)
        {
            base.Init(mainNode);
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = clock;
            this.SetRoot(this);
        }

        public override void Dispose()
        {
            var component = Model.Game.Instance.Scene.GetComponent<Model.NPContextComponent>();
            this.blackboard.Dispose();
            component.RecycleBlackboard(this.blackboard);
            this.blackboard = null;
            clock.Dispose();
            component.RecycleClock(clock);
            this.clock = null;
            this.mainNode = null;
            this.Tree = null;
            base.Dispose();
        }

        public override void SetRoot(Root rootNode)
        {
            Assert.AreEqual(this, rootNode);
            base.SetRoot(rootNode);
            this.mainNode.SetRoot(rootNode);
        }

        public void SetTree(Model.NP_BaseBehaviorTree tree)
        {
            this.Tree = tree;
        }

        protected override void DoStart()
        {
            this.blackboard.Enable();
            this.mainNode.Start();
        }

        protected override void DoStop()
        {
            if (this.mainNode.IsActive)
            {
                this.mainNode.Stop();
            }
            else
            {
                this.clock.RemoveTimer(this.mainNode);
            }
        }

        protected override void DoChildStopped(Node node, bool success)
        {
            if (!IsStopRequested)
            {
                // wait one tick, to prevent endless recursions
                this.clock.AddTimer(0, 0, this.mainNode.Start);
            }
            else
            {
                this.blackboard.Disable();
                Stopped(success);
            }
        }
    }
}