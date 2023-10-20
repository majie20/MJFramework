using NPBehave;
using UnityEngine;

namespace Model
{
    [NP_NodeTypeBind(typeof(P_MoveNode))]
    public class NP_MoveNode : Decorator
    {
        private MoveWay way;
        private Vector2 direction;

        public NP_MoveNode() : base("NP_MoveNode")
        {
        }

        public NP_MoveNode(MoveWay way, Vector2 direction, Node decoratee) : base("NP_MoveNode", decoratee)
        {
            this.way = way;
            this.direction = direction;
        }

        public void Init(MoveWay way, Vector2 direction, Node decoratee)
        {
            base.Init(decoratee);
            this.way = way;
            this.direction = direction;
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            var n = node as P_MoveNode;
            Init(n.Way, n.Direction, children[0]);
        }

        protected override void DoStart()
        {
            if (RootNode.Tree is NP_MonsterBehaviorTree monsterBehaviorTree)
            {
                Vector2 dir = Vector2.zero;

                if (way == MoveWay.Normal)
                {
                    dir = this.direction;
                }
                else if (way == MoveWay.Target)
                {
                    MonsterAIComponent monsterAIComponent = monsterBehaviorTree.Component;
                    dir = monsterAIComponent.TargetPoint - (Vector2)RootNode.Tree.Entity.Transform.position;
                }

                RootNode.Tree.Entity.EventSystem.Invoke<E_CharacterMove, MoveType, Vector2>(MoveType.Walking, dir.x > 0 ? Vector2.right : Vector2.left);
            }

            Decoratee.Start();
        }

        protected override void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}