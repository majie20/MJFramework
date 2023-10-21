using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(CharacterComponent))]
    [LifeCycle]
    public class CharacterAttackComponent : Component, IAwake
    {
        private HashSet<long>           _beAttackedMap;
        private int                     _interval;
        private AttackWay               _way;
        private CancellationTokenSource _cts;

        public void Awake()
        {
            _beAttackedMap = new HashSet<long>();

            this.Entity.EventSystem.AddListener<E_CharacterAttack>(this, OnCharacterAttack);
            this.Entity.EventSystem.AddListener<E_AttackBeforeInitialize, AttackWay, int>(this, OnAttackBeforeInitialize);
            this.Entity.EventSystem.AddListener<E_TriggerEnter2D, UnitColliderType, Collider2D>(this, OnTriggerEnter2D);
            this.Entity.EventSystem.AddListener<E_TriggerStay2D, UnitColliderType, Collider2D>(this, OnTriggerStay2D);
        }

        public override void Dispose()
        {
            _beAttackedMap = null;

            if (!(_cts is null))
            {
                AsyncTimerHelper.StopTimer(_cts);
                _cts = null;
            }

            base.Dispose();
        }

        public void OnCharacterAttack()
        {
            this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Attack));
        }

        public void OnAttackBeforeInitialize(AttackWay way, int interval)
        {
            _beAttackedMap.Clear();

            if (!(_cts is null))
            {
                AsyncTimerHelper.StopTimer(_cts);
            }

            _cts = new CancellationTokenSource();
            _way = way;

            if (way == AttackWay.Continuous)
            {
                _interval = interval;
            }
        }

        public void OnTriggerEnter2D(UnitColliderType type, Collider2D collider2D)
        {
            if (type == UnitColliderType.Weapon)
            {
                ColliderHandle2D handle2D = collider2D.GetComponent<ColliderHandle2D>();

                if (handle2D != null && handle2D.Type == UnitColliderType.Body)
                {
                    Attack(collider2D);
                }
            }
        }

        public void OnTriggerStay2D(UnitColliderType type, Collider2D collider2D)
        {
            if (type == UnitColliderType.Weapon)
            {
                ColliderHandle2D handle2D = collider2D.GetComponent<ColliderHandle2D>();

                if (handle2D != null && handle2D.Type == UnitColliderType.Body)
                {
                    Attack(collider2D);
                }
            }
        }

        public void Attack(Collider2D collider2D)
        {
            var guid = collider2D.attachedRigidbody.GetComponent<EntityIdHandle>().Guid;

            if (!_beAttackedMap.Contains(guid))
            {
                _beAttackedMap.Add(guid);

                if (_way == AttackWay.Continuous)
                {
                    AsyncTimerHelper.TimeHandle(() => { _beAttackedMap.Remove(guid); }, _interval, 1, _cts).Forget();
                }

                CharacterComponent characterComponent = Entity.GetComponent<CharacterComponent>();
                Game.Instance.Scene.GetComponent<EntityPoolComponent>().GetEntity(guid).EventSystem.Invoke<E_AttackCompute, MoveDir>(characterComponent.MoveDir);
            }
        }
    }
}