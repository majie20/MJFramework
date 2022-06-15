using System;
using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class CharacterJumpComponent : Component, IAwake, IUpdateSystem
    {
        private Animator characterAnimator;

        public void Awake()
        {
            this.characterAnimator = this.Entity.Transform.GetComponent<Animator>();
        }

        public override void Dispose()
        {
            characterAnimator = null;
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {

        }

    }
}