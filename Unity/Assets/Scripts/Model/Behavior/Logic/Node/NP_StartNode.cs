using NPBehave;

namespace Model
{
    public class NP_StartNode : Decorator
    {
        public NP_StartNode(Node decoratee) : base("NP_StartNode", decoratee)
        {
        }

        protected override void DoStart()
        {
            Decoratee.Start();
        }

        protected override void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}