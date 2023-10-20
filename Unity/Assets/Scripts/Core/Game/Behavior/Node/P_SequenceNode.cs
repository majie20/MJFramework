using System.Collections.Generic;
using GraphProcessor;

namespace Model
{
    [System.Serializable, NodeMenuItem("基础/Sequence")]
    public class P_SequenceNode : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        protected override void Process()
        {
            TryGetInputValue(nameof(In), ref In);
        }
    }
}