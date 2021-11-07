using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public sealed class EventSystem : IDisposable
    {
        private Dictionary<string, IEvent> allEventDic;
        private Dictionary<string, EventDelegateParams> allEventParamsDic;
        private Dictionary<string, EventGroup> allEventGroupDic;

        public EventSystem()
        {
            EventType.Init();

            allEventDic = new Dictionary<string, IEvent>();
            allEventParamsDic = new Dictionary<string, EventDelegateParams>();
            allEventGroupDic = new Dictionary<string, EventGroup>();
        }

        public void Dispose()
        {
            allEventDic = null;
            allEventParamsDic = null;
            allEventGroupDic = null;
        }

        private T1 AddEventModel<T1>() where T1 : IEvent, new()
        {
            Type type = typeof(T1);

            if (!allEventDic.TryGetValue(type.FullName, out IEvent e))
            {
                e = new T1();
                allEventDic.Add(type.FullName, e);
            }

            return (T1)e;
        }

        private T1 GetEventModel<T1>() where T1 : IEvent, new()
        {
            Type type = typeof(T1);

            if (allEventDic.TryGetValue(type.FullName, out IEvent e))
            {
                return (T1)e;
            }

            Debug.LogWarning($"{type.FullName}此事件没有注册过");

            return default;
        }

        #region 订阅

        public void AddListener<T1>(Action call, object self) where T1 : EventBase, new()
        {
            AddEventModel<T1>().AddListener(call, self);
        }

        public void AddListener<T1, T2>(Action<T2> call, object self) where T1 : EventBase<T2>, new()
        {
            AddEventModel<T1>().AddListener(call, self);
        }

        public void AddListener<T1, T2, T3>(Action<T2, T3> call, object self) where T1 : EventBase<T2, T3>, new()
        {
            AddEventModel<T1>().AddListener(call, self);
        }

        public void AddListener<T1, T2, T3, T4>(Action<T2, T3, T4> call, object self) where T1 : EventBase<T2, T3, T4>, new()
        {
            AddEventModel<T1>().AddListener(call, self);
        }

        public void AddListener<T1, T2, T3, T4, T5>(Action<T2, T3, T4, T5> call, object self) where T1 : EventBase<T2, T3, T4, T5>, new()
        {
            AddEventModel<T1>().AddListener(call, self);
        }

        public void AddListener(string sign, EventDelegateParams call)
        {
            if (allEventParamsDic.ContainsKey(sign))
            {
                allEventParamsDic[sign] += call;
                return;
            }
            allEventParamsDic.Add(sign, call);
        }

        public void AddListener2(string sign, int paramNum, EventDelegateParams call)
        {
            if (!allEventGroupDic.ContainsKey(sign))
            {
                allEventGroupDic.Add(sign, new EventGroup(sign, paramNum));
            }

            allEventGroupDic[sign].AddListener(call);
        }

        #endregion 订阅

        #region 移除

        public void RemoveListener<T1>(object self) where T1 : EventBase, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
                if (e.Count == 0)
                {
                    allEventDic.Remove(typeof(T1).FullName);
                }
            }
        }

        public void RemoveListener<T1, T2>(object self) where T1 : EventBase<T2>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
                if (e.Count == 0)
                {
                    allEventDic.Remove(typeof(T1).FullName);
                }
            }
        }

        public void RemoveListener<T1, T2, T3>(object self) where T1 : EventBase<T2, T3>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
                if (e.Count == 0)
                {
                    allEventDic.Remove(typeof(T1).FullName);
                }
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(object self) where T1 : EventBase<T2, T3, T4>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
                if (e.Count == 0)
                {
                    allEventDic.Remove(typeof(T1).FullName);
                }
            }
        }

        public void RemoveListener<T1, T2, T3, T4, T5>(object self) where T1 : EventBase<T2, T3, T4, T5>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
                if (e.Count == 0)
                {
                    allEventDic.Remove(typeof(T1).FullName);
                }
            }
        }

        public void RemoveListener(string sign, EventDelegateParams call)
        {
            if (allEventParamsDic.ContainsKey(sign))
            {
                allEventParamsDic[sign] -= call;
                if (allEventParamsDic[sign] == null)
                {
                    allEventParamsDic.Remove(sign);
                }
            }
        }

        public void RemoveListener2(string sign, EventDelegateParams call)
        {
            if (allEventGroupDic.ContainsKey(sign))
            {
                allEventGroupDic[sign].RemoveListener(call);
                if (allEventGroupDic[sign].Call == null)
                {
                    allEventGroupDic.Remove(sign);
                }
            }
        }

        public void RemoveAllListener<T1>() where T1 : IEvent, new()
        {
            allEventDic.Remove(typeof(T1).FullName);
        }

        public void RemoveAllListener(string sign)
        {
            allEventParamsDic.Remove(sign);
        }

        public void RemoveAllListener2(string sign)
        {
            allEventGroupDic.Remove(sign);
        }

        #endregion 移除

        #region 发布

        public void Invoke<T1>() where T1 : EventBase, new()
        {
            GetEventModel<T1>()?.Invoke();
        }

        public void Invoke<T1, T2>(T2 t2) where T1 : EventBase<T2>, new()
        {
            GetEventModel<T1>()?.Invoke(t2);
        }

        public void Invoke<T1, T2, T3>(T2 t2, T3 t3) where T1 : EventBase<T2, T3>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3);
        }

        public void Invoke<T1, T2, T3, T4>(T2 t2, T3 t3, T4 t4) where T1 : EventBase<T2, T3, T4>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4);
        }

        public void Invoke<T1, T2, T3, T4, T5>(T2 t2, T3 t3, T4 t4, T5 t5) where T1 : EventBase<T2, T3, T4, T5>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4, t5);
        }

        public void Invoke(string sign, params object[] t1)
        {
            if (allEventParamsDic.TryGetValue(sign, out EventDelegateParams e))
            {
                e(t1);
            }
        }

        public void Invoke2(string sign, string subSign, object t1 = null)
        {
            if (allEventGroupDic.ContainsKey(sign))
            {
                allEventGroupDic[sign].Invoke(subSign, t1);
            }
            else
            {
                Debug.LogWarning($"{sign}此事件组没有注册过");
            }
        }

        #endregion 发布
    }
}