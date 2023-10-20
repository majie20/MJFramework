using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using M.Algorithm;

namespace Model
{
    public class EventGroup<T> : IEvent
    {
        private StaticLinkedListDictionary<Component, Action<object[]>> _calls;
        private Dictionary<T, object>                                   _paramDic;
        private HashSet<T>                                              _signList;
        private T[]                                                     _signs;

        public EventGroup(T[] signs, int paramNum)
        {
            this._signs = signs;
            _paramDic = new Dictionary<T, object>(paramNum);
            _signList = new HashSet<T>();
        }

        public int Count()
        {
            return _calls.Length;
        }

        public void AddListener(Component self, Action<object[]> call)
        {
            if (!_calls.ContainsKey(self))
            {
                _calls.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public void RemoveListener(Component self)
        {
            _calls.Remove(self);
        }

        public void RemoveAllListener()
        {
            _calls.Clear();
        }

        public void Invoke(T subSign, object param)
        {
            if (!_signs.Contains(subSign))
            {
                return;
            }

            if (!_signList.Contains(subSign))
            {
                _signList.Add(subSign);
            }

            if (param != null)
            {
                if (_paramDic.ContainsKey(subSign))
                {
                    _paramDic[subSign] = param;
                }
                else
                {
                    _paramDic.Add(subSign, param);
                }
            }

            if (_signList.Count >= _signs.Length)
            {
                object[] paramList = new object[_paramDic.Count];

                for (int i = 0; i < _signs.Length; i++)
                {
                    if (_paramDic.ContainsKey(_signs[i]))
                    {
                        paramList[i] = _paramDic[_signs[i]];
                    }
                }

                var data = _calls.GetElement(1);

                while (data.right != 0)
                {
                    data = _calls.GetElement(data.right);
                    data.element(paramList);
                }

                _paramDic.Clear();
                _signList.Clear();
            }
        }
    }

    #region EventBaseAsync1

    public class EventBaseAsync1 : EventBase<Func<UniTask>>
    {
        public override void Invoke()
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke().Forget();
            }
        }
    }

