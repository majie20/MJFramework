using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    public enum UnitColliderType : byte
    {
        Body,
        Weapon,
        Observed,
    }

    public class ColliderHandle2D : MonoBehaviour
    {
        public UnitColliderType Type;
        public EntityIdHandle   Handle;

        private void OnCollisionEnter2D(Collision2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_CollisionEnter2D, UnitColliderType, Collision2D>(Type, other);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_CollisionStay2D, UnitColliderType, Collision2D>(Type, other);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_CollisionExit2D, UnitColliderType, Collision2D>(Type, other);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_TriggerEnter2D, UnitColliderType, Collider2D>(Type, other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_TriggerStay2D, UnitColliderType, Collider2D>(Type, other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var guid = Handle.Guid;

            if (guid > 0)
            {
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_TriggerExit2D, UnitColliderType, Collider2D>(Type, other);
            }
        }
    }
}