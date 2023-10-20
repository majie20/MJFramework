using UnityEngine;

namespace Model
{
    public class JumpState : BaseState
    {
        private float _delaytime;

        public JumpState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas) : base(manager, interruptType, datas)
        {
            Type = StateMachineType.Jump;
        }

        public override void Enter(int index)
        {
            base.Enter(index);
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();
            characterComponent.IsJump = true;
            _delaytime = 0.1f;

            Manager.Entity.EventSystem.Invoke<E_VelocityChange, VelocityInfo>(new VelocityInfo(Vector2.up * 25, VelocityType.Disposable, VelocitySource.Jump));
            Vec = new Vector2(10, 10);
            this.Move();
        }

        public override void Exit()
        {
            base.Exit();
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();
            characterComponent.IsJump = false;
            Manager.Entity.EventSystem.Invoke<E_VelocityChange, VelocityInfo>(new VelocityInfo(Vector2.zero, VelocityType.Lasting, VelocitySource.Move));
        }

        public override void Update(float tick)
        {
            if (_delaytime <= 0)
            {
                CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();

                if (!characterComponent.IsFloating)
                {
                    Manager.Entity.EventSystem.Invoke<E_CharacterStateMachineEnd, StateMachineToggleInfo, StateMachineType>(new StateMachineToggleInfo(StateMachineType.Idle, 0),
                        StateMachineType.Jump);
                }

                return;
            }

            _delaytime -= tick;

            this.Move();
        }
    }
}