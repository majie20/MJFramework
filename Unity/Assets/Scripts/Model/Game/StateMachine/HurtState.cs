using UnityEngine;

namespace Model
{
    public class HurtState : BaseState
    {
        private float _delaytime;

        public HurtState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas) : base(manager, interruptType, datas)
        {
            Type = StateMachineType.Hurt;
        }

        public override void Enter(int index)
        {
            base.Enter(index);

            _delaytime = 0.1f;
            //var data = _datas[_curIndex];

            //if (data.IsTemporary)
            //{
            //    Manager.Entity.TimeHandle(() => { Manager.Entity.EventSystem.Invoke<E_CharacterStateMachineEnd, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Idle, 0)); }, (int)(data.Clip.length * 1000 - 200), 1);
            //}

            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();
            Vector2 direction = Vector2.zero;

            if (characterComponent.HurtDir == MoveDir.Left)
            {
                direction = new Vector2(-1, 2).normalized;
            }
            else if (characterComponent.HurtDir == MoveDir.Right)
            {
                direction = new Vector2(1, 2).normalized;
            }

            Manager.Entity.EventSystem.Invoke<E_VelocityChange, VelocityInfo>(new VelocityInfo(direction * 10, VelocityType.Disposable, VelocitySource.Hurt));
        }

        public override void Update(float tick)
        {
            if (_delaytime <= 0)
            {
                CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();

                if (!characterComponent.IsFloating)
                {
                    Manager.Entity.EventSystem.Invoke<E_CharacterStateMachineEnd, StateMachineToggleInfo, StateMachineType>(new StateMachineToggleInfo(StateMachineType.Idle, 0),
                        StateMachineType.Hurt);
                }

                return;
            }

            _delaytime -= tick;
        }
    }
}