using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class CharacterMovementComponent : Component, IAwake
    {
        public void Awake()
        {
            this.Entity.EventSystem.AddListener<E_CharacterMove, MoveType, Vector2>(this, OnCharacterMove);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnCharacterMove(MoveType type, Vector2 vec)
        {
            CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();

            characterComponent.MoveVec = vec;

            //if (characterComponent.MoveType != type|| (type != MoveType.))
            //{
            characterComponent.LastMoveType = characterComponent.MoveType;
            characterComponent.MoveType = type;

            switch (type)
            {
                case MoveType.Walking:
                    this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Walk));

                    break;

                case MoveType.Running:
                    this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Run));

                    break;

                case MoveType.StopMove:
                    this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Idle));

                    break;
            }
            //}
        }
    }
}