    public class EventBaseAsync1<T1> : EventBase<Func<T1, UniTask>, T1>
    {
        public override void Invoke(T1 t1)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1).Forget();
            }
        }
    }

    public class EventBaseAsync1<T1, T2> : EventBase<Func<T1, T2, UniTask>, T1, T2>
    {
        public override void Invoke(T1 t1, T2 t2)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2).Forget();
            }
        }
    }

    public class EventBaseAsync1<T1, T2, T3> : EventBase<Func<T1, T2, T3, UniTask>, T1, T2, T3>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2, t3).Forget();
            }
        }
    }

    public class EventBaseAsync1<T1, T2, T3, T4> : EventBase<Func<T1, T2, T3, T4, UniTask>, T1, T2, T3, T4>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2, t3, t4).Forget();
            }
        }
    }

    #endregion EventBaseAsync1

    #region EventBase1

    public class EventBase1 : EventBase<Action>
    {
        public override void Invoke()
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke();
            }
        }
    }

    public class EventBase1<T1> : EventBase<Action<T1>, T1>
    {
        public override void Invoke(T1 t1)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1);
            }
        }
    }

    public class EventBase1<T1, T2> : EventBase<Action<T1, T2>, T1, T2>
    {
        public override void Invoke(T1 t1, T2 t2)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2);
            }
        }
    }

    public class EventBase1<T1, T2, T3> : EventBase<Action<T1, T2, T3>, T1, T2, T3>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2, t3);
            }
        }
    }

    public class EventBase1<T1, T2, T3, T4> : EventBase<Action<T1, T2, T3, T4>, T1, T2, T3, T4>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                data.element.Invoke(t1, t2, t3, t4);
            }
        }
    }

    #endregion EventBase1

    #region EventBaseAsync2

    public sealed class EventBaseAsync2 : EventBase<object>
    {
        public override void Invoke()
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Func<UniTask>).Invoke();
            }
        }
    }

    public sealed class EventBaseAsync2<T1> : EventBase<object, T1>
    {
        public override void Invoke(T1 t1)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Func<T1, UniTask>).Invoke(t1);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2> : EventBase<object, T1, T2>
    {
        public override void Invoke(T1 t1, T2 t2)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Func<T1, T2, UniTask>).Invoke(t1, t2);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2, T3> : EventBase<object, T1, T2, T3>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Func<T1, T2, T3, UniTask>).Invoke(t1, t2, t3);
            }
        }
    }

    public sealed class EventBaseAsync2<T1, T2, T3, T4> : EventBase<object, T1, T2, T3, T4>
    {
        public override void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Func<T1, T2, T3, T4, UniTask>).Invoke(t1, t2, t3, t4);
            }
        }
    }

    #endregion EventBaseAsync2

    #region EventBase2

    public sealed class EventBase2 : EventBase<object>
    {
    }

    public sealed class EventBase2<T1> : EventBase<object, T1>
    {
    }

    public sealed class EventBase2<T1, T2> : EventBase<object, T1, T2>
    {
    }

    public sealed class EventBase2<T1, T2, T3> : EventBase<object, T1, T2, T3>
    {
    }

    public sealed class EventBase2<T1, T2, T3, T4> : EventBase<object, T1, T2, T3, T4>
    {
    }

    #endregion EventBase2

    public class EventValue
    {
        public const int INITIAL_COUNT = 8;
    }

    public interface IEvent
    {
        int Count();

        void RemoveListener(Component self);

        void RemoveAllListener();
    }

    public class EventBase<B> : IEvent
    {
        protected StaticLinkedListDictionary<Component, B> _datas = new StaticLinkedListDictionary<Component, B>(0, EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return _datas.Length;
        }

        public virtual void AddListener(Component self, B call)
        {
            if (!_datas.ContainsKey(self))
            {
                _datas.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public virtual void RemoveListener(Component self)
        {
            _datas.Remove(self);
        }

        public void RemoveAllListener()
        {
            _datas.Clear();
        }

        public virtual void Invoke()
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Action).Invoke();
            }
        }
    }

    public class EventBase<B, T1> : IEvent
    {
        protected StaticLinkedListDictionary<Component, B> _datas = new StaticLinkedListDictionary<Component, B>(0, EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return _datas.Length;
        }

        public virtual void AddListener(Component self, B call)
        {
            if (!_datas.ContainsKey(self))
            {
                _datas.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public virtual void RemoveListener(Component self)
        {
            _datas.Remove(self);
        }

        public void RemoveAllListener()
        {
            _datas.Clear();
        }

        public virtual void Invoke(T1 t1)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Action<T1>).Invoke(t1);
            }
        }
    }

    public class EventBase<B, T1, T2> : IEvent
    {
        protected StaticLinkedListDictionary<Component, B> _datas = new StaticLinkedListDictionary<Component, B>(0, EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return _datas.Length;
        }

        public virtual void AddListener(Component self, B call)
        {
            if (!_datas.ContainsKey(self))
            {
                _datas.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public virtual void RemoveListener(Component self)
        {
            _datas.Remove(self);
        }

        public void RemoveAllListener()
        {
            _datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Action<T1, T2>).Invoke(t1, t2);
            }
        }
    }

    public class EventBase<B, T1, T2, T3> : IEvent
    {
        protected StaticLinkedListDictionary<Component, B> _datas = new StaticLinkedListDictionary<Component, B>(0, EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return _datas.Length;
        }

        public virtual void AddListener(Component self, B call)
        {
            if (!_datas.ContainsKey(self))
            {
                _datas.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public virtual void RemoveListener(Component self)
        {
            _datas.Remove(self);
        }

        public void RemoveAllListener()
        {
            _datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2, T3 t3)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Action<T1, T2, T3>).Invoke(t1, t2, t3);
            }
        }
    }

    public class EventBase<B, T1, T2, T3, T4> : IEvent
    {
        protected StaticLinkedListDictionary<Component, B> _datas = new StaticLinkedListDictionary<Component, B>(0, EventValue.INITIAL_COUNT);

        public EventBase()
        {
        }

        public int Count()
        {
            return _datas.Length;
        }

        public virtual void AddListener(Component self, B call)
        {
            if (!_datas.ContainsKey(self))
            {
                _datas.Add(self, call);

                if (!self.EventList.Contains(this))
                {
                    self.EventList.Add(this);
                }
            }
        }

        public virtual void RemoveListener(Component self)
        {
            _datas.Remove(self);
        }

        public void RemoveAllListener()
        {
            _datas.Clear();
        }

        public virtual void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var data = _datas.GetElement(1);

            while (data.right != 0)
            {
                data = _datas.GetElement(data.right);
                (data.element as Action<T1, T2, T3, T4>).Invoke(t1, t2, t3, t4);
            }
        }
    }
}