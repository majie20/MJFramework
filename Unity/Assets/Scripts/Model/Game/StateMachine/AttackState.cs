namespace Model
{
    public class AttackState : BaseState
    {
        public AttackState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas) : base(manager, interruptType, datas)
        {
            Type = StateMachineType.Attack;
        }

        public override void Enter(int index)
        {
            base.Enter(index);
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();
            characterComponent.IsAttack = true;
        }

        public override void Exit()
        {
            base.Exit();
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();
            characterComponent.IsAttack = false;

            characterComponent.IsCanMove = true;
            //NLog.Log.Error($"{Manager.Entity.GameObject.name}.characterComponent.IsAttack:{characterComponent.IsAttack}");
        }

        public override void Update(float tick)
        {
            CharacterComponent characterComponent = Manager.Entity.GetComponent<CharacterComponent>();

            characterComponent.IsCanMove = characterComponent.IsFloating;
        }
    }
}