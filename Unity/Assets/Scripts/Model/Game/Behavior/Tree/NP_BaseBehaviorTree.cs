using System;
using NPBehave;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace Model
{
    public class NP_BaseBehaviorTree : System.IDisposable
    {
        private Entity _entity;

        public Entity Entity
        {
            get { return _entity; }
            private set { _entity = value; }
        }

        private Root _root;

        public Root Root
        {
            get { return _root; }
            private set { _root = value; }
        }

        protected BaseGraph _baseGraph;

        protected P_StartNode StartNode;

        public NP_BaseBehaviorTree(BaseGraph graph, Entity entity)
        {
            _baseGraph = graph;
            _baseGraph.OnGraphEnable();
            this.StartNode = graph.GetNodeFirstByType<P_StartNode>();
            this.Entity = entity;
        }

        public virtual void Init()
        {
            Root = CreateRoot();
            Root.SetTree(this);
        }

        public Root CreateRoot()
        {
            return Game.Instance.Scene.GetComponent<NPNodePoolComponent>().HatchRoot(CreateNode(StartNode));
        }

        private Node CreateNode(P_BaseNode pNode)
        {
            Node[] nodes = null;

            if (pNode is P_IfNode)
            {
                nodes = new[] { CreateNode(pNode.GetOutputPort("True").owner as P_BaseNode), CreateNode(pNode.GetOutputPort("False").owner as P_BaseNode) };
            }
            else
            {
                if (pNode.outputPorts.Count > 0)
                {
                    var edges = pNode.outputPorts[0].GetEdges();
                    var count = edges.Count;

                    if (count > 0)
                    {
                        nodes = new Node[count];

                        for (int j = 0; j < count; j++)
                        {
                            nodes[j] = CreateNode(edges[j].inputNode as P_BaseNode);
                        }

                        Array.Sort(nodes, (n1, n2) => n2.Order - n1.Order);
                    }
                }
            }

            return Game.Instance.Scene.GetComponent<NPNodePoolComponent>().HatchNode(pNode, nodes);
        }

        public virtual void Dispose()
        {
            Root.Dispose();
            Game.Instance.Scene.GetComponent<NPNodePoolComponent>().RecycleNode(this.Root);
            this.Root = null;
        }

        public virtual void Start()
        {
            if (Root.CurrentState != Node.State.ACTIVE)
            {
                this.Root.Start();
            }
        }

        public virtual void Stop()
        {
            if (Root.CurrentState == Node.State.ACTIVE)
            {
                this.Root.Stop();
            }
        }

        public virtual void Pause()
        {
            this.Root.Clock.Pause();
        }

        public virtual void Recover()
        {
            this.Root.Clock.Recover();
        }

        #region 判断使用的方法

        public virtual bool JudgeDistanceByTargetPoint()
        {
            return false;
        }

        public virtual bool JudgeIsHurt()
        {
            return false;
        }

        public virtual bool If_JudgeIsInFirstAction()
        {
            return false;
        }

        #endregion

        #region 执行action使用的方法

        public virtual void TogglePatrolPoint()
        {
        }

        public virtual void StayForWhile()
        {
        }

        public virtual void FirstActionExpired()
        {
        }

        #endregion
    }
}