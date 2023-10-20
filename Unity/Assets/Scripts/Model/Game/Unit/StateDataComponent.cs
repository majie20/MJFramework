using System.Collections.Generic;

namespace Model
{
    public enum StateType
    {
        IsHurt,
        IsInFirstAction
    }

    [LifeCycle]
    [ComponentOf(typeof(UnitComponent))]
    public class StateDataComponent : Component, IAwake
    {
        private Dictionary<StateType, int> StateDataDic;

        public void Awake()
        {
            StateDataDic = new Dictionary<StateType, int>();
        }

        public override void Dispose()
        {
            StateDataDic = null;
            base.Dispose();
        }

        public int GetStateAsInt(StateType stateType)
        {
            return GetByKey(stateType);
        }

        public bool GetStateAsBool(StateType stateType)
        {
            return GetByKey(stateType) != 0;
        }

        public void Set(StateType stateType, bool value)
        {
            this[stateType] = value ? 1 : 0;
        }

        public void Set(StateType stateType, int value)
        {
            this[stateType] = value;
        }

        public int this[StateType stateType]
        {
            get => this.GetByKey(stateType);
            set => StateDataDic[stateType] = value;
        }

        private int GetByKey(StateType key)
        {
            this.StateDataDic.TryGetValue(key, out var value);

            return value;
        }
    }
}