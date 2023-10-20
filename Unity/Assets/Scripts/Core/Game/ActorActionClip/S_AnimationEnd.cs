using System;
using Slate;
using Slate.ActionClips;
using UnityEngine;

namespace Model
{
    [Flags]
    public enum StateMachineType
    {
        Idle   = 1,
        Die    = 2,
        Walk   = 4,
        Run    = 8,
        Jump   = 16,
        Climb  = 32,
        Attack = 64,
        Skill  = 128,
        Hurt   = 256,
    }

    [Category("自定义")]
    public class S_AnimationEnd : ActorActionClip
    {
        public EntityIdHandle   Handle;
        public StateMachineType OwnedType;
        public StateMachineType Type;
        public int              Index;

        protected override void OnEnter()
        {
            //if (!root.isActive)
            //{
            //    return;
            //}
            if (!Application.isPlaying)
            {
                return;
            }

            Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(Handle.Guid)?.EventSystem.Invoke<E_AnimationEnd, S_AnimationEnd>(this);
        }
    }
}