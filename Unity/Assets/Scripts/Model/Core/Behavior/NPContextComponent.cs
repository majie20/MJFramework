using UnityEngine;
using System.Collections.Generic;
using NPBehave;

namespace Model
{
    [LifeCycle]
    public class NPContextComponent : Component, IAwake, IUpdateSystem
    {
        private Dictionary<string, Blackboard> blackboards;
        private Queue<Blackboard> blackboardqQueue;

        private Clock clock;

        public void Awake()
        {
            blackboards = new Dictionary<string, Blackboard>();
            blackboardqQueue = new Queue<Blackboard>();
            clock = new Clock();
        }

        public override void Dispose()
        {
            Entity = null;
            blackboards = null;
            blackboardqQueue = null;
            clock = null;
        }
        public void OnUpdate(float tick)
        {
            clock.Update(tick);
        }

        public Clock GetClock()
        {
            return this.clock;
        }

        public Blackboard GetSharedBlackboard(string key)
        {
            if (!this.blackboards.ContainsKey(key))
            {
                this.blackboards.Add(key, new Blackboard(this.clock));
            }
            return this.blackboards[key];
        }

        public Blackboard HatchBlackboard(Clock clock, Blackboard parent = null)
        {
            Blackboard blackboard;
            if (blackboardqQueue.Count > 0)
            {
                blackboard = blackboardqQueue.Dequeue();
                blackboard.Init(parent, clock);
            }
            else
            {
                blackboard = new Blackboard(parent, clock);
            }

            return blackboard;
        }

        public void RecycleBlackboard(Blackboard blackboard)
        {
            blackboardqQueue.Enqueue(blackboard);
        }
    }
}