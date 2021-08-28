using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MGame.Model
{
    public sealed class EventSystem1
    {
        public class EventModel
        {
            public int count;
            public UnityEventBase item;
        }

        private Dictionary<string, EventModel> allEventDic;

        public EventSystem1 Init()
        {
            allEventDic = new Dictionary<string, EventModel>();
            return this;
        }

        public void Dispose()
        {
            allEventDic = null;
        }

        private EventModel GetEventModel<T1>() where T1 : UnityEventBase, new()
        {
            Type type = typeof(T1);

            if (!allEventDic.TryGetValue(type.Name, out EventModel model))
            {
                Debug.LogWarning($"{type.Name}此事件没有注册过");
                return null;
            }

            return model;
        }

        public int GetEventModelCount<T1>() where T1 : UnityEventBase, new()
        {
            Type type = typeof(T1);

            if (!allEventDic.TryGetValue(type.Name, out EventModel model))
            {
                Debug.LogWarning($"{type.Name}此事件没有注册过");
                return 0;
            }

            return model.count;
        }

        #region 添加

        private T1 Add<T1>() where T1 : UnityEventBase, new()
        {
            Type type = typeof(T1);

            if (allEventDic.TryGetValue(type.Name, out EventModel model))
            {
                model.count++;
                return (T1)model.item;
            }

            T1 t = new T1();
            allEventDic.Add(type.Name, new EventModel { count = 1, item = t });
            return t;
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

        #endregion 添加

        #region 删除

        private bool Remove<T1>(out T1 t) where T1 : UnityEventBase, new()
        {
            var model = GetEventModel<T1>();
            if (model != null)
            {
                model.count--;
                t = (T1)model.item;
                if (model.count <= 0)
                {
                    allEventDic.Remove(typeof(T1).Name);
                }
                return true;
            }

            t = null;
            return false;
        }

        public bool Remove<T1>(UnityAction call) where T1 : UnityEvent, new()
        {
            if (Remove<T1>(out T1 t))
            {
                t.RemoveListener(call);
                return true;
            }

            return false;
        }

        public bool Remove<T1, T2>(UnityAction<T2> call) where T1 : UnityEvent<T2>, new()
        {
            if (Remove<T1>(out T1 t))
            {
                t.RemoveListener(call);
                return true;
            }

            return false;
        }

        public bool Remove<T1, T2, T3>(UnityAction<T2, T3> call) where T1 : UnityEvent<T2, T3>, new()
        {
            if (Remove<T1>(out T1 t))
            {
                t.RemoveListener(call);
                return true;
            }

            return false;
        }

        public bool Remove<T1, T2, T3, T4>(UnityAction<T2, T3, T4> call) where T1 : UnityEvent<T2, T3, T4>, new()
        {
            if (Remove<T1>(out T1 t))
            {
                t.RemoveListener(call);
                return true;
            }

            return false;
        }

        #endregion 删除

        #region 执行

        public void Run<T1>() where T1 : UnityEvent, new()
        {
            var model = GetEventModel<T1>();
            if (model != null && model.count > 0)
            {
                ((T1)model.item).Invoke();
            }
        }

        public void Run<T1, T2>(T2 t2) where T1 : UnityEvent<T2>, new()
        {
            var model = GetEventModel<T1>();
            if (model != null && model.count > 0)
            {
                ((T1)model.item).Invoke(t2);
            }
        }

        public void Run<T1, T2, T3>(T2 t2, T3 t3) where T1 : UnityEvent<T2, T3>, new()
        {
            var model = GetEventModel<T1>();
            if (model != null && model.count > 0)
            {
                ((T1)model.item).Invoke(t2, t3);
            }
        }

        public void Run<T1, T2, T3, T4>(T2 t2, T3 t3, T4 t4) where T1 : UnityEvent<T2, T3, T4>, new()
        {
            var model = GetEventModel<T1>();
            if (model != null && model.count > 0)
            {
                ((T1)model.item).Invoke(t2, t3, t4);
            }
        }

        #endregion 执行
    }
}