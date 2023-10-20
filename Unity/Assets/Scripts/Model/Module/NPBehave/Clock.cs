using System.Collections.Generic;
using M.Algorithm;
using UnityEngine.Assertions;

namespace NPBehave
{
    public class Clock : System.IDisposable
    {
        public class Timer
        {
            public double scheduledTime  = 0f;
            public int    repeat         = 0;
            public bool   used           = false;
            public double delay          = 0f;
            public float  randomVariance = 0.0f;

            public void ScheduleAbsoluteTime(double elapsedTime)
            {
                scheduledTime = elapsedTime + delay - randomVariance * 0.5f + randomVariance * UnityEngine.Random.value;
            }
        }

        public struct TimerContainer
        {
            public bool Equals(TimerContainer other)
            {
                return Equals(Action, other.Action) && Equals(Timer, other.Timer);
            }

            public override bool Equals(object obj)
            {
                return obj is TimerContainer other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Action != null ? Action.GetHashCode() : 0) * 397) ^ (Timer != null ? Timer.GetHashCode() : 0);
                }
            }

            public System.Action Action;
            public Timer         Timer;

            public static bool operator ==(TimerContainer left, TimerContainer right)
            {
                return left.Timer == right.Timer && left.Action == right.Action;
            }

            public static bool operator !=(TimerContainer left, TimerContainer right)
            {
                return left.Timer != right.Timer || left.Action != right.Action;
            }
        }

        private static int         currentTimerPoolIndex = 0;
        private static List<Timer> timerPool             = new List<Timer>();

        private StaticLinkedListDictionary<object, System.Action>  updateObservers = new StaticLinkedListDictionary<object, System.Action>(0, 5);
        private StaticLinkedListDictionary<object, TimerContainer> timers          = new StaticLinkedListDictionary<object, TimerContainer>(0, 5);
        private List<System.Action>                                addObservers    = new List<System.Action>();
        private List<object>                                       removeObservers = new List<object>();
        private List<object>                                       removeTimers    = new List<object>();

        private double elapsedTime = 0f;
        private bool   isInUpdate;
        private bool   isPause;

        public Clock()
        {
            Init();
        }

        public void Init()
        {
            elapsedTime = 0f;
            isPause = false;
            isInUpdate = false;
        }

        public void Dispose()
        {
            Reset();
        }

        private void Reset()
        {
            updateObservers.Clear();
            timers.Clear();
            removeObservers.Clear();
            removeTimers.Clear();
            addObservers.Clear();
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Recover()
        {
            isPause = false;
        }

        /// <summary>Register a timer function</summary>
        /// <param name="time">time in seconds</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float time, int repeat, System.Action action)
        {
            AddTimer(time, 0f, repeat, action);
        }

        /// <summary>Register a timer function with random variance</summary>
        /// <param name="time">time in seconds</param>
        /// <param name="randomVariance">deviate from time on a random basis</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float delay, float randomVariance, int repeat, System.Action action)
        {
            var node = action.Target;

            if (!this.timers.TryGetValue(node, out var container))
            {
                container = new TimerContainer() { Action = action, Timer = getTimerFromPool() };
                timers.Add(node, container);
            }

            Timer timer = container.Timer;

            if (isInUpdate)
            {
                removeTimers.Remove(node);
            }

            Assert.IsTrue(timer.used);
            timer.delay = delay;
            timer.randomVariance = randomVariance;
            timer.repeat = repeat;
            timer.ScheduleAbsoluteTime(elapsedTime);
        }

        public void RemoveTimer(object node)
        {
            if (isInUpdate)
            {
                if (this.timers.ContainsKey(node))
                {
                    removeTimers.Add(node);
                }
            }
            else
            {
                if (this.timers.TryGetValue(node, out var container))
                {
                    container.Timer.used = false;
                    this.timers.Remove(node);
                }
            }
        }

        public bool HasTimer(object node)
        {
            if (isInUpdate)
            {
                if (this.removeTimers.Contains(node))
                {
                    return false;
                }
                else
                {
                    return this.timers.ContainsKey(node);
                }
            }

            return this.timers.ContainsKey(node);
        }

        /// <summary>Register a function that is called every frame</summary>
        /// <param name="action">function to invoke</param>
        public void AddUpdateObserver(System.Action action)
        {
            var node = action.Target;

            if (isInUpdate)
            {
                if (!addObservers.Contains(action))
                {
                    addObservers.Add(action);
                }
            }
            else
            {
                if (!updateObservers.ContainsKey(node))
                {
                    updateObservers.Add(node, action);
                }
            }
        }

        public void RemoveUpdateObserver(object node)
        {
            if (isInUpdate)
            {
                if (this.updateObservers.ContainsKey(node))
                {
                    this.removeObservers.Add(node);
                }
            }
            else
            {
                this.updateObservers.Remove(node);
            }
        }

        public bool HasUpdateObserver(System.Action action)
        {
            var node = action.Target;

            if (isInUpdate)
            {
                if (this.removeObservers.Contains(node))
                {
                    return addObservers.Contains(action);
                }
            }

            return this.updateObservers.ContainsKey(node);
        }

        public void Update(float deltaTime)
        {
            if (isPause)
            {
                return;
            }

            this.elapsedTime += deltaTime;

            this.isInUpdate = true;

            var e1 = updateObservers.GetElement(1);

            while (e1.right != 0)
            {
                e1 = updateObservers.GetElement(e1.right);

                if (!removeObservers.Contains(e1.element.Target))
                {
                    e1.element.Invoke();
                }
            }

            var e2 = timers.GetElement(1);

            while (e2.right != 0)
            {
                e2 = timers.GetElement(e2.right);

                if (!removeTimers.Contains(e2.element.Action.Target))
                {
                    var action = e2.element.Action;
                    var timer = e2.element.Timer;

                    if (timer.scheduledTime <= this.elapsedTime)
                    {
                        if (timer.repeat == 0)
                        {
                            RemoveTimer(e2.element.Action.Target);

                            continue;
                        }
                        else if (timer.repeat >= 0)
                        {
                            timer.repeat--;
                        }

                        action.Invoke();
                        timer.ScheduleAbsoluteTime(elapsedTime);
                    }
                }
            }

            for (int i = removeObservers.Count - 1; i >= 0; i--)
            {
                this.updateObservers.Remove(removeObservers[i]);
            }

            for (int i = addObservers.Count - 1; i >= 0; i--)
            {
                updateObservers.Add(addObservers[i].Target, addObservers[i]);
            }

            for (int i = removeTimers.Count - 1; i >= 0; i--)
            {
                var o = removeTimers[i];

                if (timers.TryGetValue(o, out var container))
                {
                    Assert.IsTrue(container.Timer.used);
                    container.Timer.used = false;
                    timers.Remove(o);
                }
            }

            this.removeObservers.Clear();
            this.removeTimers.Clear();
            addObservers.Clear();

            this.isInUpdate = false;
        }

        public int NumUpdateObservers
        {
            get { return updateObservers.Length; }
        }

        public int NumTimers
        {
            get { return timers.Length; }
        }

        public double ElapsedTime
        {
            get { return elapsedTime; }
        }

        private Timer getTimerFromPool()
        {
            int i = 0;
            int l = timerPool.Count;
            Timer timer = null;

            while (i < l)
            {
                int timerIndex = (i + currentTimerPoolIndex) % l;

                if (!timerPool[timerIndex].used)
                {
                    currentTimerPoolIndex = timerIndex;
                    timer = timerPool[timerIndex];

                    break;
                }

                i++;
            }

            if (timer == null)
            {
                timer = new Timer();
                currentTimerPoolIndex = 0;
                timerPool.Add(timer);
            }

            timer.used = true;

            return timer;
        }

        public int DebugPoolSize
        {
            get { return timerPool.Count; }
        }
    }
}