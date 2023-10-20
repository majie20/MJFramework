using NPBehave;
using System.Collections.Generic;
using M.Algorithm;

namespace Model
{
    [LifeCycle]
    public class NPContextComponent : Component, IAwake, IUpdateSystem
    {
        //private Dictionary<string, Blackboard> blackboards;
        private Queue<Blackboard>       _blackboardqQueue;
        private Queue<Clock>            _clockQueue;
        private StaticLinkedList<Clock> _clocks;

        public void Awake()
        {
            //blackboards = new Dictionary<string, Blackboard>();
            _blackboardqQueue = new Queue<Blackboard>();
            _clockQueue = new Queue<Clock>();
            _clocks = new StaticLinkedList<Clock>();
        }

        public override void Dispose()
        {
            //blackboards = null;
            _blackboardqQueue = null;
            _clockQueue = null;
            _clocks = null;
            base.Dispose();
        }

        [FunctionSort(FunctionLayer.Low)]
        public void OnUpdate(float tick)
        {
            var clock = _clocks[1];

            while (clock.cur != 0)
            {
                clock = _clocks[clock.cur];
                clock.element.Update(tick);
            }
        }

        //public Blackboard GetSharedBlackboard(string key)
        //{
        //    if (!this.blackboards.ContainsKey(key))
        //    {
        //        this.blackboards.Add(key, new Blackboard(this.clock));
        //    }
        //    return this.blackboards[key];
        //}

        public Blackboard HatchBlackboard(Blackboard parent = null)
        {
            var clock = HatchClock();
            Blackboard blackboard;

            if (_blackboardqQueue.Count > 0)
            {
                blackboard = _blackboardqQueue.Dequeue();
                blackboard.Init(parent, clock);
            }
            else
            {
                blackboard = new Blackboard(parent, clock);
            }

            return blackboard;
        }

        public Clock HatchClock()
        {
            Clock clock;

            if (_clockQueue.Count > 0)
            {
                clock = _clockQueue.Dequeue();
                clock.Init();
            }
            else
            {
                clock = new Clock();
            }

            if (!_clocks.Contains(clock))
            {
                _clocks.Add(clock);
            }

            return clock;
        }

        public void RecycleBlackboard(Blackboard blackboard)
        {
            _blackboardqQueue.Enqueue(blackboard);
        }

        public void RecycleClock(Clock clock)
        {
            _clockQueue.Enqueue(clock);
            _clocks.Remove(clock);
        }
    }
}