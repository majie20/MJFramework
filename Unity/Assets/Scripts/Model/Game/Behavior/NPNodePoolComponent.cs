using System;
using System.Collections.Generic;
using NPBehave;
using Sirenix.Utilities;

namespace Model
{
    public class NPNodePoolComponent : Component, IAwake
    {
        private Dictionary<Type, Queue<Node>> nodeDic;

        public void Awake()
        {
            nodeDic = new Dictionary<Type, Queue<Node>>();
        }

        public override void Dispose()
        {
            nodeDic = null;
            base.Dispose();
        }

        public Node HatchNode(X_BaseNode x_node, params Node[] nodes)
        {
            var type = x_node.GetType().GetCustomAttribute<NP_NodeTypeBindAttribute>().Type;

            if (nodeDic.ContainsKey(type))
            {
                var np_node = nodeDic[type].Dequeue();
                x_node.InitNode(np_node, nodes);
                return np_node;
            }

            return x_node.CreateNode(nodes);
        }

        public Root HatchRoot(Node mainNode)
        {
            var type = typeof(Root);

            if (nodeDic.ContainsKey(type))
            {
                var np_node = nodeDic[type].Dequeue() as Root;
                np_node?.Init(mainNode);

                return np_node;
            }

            return new Root(mainNode);
        }

        public Root HatchRoot(Blackboard blackboard, Node mainNode)
        {
            var type = typeof(Root);

            if (nodeDic.ContainsKey(type))
            {
                var np_node = nodeDic[type].Dequeue() as Root;
                np_node?.Init(blackboard, mainNode);

                return np_node;
            }

            return new Root(mainNode);
        }

        public Root HatchRoot(Blackboard blackboard, Clock clock, Node mainNode)
        {
            var type = typeof(Root);

            if (nodeDic.ContainsKey(type))
            {
                var np_node = nodeDic[type].Dequeue() as Root;
                np_node?.Init(blackboard, clock, mainNode);

                return np_node;
            }

            return new Root(blackboard, clock, mainNode);
        }

        public void RecycleNode(Node node)
        {
            var type = node.GetType();
            Queue<Node> queue;
            if (nodeDic.ContainsKey(type))
            {
                queue = nodeDic[type];
            }
            else
            {
                queue = new Queue<Node>();
                nodeDic.Add(type, queue);
            }

            queue.Enqueue(node);
        }
    }
}