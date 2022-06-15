using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class PlayerCtrlComponent : Component, IAwake, IUpdateSystem
    {
        private bool isMove;
        private bool isRun;

        public void Awake()
        {
            isMove = false;
            isRun = false;

            Game.Instance.EventSystem.AddListener<E_ButtonPressedFirstTimeRun>(this, OnButtonPressedFirstTimeRun);
            Game.Instance.EventSystem.AddListener<E_ButtonReleasedRun>(this, OnButtonReleasedRun);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
            var move = Game.Instance.GGetComponent<VirtualJoystickViewComponent>().GetPrimaryMovement;
            var sqrt = move.sqrMagnitude;

            if (sqrt > 0)
            {
                if (!isMove)
                {
                    Game.Instance.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineType>(StateMachineType.Walk);
                    isMove = true;
                }
            }
            else
            {
                if (isMove)
                {
                    isMove = false;
                    Game.Instance.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineType>(StateMachineType.Idle);
                }
            }

            if (isMove)
            {
                if (isRun)
                {
                    Game.Instance.EventSystem.Invoke<E_CharacterRun, Vector2, float>(move, tick);
                }
                else
                {
                    Game.Instance.EventSystem.Invoke<E_CharacterWalk, Vector2, float>(move, tick);
                }
            }
        }

        private void OnButtonPressedFirstTimeRun()
        {
            Game.Instance.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineType>(StateMachineType.Run);
            isRun = true;
        }

        private void OnButtonReleasedRun()
        {
            if (isMove)
            {
                Game.Instance.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineType>(StateMachineType.Walk);
            }
            else
            {
                Game.Instance.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineType>(StateMachineType.Idle);
            }
            isRun = false;
        }
    }
}