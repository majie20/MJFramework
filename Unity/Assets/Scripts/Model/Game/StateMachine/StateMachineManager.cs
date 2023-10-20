using Animancer;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class StateMachineManager
    {
        private Dictionary<StateMachineType, BaseState> _stateMap;

        private Animator           _animator;
        private AnimancerComponent _animancerComponent;
        private AnimationCollector _animationCollector;
        private AnimancerState     _animancerState;

        private Entity _entity;

        public Entity Entity
        {
            get { return _entity; }
        }

        private UnitType _unitType;

        public UnitType UnitType
        {
            get { return _unitType; }
        }

        private BaseState _curState;

        public BaseState CurState
        {
            get { return _curState; }
        }

        //private bool _isExecute;

        public StateMachineManager(Entity entity, UnitType unitType)
        {
            //_isExecute = false;
            _entity = entity;
            _unitType = unitType;

            var rc = this.Entity.GameObject.GetComponent<ReferenceCollector>();
            var renderer = rc.Get<GameObject>("Renderer");
            _animator = renderer.GetComponent<Animator>();
            _animancerComponent = renderer.GetComponent<AnimancerComponent>();
            _animationCollector = renderer.GetComponent<AnimationCollector>();
            _animationCollector.Initialize();

            var infos = UnitValue.StateMachineTypeMap[unitType];
            _stateMap = new Dictionary<StateMachineType, BaseState>(infos.Length);

            for (int i = infos.Length - 1; i >= 0; i--)
            {
                var type = infos[i].Type;

                switch (type)
                {
                    case StateMachineType.Idle:
                        _stateMap.Add(infos[i].Type, new IdleState(this, infos[i].InterruptType, _animationCollector.IdleClips));

                        break;

                    case StateMachineType.Die:
                        break;

                    case StateMachineType.Walk:
                        _stateMap.Add(infos[i].Type, new WalkState(this, infos[i].InterruptType, _animationCollector.WalkClips));

                        break;

                    case StateMachineType.Run:
                        _stateMap.Add(infos[i].Type, new RunState(this, infos[i].InterruptType, _animationCollector.RunClips));

                        break;

                    case StateMachineType.Jump:
                        _stateMap.Add(infos[i].Type, new JumpState(this, infos[i].InterruptType, _animationCollector.JumpClips));

                        break;

                    case StateMachineType.Climb:
                        break;

                    case StateMachineType.Attack:
                        _stateMap.Add(infos[i].Type, new AttackState(this, infos[i].InterruptType, _animationCollector.AttackClips));

                        break;

                    case StateMachineType.Skill:
                        break;

                    case StateMachineType.Hurt:
                        _stateMap.Add(infos[i].Type, new HurtState(this, infos[i].InterruptType, _animationCollector.HurtClips));

                        break;
                }
            }
        }

        public void Update(float tick)
        {
            _curState?.Update(tick);
        }

        public void Play(StateMachineToggleInfo info)
        {
            if (_stateMap.TryGetValue(info.Type, out var state))
            {
                var index = info.Index;

                if (_curState == null || state.InterruptJudge(_curState, index))
                {
                    //if (Entity.GameObject.name == "Slime01")
                    //{
                    //    NLog.Log.Error($"BBBB{(_curState == null ? "None" : _curState.Type.ToString())}=>{state.Type}");
                    //}

                    _curState?.Exit();
                    //_isExecute = true;
                    _curState = state;
                    _curState.Enter(index);
                }

                return;
            }

            NLog.Log.Error($"{Entity.GameObject.name}的状态机内没有此状态!");
        }

        public void Play(AnimationClip clip)
        {
            _animancerState = _animancerComponent.Play(clip);
        }

        public void PlayByEnd(StateMachineToggleInfo info, StateMachineType stateMachineType)
        {
            if (_curState != null && stateMachineType != _curState.Type)
            {
                return;
            }

            if (_stateMap.TryGetValue(info.Type, out var state))
            {
                var index = info.Index;

                //if (Entity.GameObject.name == "Slime01")
                //{
                //    NLog.Log.Error($"BBBB{(_curState == null ? "None" : _curState.Type.ToString())}=>{state.Type}");
                //}

                _curState?.Exit();
                //_isExecute = true;
                _curState = state;
                _curState.Enter(index);

                return;
            }

            NLog.Log.Error($"{Entity.GameObject.name}的状态机内没有此状态!");
        }
    }
}