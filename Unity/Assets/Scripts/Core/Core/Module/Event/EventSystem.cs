using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public sealed partial class EventSystem : IDisposable
    {
        private Dictionary<Type, IEvent>   allEventClassDic;
        private Dictionary<uint, IEvent>   allEventParamsUintDic;
        private Dictionary<string, IEvent> allEventParamsStrDic;

        private bool _isLocal;

        public EventSystem(bool isLocal)
        {
            this._isLocal = isLocal;

            if (isLocal)
            {
                allEventClassDic = new Dictionary<Type, IEvent>();
                allEventParamsUintDic = new Dictionary<uint, IEvent>();
                allEventParamsStrDic = new Dictionary<string, IEvent>();
            }
            else
            {
                allEventClassDic = new Dictionary<Type, IEvent>(30);
                allEventParamsUintDic = new Dictionary<uint, IEvent>(15);
                allEventParamsStrDic = new Dictionary<string, IEvent>(15);
            }
        }

        public void Dispose()
        {
            if (_isLocal)
            {
                allEventClassDic.Clear();
                allEventParamsUintDic.Clear();
                allEventParamsStrDic.Clear();
            }
            else
            {
                allEventClassDic = null;
                allEventParamsUintDic = null;
                allEventParamsStrDic = null;
            }
        }

        private T1 AddEventModel<T1>() where T1 : IEvent, new()
        {
            Type type = typeof(T1);

            if (!allEventClassDic.TryGetValue(type, out IEvent e))
            {
                e = new T1();

                allEventClassDic.Add(type, e);
            }

            return (T1)e;
        }

        public IEvent GetEventModel(Type type)
        {
            if (allEventClassDic.TryGetValue(type, out IEvent e))
            {
                return e;
            }

            //NLog.Log.Warn($"{type.FullName}此事件没有注册过");

            return default;
        }

        public T1 GetEventModel<T1>() where T1 : IEvent, new()
        {
            return (T1)GetEventModel(typeof(T1));
        }

        private T1 AddEventModel<T1>(uint i) where T1 : IEvent, new()
        {
            if (!allEventParamsUintDic.TryGetValue(i, out IEvent e))
            {
                e = new T1();

                allEventParamsUintDic.Add(i, e);
            }

            return (T1)e;
        }

        public IEvent GetEventModel(uint i)
        {
            if (allEventParamsUintDic.TryGetValue(i, out IEvent e))
            {
                return e;
            }

            //NLog.Log.Warn($"{i}此事件没有注册过");

            return default;
        }

        private T1 AddEventModel<T1>(string str) where T1 : IEvent, new()
        {
            if (!allEventParamsStrDic.TryGetValue(str, out IEvent e))
            {
                e = new T1();

                allEventParamsStrDic.Add(str, e);
            }

            return (T1)e;
        }

        public IEvent GetEventModel(string str)
        {
            if (allEventParamsStrDic.TryGetValue(str, out IEvent e))
            {
                return e;
            }

            //NLog.Log.Warn($"{str}此事件没有注册过");

            return default;
        }

        public void Clear()
        {
            var signList = allEventClassDic.Keys.ToArray();
            var len = signList.Length;

            for (int i = 0; i < len; i++)
            {
                var sign = signList[i];

                if (allEventClassDic[sign].Count() == 0)
                {
                    allEventClassDic.Remove(sign);
                }
            }

            var signList2 = allEventParamsUintDic.Keys.ToArray();
            len = signList2.Length;

            for (int i = 0; i < len; i++)
            {
                var sign = signList2[i];

                if (allEventParamsUintDic[sign].Count() == 0)
                {
                    allEventParamsUintDic.Remove(sign);
                }
            }

            var signList3 = allEventParamsStrDic.Keys.ToArray();
            len = signList3.Length;

            for (int i = 0; i < len; i++)
            {
                var sign = signList3[i];

                if (allEventParamsStrDic[sign].Count() == 0)
                {
                    allEventParamsStrDic.Remove(sign);
                }
            }
        }

        #region 订阅

        public void AddListener<T1>(Component self, Action call) where T1 : EventBase1, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListener<T1, T2>(Component self, Action<T2> call) where T1 : EventBase1<T2>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListener<T1, T2, T3>(Component self, Action<T2, T3> call) where T1 : EventBase1<T2, T3>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListener<T1, T2, T3, T4>(Component self, Action<T2, T3, T4> call) where T1 : EventBase1<T2, T3, T4>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListener<T1, T2, T3, T4, T5>(Component self, Action<T2, T3, T4, T5> call) where T1 : EventBase1<T2, T3, T4, T5>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListener(uint sign, Component self, Action call)
        {
            AddEventModel<EventBase2>(sign).AddListener(self, call);
        }

        public void AddListener<T2>(uint sign, Component self, Action<T2> call)
        {
            AddEventModel<EventBase2<T2>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3>(uint sign, Component self, Action<T2, T3> call)
        {
            AddEventModel<EventBase2<T2, T3>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3, T4>(uint sign, Component self, Action<T2, T3, T4> call)
        {
            AddEventModel<EventBase2<T2, T3, T4>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3, T4, T5>(uint sign, Component self, Action<T2, T3, T4, T5> call)
        {
            AddEventModel<EventBase2<T2, T3, T4, T5>>(sign).AddListener(self, call);
        }

        public void AddListener(string sign, Component self, Action call)
        {
            AddEventModel<EventBase2>(sign).AddListener(self, call);
        }

        public void AddListener<T2>(string sign, Component self, Action<T2> call)
        {
            AddEventModel<EventBase2<T2>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3>(string sign, Component self, Action<T2, T3> call)
        {
            AddEventModel<EventBase2<T2, T3>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3, T4>(string sign, Component self, Action<T2, T3, T4> call)
        {
            AddEventModel<EventBase2<T2, T3, T4>>(sign).AddListener(self, call);
        }

        public void AddListener<T2, T3, T4, T5>(string sign, Component self, Action<T2, T3, T4, T5> call)
        {
            AddEventModel<EventBase2<T2, T3, T4, T5>>(sign).AddListener(self, call);
        }

        public void AddListenerMult(uint sign, uint[] signs, int paramNum, Component self, Action<object[]> call)
        {
            if (!allEventParamsUintDic.TryGetValue(sign, out var e))
            {
                e = new EventGroup<uint>(signs, paramNum);

                allEventParamsUintDic.Add(sign, e);
            }

            (e as EventGroup<uint>).AddListener(self, call);
        }

        #endregion 订阅

        #region 移除

        public void RemoveListener<T1>(Component self) where T1 : EventBase1, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2>(Component self) where T1 : EventBase1<T2>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3>(Component self) where T1 : EventBase1<T2, T3>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3, T4>(Component self) where T1 : EventBase1<T2, T3, T4>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3, T4, T5>(Component self) where T1 : EventBase1<T2, T3, T4, T5>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListener(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3, T4>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListener<T1, T2, T3, T4>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerMult(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveAllListener<T1>() where T1 : IEvent, new()
        {
            GetEventModel<T1>()?.RemoveAllListener();
        }

        public void RemoveAllListenerUint(uint sign)
        {
            GetEventModel(sign)?.RemoveAllListener();
        }

        public void RemoveAllListenerStr(string sign)
        {
            GetEventModel(sign)?.RemoveAllListener();
        }

        public void RemoveAllListenerMult(uint sign)
        {
            GetEventModel(sign)?.RemoveAllListener();
        }

        #endregion 移除

        #region 发布

        public void Invoke<T1>() where T1 : EventBase1, new()
        {
            GetEventModel<T1>()?.Invoke();
        }

        public void Invoke<T1, T2>(T2 t2) where T1 : EventBase1<T2>, new()
        {
            GetEventModel<T1>()?.Invoke(t2);
        }

        public void Invoke<T1, T2, T3>(T2 t2, T3 t3) where T1 : EventBase1<T2, T3>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3);
        }

        public void Invoke<T1, T2, T3, T4>(T2 t2, T3 t3, T4 t4) where T1 : EventBase1<T2, T3, T4>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4);
        }

        public void Invoke<T1, T2, T3, T4, T5>(T2 t2, T3 t3, T4 t4, T5 t5) where T1 : EventBase1<T2, T3, T4, T5>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4, t5);
        }

        public void Invoke(uint sign)
        {
            (GetEventModel(sign) as EventBase2)?.Invoke();
        }

        public void Invoke<T1>(uint sign, T1 t1)
        {
            (GetEventModel(sign) as EventBase2<T1>)?.Invoke(t1);
        }

        public void Invoke<T1, T2>(uint sign, T1 t1, T2 t2)
        {
            (GetEventModel(sign) as EventBase2<T1, T2>)?.Invoke(t1, t2);
        }

        public void Invoke<T1, T2, T3>(uint sign, T1 t1, T2 t2, T3 t3)
        {
            (GetEventModel(sign) as EventBase2<T1, T2, T3>)?.Invoke(t1, t2, t3);
        }

        public void Invoke<T1, T2, T3, T4>(uint sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            (GetEventModel(sign) as EventBase2<T1, T2, T3, T4>)?.Invoke(t1, t2, t3, t4);
        }

        public void Invoke(string sign)
        {
            (GetEventModel(sign) as EventBase2)?.Invoke();
        }

        public void Invoke<T1>(string sign, T1 t1)
        {
            (GetEventModel(sign) as EventBase2<T1>)?.Invoke(t1);
        }

        public void Invoke<T1, T2>(string sign, T1 t1, T2 t2)
        {
            (GetEventModel(sign) as EventBase2<T1, T2>)?.Invoke(t1, t2);
        }

        public void Invoke<T1, T2, T3>(string sign, T1 t1, T2 t2, T3 t3)
        {
            (GetEventModel(sign) as EventBase2<T1, T2, T3>)?.Invoke(t1, t2, t3);
        }

        public void Invoke<T1, T2, T3, T4>(string sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            (GetEventModel(sign) as EventBase2<T1, T2, T3, T4>)?.Invoke(t1, t2, t3, t4);
        }

        public void InvokeMult(uint sign, uint subSign, object t1 = null)
        {
            (GetEventModel(sign) as EventGroup<uint>)?.Invoke(subSign, t1);
        }

        #endregion 发布
    }
}