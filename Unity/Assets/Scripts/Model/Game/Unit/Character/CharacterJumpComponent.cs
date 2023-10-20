using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class CharacterJumpComponent : Component, IAwake
    {
        public void Awake()
        {
            this.Entity.EventSystem.AddListener<E_CharacterJump>(this, OnCharacterJump);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnCharacterJump()
        {
            this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Jump));
        }
    }
}