using NPBehave;
using System.Collections.Generic;

namespace Model
{
    public class NP_BaseBehaviorTree : System.IDisposable
    {
        private Root root;

        public Root Root
        {
            get
            {
                return root;
            }
            private set
            {
                root = value;
            }
        }

        private X_StartNode StartNode;
        private BaseGraph BaseGraph;

        public NP_BaseBehaviorTree(BaseGraph graph)
        {
            this.StartNode = graph.GetNodeFirstByType<X_StartNode>();
            this.BaseGraph = graph;
        }

        public virtual void Init()
        {
            Root = CreateRoot();
            Root.SetTree(this);
        }

        public Root CreateRoot()
        {
            return Game.Instance.ObjectPool.GetComponent<NPNodePoolComponent>().HatchRoot(CreateNode(StartNode));
        }

        public Node CreateNode(X_BaseNode x_node)
        {
            var nodes = new List<Node>();
            if (x_node is X_IfNode)
            {
                nodes.Add(CreateNode(x_node.GetOutputPort("True").Connection.node as X_BaseNode));
                nodes.Add(CreateNode(x_node.GetOutputPort("False").Connection.node as X_BaseNode));
            }
            else
            {
                foreach (var port in x_node.Outputs) nodes.Add(CreateNode(port.Connection.node as X_BaseNode));
            }
            return Game.Instance.ObjectPool.GetComponent<NPNodePoolComponent>().HatchNode(x_node, nodes.ToArray());
        }

        public virtual void Dispose()
        {
            Root.Dispose();
            Game.Instance.ObjectPool.GetComponent<NPNodePoolComponent>().RecycleNode(this.Root);
            this.Root = null;
        }

        public virtual void Start()
        {
            this.Root.Start();
        }

        public virtual void Stop()
        {
            this.Root.Stop();
        }
    }
}