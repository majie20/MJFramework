using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix
{
    public delegate void EventDelegateParams(object[] args);

    public class EventGroup
    {
        private EventDelegateParams call;
        private Dictionary<string, object> paramdic;
        private List<string> signList;
        private string sign;

        public EventDelegateParams Call => call;

        public EventGroup(string sign, int paramNum)
        {
            this.sign = sign;
            paramdic = new Dictionary<string, object>(paramNum);
            signList = new List<string>(EventType.EventTypeGroupDic[this.sign].Length);
        }

        public void AddListener(EventDelegateParams call)
        {
            this.call += call;
        }

        public void RemoveListener(EventDelegateParams call)
        {
            this.call -= call;
        }

        public void Invoke(string subSign, object param)
        {
            var group = EventType.EventTypeGroupDic[this.sign];
            if (group.Contains(subSign) && !signList.Contains(subSign))
            {
                signList.Add(subSign);
            }

            if (param != null)
            {
                if (paramdic.ContainsKey(subSign))
                {
                    paramdic[subSign] = param;
                }
                else
                {
                    paramdic.Add(subSign, param);
                }
            }

            if (signList.Count >= group.Length)
            {
                var paramList = new ArrayList(paramdic.Count);
                for (int i = 0; i < group.Length; i++)
                {
                    if (paramdic.ContainsKey(group[i]))
                    {
                        paramList.Add(paramdic[group[i]]);
                    }
                }
                this.call(paramList.ToArray());
                paramdic.Clear();
                signList.Clear();
            }
        }
    }

    public interface IEvent
    {
    }

    public class EventBase : IEvent
    {
        private Dictionary<object, Action> callDic = new Dictionary<object, Action>();

        public int Count => callDic.Count;

        public void AddListener(Action call, object self)
        {
            callDic.Add(self, call);
        }

        public void RemoveListener(object self)
        {
            callDic.Remove(self);
        }

        public void Invoke()
        {
            foreach (var v in callDic.Values)
            {
                v();
            }
        }
    }

    public class EventBase<T1> : IEvent
    {
        private Dictionary<object, Action<T1>> callDic = new Dictionary<object, Action<T1>>();

        public int Count => callDic.Count;

        public void AddListener(Action<T1> call, object self)
        {
            callDic.Add(self, call);
        }

        public void RemoveListener(object self)
        {
            callDic.Remove(self);
        }

        public void Invoke(T1 t1)
        {
            foreach (var v in callDic.Values)
            {
                v(t1);
            }
        }
    }

    public class EventBase<T1, T2> : IEvent
    {
        private Dictionary<object, Action<T1, T2>> callDic = new Dictionary<object, Action<T1, T2>>();

        public int Count => callDic.Count;

        public void AddListener(Action<T1, T2> call, object self)
        {
            callDic.Add(self, call);
        }

        public void RemoveListener(object self)
        {
            callDic.Remove(self);
        }

        public void Invoke(T1 t1, T2 t2)
        {
            foreach (var v in callDic.Values)
            {
                v(t1, t2);
            }
        }
    }

    public class EventBase<T1, T2, T3> : IEvent
    {
        private Dictionary<object, Action<T1, T2, T3>> callDic = new Dictionary<object, Action<T1, T2, T3>>();

        public int Count => callDic.Count;

        public void AddListener(Action<T1, T2, T3> call, object self)
        {
            callDic.Add(self, call);
        }

        public void RemoveListener(object self)
        {
            callDic.Remove(self);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            foreach (var v in callDic.Values)
            {
                v(t1, t2, t3);
            }
        }
    }

    public class EventBase<T1, T2, T3, T4> : IEvent
    {
        private Dictionary<object, Action<T1, T2, T3, T4>> callDic = new Dictionary<object, Action<T1, T2, T3, T4>>();

        public int Count => callDic.Count;

        public void AddListener(Action<T1, T2, T3, T4> call, object self)
        {
            callDic.Add(self, call);
        }

        public void RemoveListener(object self)
        {
            callDic.Remove(self);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            foreach (var v in callDic.Values)
            {
                v(t1, t2, t3, t4);
            }
        }
    }
}