using System;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class CharacterJumpComponent : Component, IAwake, IUpdateSystem
    {
        private Animator characterAnimator;
        private CharacterComponent characterComponent;

        public void Awake()
        {
            var characterComponent = this.Entity.GetComponent<CharacterComponent>();

            this.characterComponent = characterComponent ?? throw new NotImplementedException();
            this.characterAnimator = characterComponent.CharacterAnimator;
        }

        public override void Dispose()
        {
            characterComponent = null;
            characterAnimator = null;
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {

        }

    }
}