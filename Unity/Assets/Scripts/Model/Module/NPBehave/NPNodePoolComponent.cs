using NPBehave;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;

namespace Model
{
    public class NPNodePoolComponent : Component, IAwake
    {
        private Dictionary<Type, Queue<Node>> _nodePoolDic;
        private Dictionary<Type, Type>        _nodeTypeBindMap;

        public void Awake()
        {
            _nodePoolDic = new Dictionary<Type, Queue<Node>>();
            _nodeTypeBindMap = new Dictionary<Type, Type>();
            HashSet<Type> list = Game.Instance.LifecycleSystem.Types[typeof(NP_NodeTypeBindAttribute)];

            foreach (var type in list)
            {
                var attributes = type.GetAttributes<NP_NodeTypeBindAttribute>();

                foreach (var attribute in attributes)
                {
                    _nodeTypeBindMap.Add(attribute.Type, type);
                }
            }
        }

        public override void Dispose()
        {
            _nodePoolDic = null;
            _nodeTypeBindMap = null;
            base.Dispose();
        }

        public Node HatchNode(P_BaseNode pNode, params Node[] nodes)
        {
            var type = _nodeTypeBindMap[pNode.GetType()];

            Node node;

            if (_nodePoolDic.ContainsKey(type))
            {
                node = _nodePoolDic[type].Dequeue();
            }
            else
            {
                node = (Node)Activator.CreateInstance(type);
            }

            node.Init(pNode, nodes);
            node.Order = pNode.Order;

            return node;
        }

        public Root HatchRoot(Node mainNode)
        {
            var type = typeof(Root);

            if (_nodePoolDic.ContainsKey(type))
            {
                var root = _nodePoolDic[type].Dequeue() as Root;
                root?.Init(mainNode);

                return root;
            }

            return new Root(mainNode);
        }

        public Root HatchRoot(Blackboard blackboard, Node mainNode)
        {
            var type = typeof(Root);

            if (_nodePoolDic.ContainsKey(type))
            {
                var root = _nodePoolDic[type].Dequeue() as Root;
                root?.Init(blackboard, mainNode);

                return root;
            }

            return new Root(mainNode);
        }

        public Root HatchRoot(Blackboard blackboard, Clock clock, Node mainNode)
        {
            var type = typeof(Root);

            if (_nodePoolDic.ContainsKey(type))
            {
                var root = _nodePoolDic[type].Dequeue() as Root;
                root?.Init(blackboard, clock, mainNode);

                return root;
            }

            return new Root(blackboard, clock, mainNode);
        }

        public void RecycleNode(Node node)
        {
            var type = node.GetType();
            Queue<Node> queue;

            if (_nodePoolDic.ContainsKey(type))
            {
                queue = _nodePoolDic[type];
            }
            else
            {
                queue = new Queue<Node>();
                _nodePoolDic.Add(type, queue);
            }

            queue.Enqueue(node);
        }
    }
}