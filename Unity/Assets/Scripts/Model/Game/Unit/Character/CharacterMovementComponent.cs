using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class CharacterMovementComponent : Component, IAwake
    {
        private Animator characterAnimator;

        public void Awake()
        {
            var characterComponent = this.Entity.GetComponent<CharacterComponent>();

            this.characterAnimator = this.Entity.Transform.GetComponent<Animator>();

            Game.Instance.EventSystem.AddListener<E_CharacterWalk, Vector2, float>(this, OnCharacterWalk);
            Game.Instance.EventSystem.AddListener<E_CharacterRun, Vector2, float>(this, OnCharacterRun);
            Game.Instance.EventSystem.AddListener<E_CharacterStateMachineToggle, StateMachineType>(this, OnCharacterStateMachineToggle);
        }

        public override void Dispose()
        {
            characterAnimator = null;
            base.Dispose();
        }

        public void OnCharacterWalk(Vector2 vec, float tick)
        {
            CharacterComponent characterComponent = this.Entity.GetComponent<CharacterComponent>();
            this.Entity.Transform.position += new Vector3(vec.x, vec.y, 0) * characterComponent.GetCharacterBaseData().moveSpeed * tick;
        }

        public void OnCharacterRun(Vector2 vec, float tick)
        {
            CharacterComponent characterComponent = this.Entity.GetComponent<CharacterComponent>();
            this.Entity.Transform.position += new Vector3(vec.x, vec.y, 0) * characterComponent.GetCharacterBaseData().runSpeed * tick;
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