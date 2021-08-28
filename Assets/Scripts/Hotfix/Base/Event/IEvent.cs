using System;
using System.Collections.Generic;

namespace MGame.Hotfix
{
    public delegate void EventDelegateParams(object[] args);

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