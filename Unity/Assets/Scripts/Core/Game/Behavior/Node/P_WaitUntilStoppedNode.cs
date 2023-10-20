using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/WaitUntilStopped")]
    public class P_WaitUntilStoppedNode : P_BaseNode
    {
        [Input]
        public Empty In;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}