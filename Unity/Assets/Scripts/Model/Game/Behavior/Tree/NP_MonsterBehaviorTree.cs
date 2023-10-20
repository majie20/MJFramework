using GraphProcessor;
using UnityEngine;

namespace Model
{
    public class NP_MonsterBehaviorTree : NP_BaseBehaviorTree
    {
        public MonsterAIComponent Component;

        public NP_MonsterBehaviorTree(MonsterAIComponent component, BaseGraph graph, Entity entity) : base(graph, entity)
        {
            Component = component;
        }

        public override void Init()
        {
            base.Init();

            Component.InitAI();
        }

        public override bool JudgeDistanceByTargetPoint()
        {
            var position = (Vector2)Entity.Transform.position;
            var distance = MathHelper.DistanceByPointToSeg2D(Component.TargetPoint, Entity.Transform.position, Component.LastPosition);
            Component.LastPosition = position;

            return distance <= 0.1f;
        }

        public override bool JudgeIsHurt()
        {
            return Entity.GetComponent<StateDataComponent>().GetStateAsBool(StateType.IsHurt);
        }

        public override bool If_JudgeIsInFirstAction()
        {
            return Entity.GetComponent<StateDataComponent>().GetStateAsBool(StateType.IsInFirstAction);
        }

        public override void TogglePatrolPoint()
        {
            Component.TargetPoint = Component.BornHandle.PatrolPointList[Component.PatrolPointListIndex].position;

            if (Component.PatrolPointListIndex + 1 >= Component.BornHandle.PatrolPointList.Length)
            {
                Component.PatrolPointListIndex = 0;
            }
            else
            {
                Component.PatrolPointListIndex++;
            }
        }

        public override void StayForWhile()
        {
            Entity.EventSystem.Invoke<E_CharacterMove, MoveType, Vector2>(MoveType.StopMove, Vector2.zero);
        }

        public override void FirstActionExpired()
        {
            Entity.GetComponent<StateDataComponent>().Set(StateType.IsInFirstAction, false);
        }
    }
}