using UnityEngine.Assertions;

namespace NPBehave
{
    public abstract class Composite : Container
    {
        protected Node[] Children;

        protected Composite(string name, Node[] children) : base(name)
        {
            this.Init(children);
        }

        public virtual void Init(Node[] children)
        {
            base.Init();
            this.Children = children;
            Assert.IsTrue(children.Length > 0, "Composite nodes (Selector, Sequence, Parallel) need at least one child!");

            foreach (Node node in Children)
            {
                node.SetParent(this);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            for (int i = 0; i < Children.Length; i++)
            {
                this.Children[i].Dispose();
                Model.Game.Instance.ObjectPool.GetComponent<Model.NPNodePoolComponent>().RecycleNode(Children[i]);
            }
            this.Children = null;
        }

        public override void SetRoot(Root rootNode)
        {
            base.SetRoot(rootNode);

            foreach (Node node in Children)
            {
                node.SetRoot(rootNode);
            }
        }


#if UNITY_EDITOR
        public override Node[] DebugChildren
        {
            get
            {
                return this.Children;
            }
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
            foreach (Node child in Children)
            {
                child.ParentCompositeStopped(this);
            }
            base.Stopped(success);
        }

        public abstract void StopLowerPriorityChildrenForChild(Node child, bool immediateRestart);
    }
}