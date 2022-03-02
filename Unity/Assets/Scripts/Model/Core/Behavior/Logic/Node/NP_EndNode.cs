using NPBehave;

namespace Model
{
    public class NP_EndNode : Task
    {
        public NP_EndNode() : base("NP_EndNode")
        {
        }

        protected override void DoStart()
        {
            RootNode.Stop();
            Stopped(true);
        }
    }
}