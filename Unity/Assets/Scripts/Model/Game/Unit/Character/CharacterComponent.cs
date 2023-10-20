using System;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(UnitComponent))]
    public class CharacterComponent : Component, IAwake, IStartSystem, ILateUpdateSystem
    {
        private bool _isAttack;

        public bool IsAttack
        {
            set
            {
                //NLog.Log.Error($"{Entity.GameObject.name}.IsAttack:{value}");
                _isAttack = value;
            }
            get { return _isAttack; }
        }

        private bool _isJump;

        public bool IsJump
        {
            set { _isJump = value; }
            get { return _isJump; }
        }

        private bool _isCanMove;

        public bool IsCanMove
        {
            set { _isCanMove = value; }
            get { return _isCanMove; }
        }

        private MoveDir _moveDir;

        public MoveDir MoveDir
        {
            set { _moveDir = value; }
            get { return _moveDir; }
        }

        private MoveDir _hurtDir;

        public MoveDir HurtDir
        {
            set { _hurtDir = value; }
            get { return _hurtDir; }
        }

        private bool _isFloating;

        public bool IsFloating
        {
            set { _isFloating = value; }
            get { return _isFloating; }
        }

        public MoveType MoveType;
        public MoveType LastMoveType;
        public Vector2  MoveVec;

        public void Awake()
        {
            IsCanMove = true;
            HurtDir = MoveDir.None;
            MoveDir = MoveDir.None;
            MoveType = MoveType.StopMove;
            LastMoveType = MoveType.StopMove;
            MoveVec = Vector2.zero;

            this.Entity.EventSystem.AddListener<E_AttackCompute, MoveDir>(this, OnAttackCompute);
            this.Entity.EventSystem.AddListener<E_SwitchCharacterDir, MoveDir>(this, OnSwitchCharacterDir);
        }

        public void Start()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnAttackCompute(MoveDir dir)
        {
            //NLog.Log.Error($"{Entity.GameObject.name}被攻击！");
            HurtDir = dir;

            this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Hurt));
        }

        private void OnSwitchCharacterDir(MoveDir dir)
        {
            if (MoveDir == dir)
                return;
            MoveDir = dir;
            this.Entity.Transform.localScale = dir == MoveDir.Left ? new Vector3(-1, 1, 1) : Vector3.one;
        }

        public void OnLateUpdate()
        {
            if (IsFloating)
            {
                MoveType = MoveType.StopMove;
            }
        }
    }
}