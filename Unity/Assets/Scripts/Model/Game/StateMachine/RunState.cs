namespace Model
{
    public class RunState : WalkState
    {
        public RunState(StateMachineManager manager, StateMachineType interruptType, AnimationData[] datas) : base(manager, interruptType, datas)
        {
            Type = StateMachineType.Run;
        }
    }
}