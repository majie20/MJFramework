using System;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class CharacterMovementComponent : Component, IAwake
    {
        private Animator characterAnimator;
        private CharacterComponent characterComponent;

        public void Awake()
        {
            var characterComponent = this.Entity.GetComponent<CharacterComponent>();

            this.characterComponent = characterComponent ?? throw new NotImplementedException();
            this.characterAnimator = characterComponent.CharacterAnimator;

            Game.Instance.EventSystem.AddListener<E_CharacterWalk, Vector2, float>(this, OnCharacterWalk);
            Game.Instance.EventSystem.AddListener<E_CharacterRun, Vector2, float>(this, OnCharacterRun);
            Game.Instance.EventSystem.AddListener<E_CharacterStateMachineToggle, StateMachineType>(this, OnCharacterStateMachineToggle);
        }

        public override void Dispose()
        {
            characterComponent = null;
            characterAnimator = null;
            base.Dispose();
        }

        public void OnCharacterWalk(Vector2 vec, float tick)
        {
            this.Entity.Transform.position += new Vector3(vec.x, vec.y, 0) * this.characterComponent.GetCharacterBaseData().moveSpeed * tick;
        }

        public void OnCharacterRun(Vector2 vec, float tick)
        {
            this.Entity.Transform.position += new Vector3(vec.x, vec.y, 0) * this.characterComponent.GetCharacterBaseData().runSpeed * tick;
        }

        public void OnCharacterStateMachineToggle(StateMachineType type)
        {
            switch (type)
            {
                case StateMachineType.Idle:
                    characterAnimator.SetBool("Walking", false);
                    break;

                case StateMachineType.Walk:
                    characterAnimator.SetBool("Walking", true);
                    break;

                case StateMachineType.Run:
                    characterAnimator.SetBool("Walking", true);
                    break;

                case StateMachineType.Jump:
                    break;

                case StateMachineType.Die:
                    break;
            }
        }
    }
}