using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public sealed class EventSystem : IDisposable
    {
        private Dictionary<string, IEvent> allEventDic;
        private Dictionary<uint, EventGroup> allEventGroupDic;
        private Dictionary<uint, object> allEventParamsUintDic;
        private Dictionary<string, object> allEventParamsStrDic;

        public EventSystem()
        {
            EventType.Init();

            allEventDic = new Dictionary<string, IEvent>();
            allEventGroupDic = new Dictionary<uint, EventGroup>();
            allEventParamsUintDic = new Dictionary<uint, object>();
            allEventParamsStrDic = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            allEventDic = null;
            allEventGroupDic = null;
            allEventParamsUintDic = null;
            allEventParamsStrDic = null;
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

        public void Clear()
        {
            var signList = allEventDic.Keys.ToArray();
            for (int i = 0; i < signList.Length; i++)
            {
                var sign = signList[i];
                if (allEventDic[sign].Count() == 0)
                {
                    allEventDic.Remove(sign);
                }
            }

            var signList1 = allEventGroupDic.Keys.ToArray();
            for (int i = 0; i < signList1.Length; i++)
            {
                var sign = signList1[i];
                if (allEventGroupDic[sign].Call == null)
                {
                    allEventGroupDic.Remove(sign);
                }
            }
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

        public void AddListener(uint sign, Action call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action e = allEventParamsUintDic[sign] as Action;
                e += call;
                return;
            }
            allEventParamsUintDic.Add(sign, new Action(call));
        }

        public void AddListener<T1>(uint sign, Action<T1> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsUintDic[sign] as Action<T1>;
                e += call;
                return;
            }

            allEventParamsUintDic.Add(sign, new Action<T1>(call));
        }

        public void AddListener<T1, T2>(uint sign, Action<T1, T2> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsUintDic[sign] as Action<T1, T2>;
                e += call;
                return;
            }

            allEventParamsUintDic.Add(sign, new Action<T1, T2>(call));
        }

        public void AddListener<T1, T2, T3>(uint sign, Action<T1, T2, T3> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsUintDic[sign] as Action<T1, T2, T3>;
                e += call;
                return;
            }

            allEventParamsUintDic.Add(sign, new Action<T1, T2, T3>(call));
        }

        public void AddListener<T1, T2, T3, T4>(uint sign, Action<T1, T2, T3, T4> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsUintDic[sign] as Action<T1, T2, T3, T4>;
                e += call;
                return;
            }

            allEventParamsUintDic.Add(sign, new Action<T1, T2, T3, T4>(call));
        }

        public void AddListener(string sign, Action call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action e = allEventParamsStrDic[sign] as Action;
                e += call;
                return;
            }
            allEventParamsStrDic.Add(sign, new Action(call));
        }

        public void AddListener<T1>(string sign, Action<T1> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsStrDic[sign] as Action<T1>;
                e += call;
                return;
            }

            allEventParamsStrDic.Add(sign, new Action<T1>(call));
        }

        public void AddListener<T1, T2>(string sign, Action<T1, T2> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsStrDic[sign] as Action<T1, T2>;
                e += call;
                return;
            }

            allEventParamsStrDic.Add(sign, new Action<T1, T2>(call));
        }

        public void AddListener<T1, T2, T3>(string sign, Action<T1, T2, T3> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsStrDic[sign] as Action<T1, T2, T3>;
                e += call;
                return;
            }

            allEventParamsStrDic.Add(sign, new Action<T1, T2, T3>(call));
        }

        public void AddListener<T1, T2, T3, T4>(string sign, Action<T1, T2, T3, T4> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsStrDic[sign] as Action<T1, T2, T3, T4>;
                e += call;
                return;
            }

            allEventParamsStrDic.Add(sign, new Action<T1, T2, T3, T4>(call));
        }

        public void AddListenerMult(uint sign, int paramNum, EventDelegateParams call)
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
            }
        }

        public void RemoveListener<T1, T2>(object self) where T1 : EventBase<T2>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
            }
        }

        public void RemoveListener<T1, T2, T3>(object self) where T1 : EventBase<T2, T3>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(object self) where T1 : EventBase<T2, T3, T4>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
            }
        }

        public void RemoveListener<T1, T2, T3, T4, T5>(object self) where T1 : EventBase<T2, T3, T4, T5>, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveListener(self);
            }
        }

        public void RemoveListener(uint sign, Action call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action e = allEventParamsUintDic[sign] as Action;
                e -= call;
                if (e == null)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1>(uint sign, Action<T1> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsUintDic[sign] as Action<T1>;
                e -= call;
                if (e == null)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2>(uint sign, Action<T1, T2> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsUintDic[sign] as Action<T1, T2>;
                e -= call;
                if (e == null)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2, T3>(uint sign, Action<T1, T2, T3> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsUintDic[sign] as Action<T1, T2, T3>;
                e -= call;
                if (e == null)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(uint sign, Action<T1, T2, T3, T4> call)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsUintDic[sign] as Action<T1, T2, T3, T4>;
                e -= call;
                if (e == null)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }
        }

        public void RemoveListener(string sign, Action call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action e = allEventParamsStrDic[sign] as Action;
                e -= call;
                if (e == null)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1>(string sign, Action<T1> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsStrDic[sign] as Action<T1>;
                e -= call;
                if (e == null)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2>(string sign, Action<T1, T2> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsStrDic[sign] as Action<T1, T2>;
                e -= call;
                if (e == null)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2, T3>(string sign, Action<T1, T2, T3> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsStrDic[sign] as Action<T1, T2, T3>;
                e -= call;
                if (e == null)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(string sign, Action<T1, T2, T3, T4> call)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsStrDic[sign] as Action<T1, T2, T3, T4>;
                e -= call;
                if (e == null)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        public void RemoveListenerMult(uint sign, EventDelegateParams call)
        {
            if (allEventGroupDic.ContainsKey(sign))
            {
                allEventGroupDic[sign].RemoveListener(call);
            }
        }

        public void RemoveAllListener<T1>() where T1 : IEvent, new()
        {
            var e = GetEventModel<T1>();
            if (e != null)
            {
                e.RemoveAllListener();
            }
        }

        public void RemoveAllListenerUint(uint sign)
        {
            allEventParamsUintDic.Remove(sign);
        }

        public void RemoveAllListenerStr(string sign)
        {
            allEventParamsStrDic.Remove(sign);
        }

        public void RemoveAllListenerMult(uint sign)
        {
            if (allEventGroupDic.ContainsKey(sign))
            {
                allEventGroupDic[sign].RemoveAllListener();
            }
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

        public void Invoke(uint sign)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action e = allEventParamsUintDic[sign] as Action;
                e();
            }
        }

        public void Invoke<T1>(uint sign, T1 t1)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsUintDic[sign] as Action<T1>;
                e(t1);
            }
        }

        public void Invoke<T1, T2>(uint sign, T1 t1, T2 t2)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsUintDic[sign] as Action<T1, T2>;
                e(t1, t2);
            }
        }

        public void Invoke<T1, T2, T3>(uint sign, T1 t1, T2 t2, T3 t3)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsUintDic[sign] as Action<T1, T2, T3>;
                e(t1, t2, t3);
            }
        }

        public void Invoke<T1, T2, T3, T4>(uint sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (allEventParamsUintDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsUintDic[sign] as Action<T1, T2, T3, T4>;
                e(t1, t2, t3, t4);
            }
        }

        public void Invoke(string sign)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action e = allEventParamsStrDic[sign] as Action;
                e();
            }
        }

        public void Invoke<T1>(string sign, T1 t1)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1> e = allEventParamsStrDic[sign] as Action<T1>;
                e(t1);
            }
        }

        public void Invoke<T1, T2>(string sign, T1 t1, T2 t2)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2> e = allEventParamsStrDic[sign] as Action<T1, T2>;
                e(t1, t2);
            }
        }

        public void Invoke<T1, T2, T3>(string sign, T1 t1, T2 t2, T3 t3)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3> e = allEventParamsStrDic[sign] as Action<T1, T2, T3>;
                e(t1, t2, t3);
            }
        }

        public void Invoke<T1, T2, T3, T4>(string sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (allEventParamsStrDic.ContainsKey(sign))
            {
                Action<T1, T2, T3, T4> e = allEventParamsStrDic[sign] as Action<T1, T2, T3, T4>;
                e(t1, t2, t3, t4);
            }
        }

        public void InvokeMult(uint sign, uint subSign, object t1 = null)
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