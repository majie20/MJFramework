using UnityEngine;

namespace Model
{
    [LifeCycle]
    [ComponentOf(typeof(CharacterComponent))]
    public class MainCharacterComponent : Component, IAwake, IStartSystem
    {
        public void Awake()
        {
            //this.Entity.GameObject.name = "player";
            this.Entity.EventSystem.AddListener<E_UnitDataInitialize, int>(this, OnUnitDataInitialize);
        }

        public void Start()
        {
            this.Entity.EventSystem.Invoke<E_SwitchCharacterDir, MoveDir>(MoveDir.Right);
            this.Entity.EventSystem.Invoke<E_CharacterStateMachineToggle, StateMachineToggleInfo>(new StateMachineToggleInfo(StateMachineType.Idle));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void OnUnitDataInitialize(int code)
        {
            var unitComponent = Entity.GetComponent<UnitComponent>();
            unitComponent.TypeCode = code;
            GameConfigDataComponent gameConfigDataComponent = Game.Instance.Scene.GetComponent<GameConfigDataComponent>();
            NumericComponent numericComponent = Entity.GetComponent<NumericComponent>();
            var roleData = gameConfigDataComponent.JsonTables.TbRoleData.Get(1, code);
            numericComponent.Set(NumericType.HpBase, roleData.Hp);
            numericComponent.Set(NumericType.SpeedBase, roleData.BaseMoveSpeed);
            //numericComponent.Set(NumericType.SpeedBase, 1);
            numericComponent.Set(NumericType.LevelBase, roleData.Level);
        }
    }
}