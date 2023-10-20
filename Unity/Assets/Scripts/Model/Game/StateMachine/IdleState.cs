using UnityEngine;

namespace Model
{
    public class IdleState : BaseState
    {
        public IdleState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas) : base(manager, interruptType, datas)
        {
            Type = StateMachineType.Idle;
        }

        public override bool ConditionJudge()
        {
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();

            if (characterComponent.IsAttack || characterComponent.IsJump)
            {
                return false;
            }

            return true;
        }

        public override void Enter(int index)
        {
            base.Enter(index);
            Vec = new Vector2(10, 10);
            this.Move();
        }

        public override void Exit()
        {
            base.Exit();
            Manager.Entity.EventSystem.Invoke<E_VelocityChange, VelocityInfo>(new VelocityInfo(Vector2.zero, VelocityType.Lasting, VelocitySource.Move));
        }

        public override void Update(float tick)
        {
            this.Move();
        }
    }
}