using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class EventGroup<T>
    {
        private StaticLinkedListDictionary<object, Action<object[]>> calls;
        private Dictionary<T, object> paramdic;
        private HashSet<T> signList;
        private T[] signs;

        public EventGroup(T[] signs, int paramNum)
        {
            this.signs = signs;
            paramdic = new Dictionary<T, object>(paramNum);
            signList = new HashSet<T>();
        }

        public int Count()
        {
            return calls.Length;
        }

        public void AddListener(object self, Action<object[]> call)
        {
            if (!calls.ContainsKey(self))
            {
                calls.Add(self, call);
            }
        }

        public void RemoveListener(object self)
        {
            if (calls.ContainsKey(self))
            {
                calls.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            calls.Clear();
        }

        public void Invoke(T subSign, object param)
        {
            if (signs.Contains(subSign) && !signList.Contains(subSign))
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

            if (signList.Count >= signs.Length)
            {
                object[] paramList = new object[paramdic.Count];
                var index = 0;
                for (int i = 0; i < signs.Length; i++)
                {
                    if (paramdic.ContainsKey(signs[i]))
                    {
                        paramList[index] = paramdic[signs[i]];
                        index++;
                    }
                }
                var data = calls[1];
                while (data.right != 0)
                {
                    data = calls[data.right];
                    data.element(paramList);
                }
                paramdic.Clear();
                signList.Clear();
            }
        }
    }

    #region EventBaseAsync1

    public class EventBaseAsync1 : EventBase<object, Func<UniTask>>
    {
        public override void AddListener(object self, Func<UniTask> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke()
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke();
            }
        }
    }

    public class EventBaseAsync1<T1> : EventBase<object, Func<T1, UniTask>, T1>
    {
        public override void AddListener(object self, Func<T1, UniTask> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1);
            }
        }
    }

    public class EventBaseAsync1<T1, T2> : EventBase<object, Func<T1, T2, UniTask>, T1, T2>
    {
        public override void AddListener(object self, Func<T1, T2, UniTask> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2);
            }
        }
    }

    public class EventBaseAsync1<T1, T2, T3> : EventBase<object, Func<T1, T2, T3, UniTask>, T1, T2, T3>
    {
        public override void AddListener(object self, Func<T1, T2, T3, UniTask> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2, t3);
            }
        }
    }

    public class EventBaseAsync1<T1, T2, T3, T4> : EventBase<object, Func<T1, T2, T3, T4, UniTask>, T1, T2, T3, T4>
    {
        public override void AddListener(object self, Func<T1, T2, T3, T4, UniTask> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2, t3, t4);
            }
        }
    }

    #endregion EventBaseAsync1

    #region EventBase1

    public class EventBase1 : EventBase<object, Action>
    {
        public override void AddListener(object self, Action call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke()
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke();
            }
        }
    }

    public class EventBase1<T1> : EventBase<object, Action<T1>, T1>
    {
        public override void AddListener(object self, Action<T1> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1);
            }
        }
    }

    public class EventBase1<T1, T2> : EventBase<object, Action<T1, T2>, T1, T2>
    {
        public override void AddListener(object self, Action<T1, T2> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2);
            }
        }
    }

    public class EventBase1<T1, T2, T3> : EventBase<object, Action<T1, T2, T3>, T1, T2, T3>
    {
        public override void AddListener(object self, Action<T1, T2, T3> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2, t3);
            }
        }
    }

    public class EventBase1<T1, T2, T3, T4> : EventBase<object, Action<T1, T2, T3, T4>, T1, T2, T3, T4>
    {
        public override void AddListener(object self, Action<T1, T2, T3, T4> call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                data.element.Invoke(t1, t2, t3, t4);
            }
        }
    }

    #endregion EventBase1

    #region EventBaseAsync2

    public sealed class EventBaseAsync2 : EventBase<object, object>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke()
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Func<UniTask>).Invoke();
            }
        }
    }

    public sealed class EventBaseAsync2<T1> : EventBase<object, object, T1>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Func<T1, UniTask>).Invoke(t1);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2> : EventBase<object, object, T1, T2>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Func<T1, T2, UniTask>).Invoke(t1, t2);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2, T3> : EventBase<object, object, T1, T2, T3>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Func<T1, T2, T3, UniTask>).Invoke(t1, t2, t3);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2, T3, T4> : EventBase<object, object, T1, T2, T3, T4>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }

        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Func<T1, T2, T3, T4, UniTask>).Invoke(t1, t2, t3, t4);
            }
        }
    }

    #endregion EventBaseAsync2

    #region EventBase2

    public sealed class EventBase2 : EventBase<object, object>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }
    }

    public sealed class EventBase2<T1> : EventBase<object, object, T1>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }
    }

    public sealed class EventBase2<T1, T2> : EventBase<object, object, T1, T2>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }
    }

    public sealed class EventBase2<T1, T2, T3> : EventBase<object, object, T1, T2, T3>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }
    }

    public sealed class EventBase2<T1, T2, T3, T4> : EventBase<object, object, T1, T2, T3, T4>
    {
        public override void AddListener(object self, object call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
                if (self is Component component)
                {
                    if (!component.EventList.Contains(this))
                    {
                        component.EventList.Add(this);
                    }
                }
            }
        }
    }

    #endregion EventBase2

    public class EventValue
    {
        public const int INITIAL_COUNT = 8;
    }

    public interface IEvent
    {
        int Count();

        void RemoveListener2(object self);

        void RemoveAllListener();
    }

    public class EventBase<A, B> : IEvent
    {
        protected StaticLinkedListDictionary<A, B> datas = new StaticLinkedListDictionary<A, B>(EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return datas.Length;
        }

        public virtual void RemoveListener2(object self)
        {
            RemoveListener((A)self);
        }

        public virtual void AddListener(A self, B call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
            }
        }

        public virtual void RemoveListener(A self)
        {
            if (datas.ContainsKey(self))
            {
                datas.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            datas.Clear();
        }

        public virtual void Invoke()
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Action).Invoke();
            }
        }
    }

    public class EventBase<A, B, T1> : IEvent
    {
        protected StaticLinkedListDictionary<A, B> datas = new StaticLinkedListDictionary<A, B>(EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return datas.Length;
        }

        public virtual void RemoveListener2(object self)
        {
            RemoveListener((A)self);
        }

        public virtual void AddListener(A self, B call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
            }
        }

        public virtual void RemoveListener(A self)
        {
            if (datas.ContainsKey(self))
            {
                datas.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            datas.Clear();
        }

        public virtual void Invoke(T1 t1)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Action<T1>).Invoke(t1);
            }
        }
    }

    public class EventBase<A, B, T1, T2> : IEvent
    {
        protected StaticLinkedListDictionary<A, B> datas = new StaticLinkedListDictionary<A, B>(EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return datas.Length;
        }

        public virtual void RemoveListener2(object self)
        {
            RemoveListener((A)self);
        }

        public virtual void AddListener(A self, B call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
            }
        }

        public virtual void RemoveListener(A self)
        {
            if (datas.ContainsKey(self))
            {
                datas.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Action<T1, T2>).Invoke(t1, t2);
            }
        }
    }

    public class EventBase<A, B, T1, T2, T3> : IEvent
    {
        protected StaticLinkedListDictionary<A, B> datas = new StaticLinkedListDictionary<A, B>(EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return datas.Length;
        }

        public virtual void RemoveListener2(object self)
        {
            RemoveListener((A)self);
        }

        public virtual void AddListener(A self, B call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
            }
        }

        public virtual void RemoveListener(A self)
        {
            if (datas.ContainsKey(self))
            {
                datas.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Action<T1, T2, T3>).Invoke(t1, t2, t3);
            }
        }
    }

    public class EventBase<A, B, T1, T2, T3, T4> : IEvent
    {
        protected StaticLinkedListDictionary<A, B> datas = new StaticLinkedListDictionary<A, B>(EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return datas.Length;
        }

        public virtual void RemoveListener2(object self)
        {
            RemoveListener((A)self);
        }

        public virtual void AddListener(A self, B call)
        {
            if (!datas.ContainsKey(self))
            {
                datas.Add(self, call);
            }
        }

        public virtual void RemoveListener(A self)
        {
            if (datas.ContainsKey(self))
            {
                datas.Remove(self);
            }
        }

        public void RemoveAllListener()
        {
            datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = datas[1];
            while (data.right != 0)
            {
                data = datas[data.right];
                (data.element as Action<T1, T2, T3, T4>).Invoke(t1, t2, t3, t4);
            }
        }
    }
}