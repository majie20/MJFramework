namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(UnitComponent))]
    public class AnimatorComponent : Component, IAwake, IUpdateSystem
    {
        private StateMachineManager _manager;

        public void Awake()
        {
            UnitComponent unitComponent = this.Entity.GetComponent<UnitComponent>();
            _manager = new StateMachineManager(this.Entity, unitComponent.Type);

            this.Entity.EventSystem.AddListener<E_CharacterStateMachineToggle, StateMachineToggleInfo>(this, OnCharacterStateMachineToggle);
            this.Entity.EventSystem.AddListener<E_CharacterStateMachineEnd, StateMachineToggleInfo, StateMachineType>(this, OnCharacterStateMachineEnd);
            this.Entity.EventSystem.AddListener<E_AnimationEnd, S_AnimationEnd>(this, OnAnimationEnd);
            this.Entity.EventSystem.AddListener<E_AttackStart, S_AttackStart>(this, OnAttackStart);
        }

        public override void Dispose()
        {
            _manager = null;
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
            _manager.Update(tick);
        }

        private void OnCharacterStateMachineToggle(StateMachineToggleInfo info)
        {
            _manager.Play(info);
        }

        private void OnCharacterStateMachineEnd(StateMachineToggleInfo info, StateMachineType stateMachineType)
        {
            _manager.PlayByEnd(info, stateMachineType);
        }

        private void OnAnimationEnd(S_AnimationEnd sAnimationEnd)
        {
            Entity.EventSystem.Invoke<E_CharacterStateMachineEnd, StateMachineToggleInfo, StateMachineType>(new StateMachineToggleInfo(sAnimationEnd.Type, sAnimationEnd.Index),
                sAnimationEnd.OwnedType);
        }

        private void OnAttackStart(S_AttackStart sAttackStart)
        {
            Entity.EventSystem.Invoke<E_AttackBeforeInitialize, AttackWay, int>(sAttackStart.Way, sAttackStart.Interval);
        }
    }
}