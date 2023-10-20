using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using Model;

namespace NPBehave
{
    [NP_NodeTypeBind(typeof(P_SelectorNode))]
    public class Selector : Composite
    {
        private int currentIndex = -1;

        public Selector() : base("Selector")
        {
        }

        public Selector(params Node[] children) : base("Selector", children)
        {
        }

        public override void Init(Model.P_BaseNode node, Node[] children)
        {
            Init(children);
        }

        protected override void DoStart()
        {
            for (int i = Children.Length - 1; i >= 0; i--)
            {
                Assert.AreEqual(Children[i].CurrentState, State.INACTIVE);
            }

            currentIndex = -1;

            ProcessChildren();
        }

        protected override void DoStop()
        {
            Children[currentIndex].Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (result)
            {
                Stopped(true);
            }
            else
            {
                ProcessChildren();
            }
        }

        private void ProcessChildren()
        {
            if (++currentIndex < Children.Length)
            {
                if (IsStopRequested)
                {
                    Stopped(false);
                }
                else
                {
                    Children[currentIndex].Start();
                }
            }
            else
            {
                Stopped(false);
            }
        }

        public override void StopLowerPriorityChildrenForChild(Node abortForChild, bool immediateRestart)
        {
            int indexForChild = 0;
            bool found = false;

            for (int i = 0; i < Children.Length; i++)
            {
                var currentChild = Children[i];

                if (currentChild == abortForChild)
                {
                    found = true;
                }
                else if (!found)
                {
                    indexForChild++;
                }
                else if (found && currentChild.IsActive)
                {
                    if (immediateRestart)
                    {
                        currentIndex = indexForChild - 1;
                    }
                    else
                    {
                        currentIndex = Children.Length;
                    }

                    currentChild.Stop();

                    break;
                }
            }
        }

        override public string ToString()
        {
            return base.ToString() + "[" + this.currentIndex + "]";
        }
    }
}