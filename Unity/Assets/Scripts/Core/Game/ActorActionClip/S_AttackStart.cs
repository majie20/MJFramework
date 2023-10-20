using Slate;
using Slate.ActionClips;
using UnityEngine;

namespace Model
{
    public enum AttackWay : byte
    {
        Once,
        Continuous,
    }

    [Category("自定义")]
    public class S_AttackStart : ActorActionClip
    {
        public EntityIdHandle Handle;
        public AttackWay      Way;
        public int            Interval;

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

            Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(Handle.Guid)?.EventSystem.Invoke<E_AttackStart, S_AttackStart>(this);
        }
    }
}