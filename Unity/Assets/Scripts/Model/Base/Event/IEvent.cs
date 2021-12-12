using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public delegate void EventDelegateParams(object[] args);

    public class EventGroup
    {
        private EventDelegateParams call;
        private Dictionary<uint, object> paramdic;
        private List<uint> signList;
        private uint sign;

        public EventDelegateParams Call => call;

        public EventGroup(uint sign, int paramNum)
        {
            this.sign = sign;
            paramdic = new Dictionary<uint, object>(paramNum);
            signList = new List<uint>(EventType.EventTypeGroupDic[this.sign].Length);
        }

        public void AddListener(EventDelegateParams call)
        {
            this.call += call;
        }

        public void RemoveListener(EventDelegateParams call)
        {
            this.call -= call;
        }

        public void RemoveAllListener()
        {
            this.call = null;
        }

        public void Invoke(uint subSign, object param)
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
                object[] paramList = new object[paramdic.Count];
                for (int i = 0; i < group.Length; i++)
                {
                    if (paramdic.ContainsKey(group[i]))
                    {
                        paramList[i] = paramdic[group[i]];
                    }
                }
                this.call(paramList);
                paramdic.Clear();
                signList.Clear();
            }
        }
    }

    public interface IEvent
    {
        int Count();

        void RemoveListener(object self);

        void RemoveAllListener();
    }

    public class EventBase : IEvent
    {
        private List<object> objects = new List<object>();
        private List<Action> calls = new List<Action>();

        public int Count()
        {
            return calls.Count;
        }

        public void AddListener(Action call, object self)
        {
            objects.Add(self);
            calls.Add(call);
        }

        public void RemoveListener(object self)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == self)
                {
                    objects.RemoveAt(i);
                    calls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAllListener()
        {
            objects.Clear();
            calls.Clear();
        }

        public void Invoke()
        {
            for (int i = 0; i < calls.Count; i++)
            {
                calls[i]();
            }
        }
    }

    public class EventBase<T1> : IEvent
    {
        private List<object> objects = new List<object>();
        private List<Action<T1>> calls = new List<Action<T1>>();

        public int Count()
        {
            return calls.Count;
        }

        public void AddListener(Action<T1> call, object self)
        {
            objects.Add(self);
            calls.Add(call);
        }

        public void RemoveListener(object self)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == self)
                {
                    objects.RemoveAt(i);
                    calls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAllListener()
        {
            objects.Clear();
            calls.Clear();
        }

        public void Invoke(T1 t1)
        {
            for (int i = 0; i < calls.Count; i++)
            {
                calls[i](t1);
            }
        }
    }

    public class EventBase<T1, T2> : IEvent
    {
        private List<object> objects = new List<object>();
        private List<Action<T1, T2>> calls = new List<Action<T1, T2>>();

        public int Count()
        {
            return calls.Count;
        }

        public void AddListener(Action<T1, T2> call, object self)
        {
            objects.Add(self);
            calls.Add(call);
        }

        public void RemoveListener(object self)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == self)
                {
                    objects.RemoveAt(i);
                    calls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAllListener()
        {
            objects.Clear();
            calls.Clear();
        }

        public void Invoke(T1 t1, T2 t2)
        {
            for (int i = 0; i < calls.Count; i++)
            {
                calls[i](t1, t2);
            }
        }
    }

    public class EventBase<T1, T2, T3> : IEvent
    {
        private List<object> objects = new List<object>();
        private List<Action<T1, T2, T3>> calls = new List<Action<T1, T2, T3>>();

        public int Count()
        {
            return calls.Count;
        }

        public void AddListener(Action<T1, T2, T3> call, object self)
        {
            objects.Add(self);
            calls.Add(call);
        }

        public void RemoveListener(object self)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == self)
                {
                    objects.RemoveAt(i);
                    calls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAllListener()
        {
            objects.Clear();
            calls.Clear();
        }

        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            for (int i = 0; i < calls.Count; i++)
            {
                calls[i](t1, t2, t3);
            }
        }
    }

    public class EventBase<T1, T2, T3, T4> : IEvent
    {
        private List<object> objects = new List<object>();
        private List<Action<T1, T2, T3, T4>> calls = new List<Action<T1, T2, T3, T4>>();

        public int Count()
        {
            return calls.Count;
        }

        public void AddListener(Action<T1, T2, T3, T4> call, object self)
        {
            objects.Add(self);
            calls.Add(call);
        }

        public void RemoveListener(object self)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == self)
                {
                    objects.RemoveAt(i);
                    calls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAllListener()
        {
            objects.Clear();
            calls.Clear();
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            for (int i = 0; i < calls.Count; i++)
            {
                calls[i](t1, t2, t3, t4);
            }
        }
    }
}