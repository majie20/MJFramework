using NPBehave;
using UnityEngine;

namespace Model
{
    public class NP_LogNode : Decorator
    {
        private string Content;

        public NP_LogNode(string content, Node decoratee) : base("NP_LogNode", decoratee)
        {
            this.Content = content;
        }

        public void Init(string content, Node decoratee)
        {
            base.Init(decoratee);
            this.Content = content;
        }

        protected override void DoStart()
        {
            NLog.Log.Debug(this.Content); // MDEBUG:
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