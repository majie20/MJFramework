namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(CharacterComponent))]
    public class MonsterComponent : Component, IAwake, IStartSystem
    {
        public void Awake()
        {
            this.Entity.EventSystem.AddListener<E_UnitDataInitialize, int>(this, OnUnitDataInitialize);
        }

        public void Start()
        {
            //this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Idle));
        }

        private void OnUnitDataInitialize(int code)
        {
            var unitComponent = Entity.GetComponent<UnitComponent>();
            unitComponent.TypeCode = code;
            GameConfigDataComponent gameConfigDataComponent = Game.Instance.Scene.GetComponent<GameConfigDataComponent>();
            NumericComponent numericComponent = Entity.GetComponent<NumericComponent>();
            var monsterData = gameConfigDataComponent.JsonTables.TbMonsterData.Get(1, code);
            numericComponent.Set(NumericType.HpBase, monsterData.Hp);
            numericComponent.Set(NumericType.SpeedBase, monsterData.BaseMoveSpeed);
            numericComponent.Set(NumericType.LevelBase, monsterData.Level);
        }
    }
}