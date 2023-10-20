using UnityEngine;

namespace Model
{
    [ComponentOf(typeof(MonsterComponent))]
    [LifeCycle]
    public class MonsterAIComponent : Component, IAwake, IStartSystem
    {
        private NP_MonsterBehaviorTree _behaviorTree;
        private MonsterBornHandle      _bornHandle;

        public MonsterBornHandle BornHandle
        {
            get { return _bornHandle; }
        }

        private MonsterBaseDataSettings _settings;

        public Vector2          TargetPoint;
        public int              PatrolPointListIndex;
        public Vector2          LastPosition;
        public bool             IsHurt;
        public StateMachineType LastStateMachineType;

        public void Awake()
        {
            this.Entity.EventSystem.AddListener<E_UnitBehaviorInitialize, UnitBornHandle>(this, OnUnitBehaviorInitialize);
            this.Entity.EventSystem.AddListener<E_AttackCompute, MoveDir>(this, OnAttackCompute);
            this.Entity.EventSystem.AddListener<E_CharacterStateMachineEnd, StateMachineToggleInfo, StateMachineType>(this, OnCharacterStateMachineEnd);
        }

        public void Start()
        {
            LastPosition = Entity.Transform.position;
            Entity.GetComponent<StateDataComponent>().Set(StateType.IsHurt, false);
        }

        public override void Dispose()
        {
            if (!(_behaviorTree is null))
            {
                _behaviorTree.Stop();
                _behaviorTree.Dispose();
                _behaviorTree = null;
            }

            _settings = null;
            _bornHandle = null;

            base.Dispose();
        }

        private void OnUnitBehaviorInitialize(UnitBornHandle bornHandle)
        {
            _bornHandle = bornHandle as MonsterBornHandle;
            _settings = bornHandle.UnitBaseDataSettings as MonsterBaseDataSettings;

            _behaviorTree = new NP_MonsterBehaviorTree(this, _settings.BehaviorMapConfig, Entity);
            InitAI();
            _behaviorTree.Init();
            _behaviorTree.Start();
        }

        public void OnAttackCompute(MoveDir dir)
        {
            //_behaviorTree.Stop();
            Entity.GetComponent<StateDataComponent>().Set(StateType.IsHurt, true);
        }

        private void OnCharacterStateMachineEnd(StateMachineToggleInfo info, StateMachineType stateMachineType)
        {
            LastStateMachineType = stateMachineType;

            if (stateMachineType == StateMachineType.Hurt)
            {
                Entity.GetComponent<StateDataComponent>().Set(StateType.IsHurt, false);
                Entity.GetComponent<StateDataComponent>().Set(StateType.IsInFirstAction, true);
            }
        }

        public void InitAI()
        {
            if (BornHandle.IsPatrol)
            {
                PatrolPointListIndex = 0;
            }
            else
            {
            }
        }
    }
}