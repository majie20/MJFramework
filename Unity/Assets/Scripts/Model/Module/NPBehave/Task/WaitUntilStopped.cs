using Model;

namespace NPBehave
{
    [NP_NodeTypeBind(typeof(P_WaitUntilStoppedNode))]
    public class WaitUntilStopped : Task
    {
        private bool sucessWhenStopped;

        public WaitUntilStopped() : base("WaitUntilStopped")
        {
        }

        public WaitUntilStopped(bool sucessWhenStopped = false) : base("WaitUntilStopped")
        {
            this.sucessWhenStopped = sucessWhenStopped;
        }

        protected override void DoStop()
        {
            this.Stopped(sucessWhenStopped);
        }
    }
}