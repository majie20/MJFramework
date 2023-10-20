using UnityEngine.Assertions;

namespace NPBehave
{
    public abstract class Composite : Container
    {
        protected Node[] Children;

        protected Composite(string name) : base(name)
        {
        }

        protected Composite(string name, Node[] children) : base(name)
        {
            this.Children = children;
            Assert.IsTrue(children.Length > 0, "Composite nodes (Selector, Sequence, Parallel) need at least one child!");

            for (int i = Children.Length - 1; i >= 0; i--)
            {
                Children[i].SetParent(this);
            }
        }

        public virtual void Init(Node[] children)
        {
            base.Init();
            this.Children = children;
            Assert.IsTrue(children.Length > 0, "Composite nodes (Selector, Sequence, Parallel) need at least one child!");

            for (int i = Children.Length - 1; i >= 0; i--)
            {
                Children[i].SetParent(this);
            }
        }

        public override void Dispose()
        {
            for (int i = 0; i < Children.Length; i++)
            {
                this.Children[i].Dispose();
            }

            this.Children = null;
            base.Dispose();
        }

        public override void SetRoot(Root rootNode)
        {
            base.SetRoot(rootNode);

            for (int i = Children.Length - 1; i >= 0; i--)
            {
                this.Children[i].SetRoot(rootNode);
            }
        }

#if UNITY_EDITOR
        public override Node[] DebugChildren
        {
            get { return this.Children; }
        }

        public Node DebugGetActiveChild()
        {
            foreach (Node node in DebugChildren)
            {
                if (node.CurrentState == Node.State.ACTIVE)
                {
                    return node;
                }
            }

            return null;
        }
#endif

        protected override void Stopped(bool success)
        {
            for (int i = 0; i < Children.Length; i++)
            {
                this.Children[i].ParentCompositeStopped(this);
            }

            base.Stopped(success);
        }

        public abstract void StopLowerPriorityChildrenForChild(Node child, bool immediateRestart);
    }
}