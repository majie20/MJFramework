using Game.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Event
{
    public class EventSystem : Singleton<EventSystem>
    {
        private Dictionary<string, UnityEventBase> allEvents;

        public override void Init()
        {
            base.Init();
            allEvents = new Dictionary<string, UnityEventBase>();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private T1 Add<T1>() where T1 : UnityEventBase, new()
        {
            UnityEventBase baseEvent;

            Type type = typeof(T1);

            if (allEvents.TryGetValue(type.Name, out baseEvent))
            {
                return (T1)baseEvent;
            }
            else
            {
                T1 t = new T1();
                allEvents.Add(type.Name, t);
                return t;
            }
        }

        public void Add<T1>(UnityAction call) where T1 : UnityEvent, new()
        {
            Add<T1>().AddListener(call);
        }

        public void Add<T1, T2>(UnityAction<T2> call) where T1 : UnityEvent<T2>, new()
        {
            Add<T1>().AddListener(call);
        }

        public void Add<T1, T2, T3>(UnityAction<T2, T3> call) where T1 : UnityEvent<T2, T3>, new()
        {
            Add<T1>().AddListener(call);
        }

        public void Add<T1, T2, T3, T4>(UnityAction<T2, T3, T4> call) where T1 : UnityEvent<T2, T3, T4>, new()
        {
            Add<T1>().AddListener(call);
        }

        public T1 GetEvent<T1>() where T1 : UnityEventBase, new()
        {
            UnityEventBase baseEvent;

            Type type = typeof(T1);

            if (!allEvents.TryGetValue(type.Name, out baseEvent))
            {
                Debug.LogWarning($"{type.Name}此事件没有注册过");
                return null;
            }

            if (baseEvent == null)
            {
                Debug.LogWarning($"{type.Name}此事件没有任何人绑定");
                return null;
            }

            return (T1)baseEvent;
        }

        public bool Remove<T1>(UnityAction call) where T1 : UnityEvent, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.RemoveListener(call);
                return true;
            }
            return false;
        }

        public bool Remove<T1, T2>(UnityAction<T2> call) where T1 : UnityEvent<T2>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.RemoveListener(call);
                return true;
            }
            return false;
        }

        public bool Remove<T1, T2, T3>(UnityAction<T2, T3> call) where T1 : UnityEvent<T2, T3>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.RemoveListener(call);
                return true;
            }
            return false;
        }

        public bool Remove<T1, T2, T3, T4>(UnityAction<T2, T3, T4> call) where T1 : UnityEvent<T2, T3, T4>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.RemoveListener(call);
                return true;
            }
            return false;
        }

        public void Run<T1>() where T1 : UnityEvent, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.Invoke();
            }
        }

        public void Run<T1, T2>(T2 t2) where T1 : UnityEvent<T2>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.Invoke(t2);
            }
        }

        public void Run<T1, T2, T3>(T2 t2, T3 t3) where T1 : UnityEvent<T2, T3>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.Invoke(t2, t3);
            }
        }

        public void Run<T1, T2, T3, T4>(T2 t2, T3 t3, T4 t4) where T1 : UnityEvent<T2, T3, T4>, new()
        {
            T1 baseEvent = GetEvent<T1>();
            if (baseEvent != null)
            {
                baseEvent.Invoke(t2, t3, t4);
            }
        }
    }
}
