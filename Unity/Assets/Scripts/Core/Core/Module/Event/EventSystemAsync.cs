using Cysharp.Threading.Tasks;
using System;

namespace Model
{
    public sealed partial class EventSystem
    {
        #region 订阅-异步

        public void AddListenerAsync<T1>(Component self, Func<UniTask> call) where T1 : EventBaseAsync1, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListenerAsync<T1, T2>(Component self, Func<T2, UniTask> call) where T1 : EventBaseAsync1<T2>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListenerAsync<T1, T2, T3>(Component self, Func<T2, T3, UniTask> call) where T1 : EventBaseAsync1<T2, T3>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListenerAsync<T1, T2, T3, T4>(Component self, Func<T2, T3, T4, UniTask> call) where T1 : EventBaseAsync1<T2, T3, T4>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListenerAsync<T1, T2, T3, T4, T5>(Component self, Func<T2, T3, T4, T5, UniTask> call) where T1 : EventBaseAsync1<T2, T3, T4, T5>, new()
        {
            AddEventModel<T1>().AddListener(self, call);
        }

        public void AddListenerAsync(uint sign, Component self, Func<UniTask> call)
        {
            AddEventModel<EventBaseAsync2>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2>(uint sign, Component self, Func<T2, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3>(uint sign, Component self, Func<T2, T3, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3, T4>(uint sign, Component self, Func<T2, T3, T4, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3, T4>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3, T4, T5>(uint sign, Component self, Func<T2, T3, T4, T5, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3, T4, T5>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync(string sign, Component self, Func<UniTask> call)
        {
            AddEventModel<EventBaseAsync2>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2>(string sign, Component self, Func<T2, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3>(string sign, Component self, Func<T2, T3, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3, T4>(string sign, Component self, Func<T2, T3, T4, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3, T4>>(sign).AddListener(self, call);
        }

        public void AddListenerAsync<T2, T3, T4, T5>(string sign, Component self, Func<T2, T3, T4, T5, UniTask> call)
        {
            AddEventModel<EventBaseAsync2<T2, T3, T4, T5>>(sign).AddListener(self, call);
        }

        #endregion 订阅-异步

        #region 移除-异步

        public void RemoveListenerAsync<T1>(Component self) where T1 : EventBaseAsync1, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2>(Component self) where T1 : EventBaseAsync1<T2>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3>(Component self) where T1 : EventBaseAsync1<T2, T3>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3, T4>(Component self) where T1 : EventBaseAsync1<T2, T3, T4>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3, T4, T5>(Component self) where T1 : EventBaseAsync1<T2, T3, T4, T5>, new()
        {
            GetEventModel<T1>()?.RemoveListener(self);
        }

        public void RemoveListenerAsync(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3, T4>(uint sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        public void RemoveListenerAsync<T1, T2, T3, T4>(string sign, Component self)
        {
            GetEventModel(sign)?.RemoveListener(self);
        }

        #endregion 移除-异步

        #region 发布-异步

        public void InvokeAsync<T1>() where T1 : EventBaseAsync1, new()
        {
            GetEventModel<T1>()?.Invoke();
        }

        public void InvokeAsync<T1, T2>(T2 t2) where T1 : EventBaseAsync1<T2>, new()
        {
            GetEventModel<T1>()?.Invoke(t2);
        }

        public void InvokeAsync<T1, T2, T3>(T2 t2, T3 t3) where T1 : EventBaseAsync1<T2, T3>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3);
        }

        public void InvokeAsync<T1, T2, T3, T4>(T2 t2, T3 t3, T4 t4) where T1 : EventBaseAsync1<T2, T3, T4>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4);
        }

        public void InvokeAsync<T1, T2, T3, T4, T5>(T2 t2, T3 t3, T4 t4, T5 t5) where T1 : EventBaseAsync1<T2, T3, T4, T5>, new()
        {
            GetEventModel<T1>()?.Invoke(t2, t3, t4, t5);
        }

        public void InvokeAsync(uint sign)
        {
            (GetEventModel(sign) as EventBaseAsync2)?.Invoke();
        }

        public void InvokeAsync<T1>(uint sign, T1 t1)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1>)?.Invoke(t1);
        }

        public void InvokeAsync<T1, T2>(uint sign, T1 t1, T2 t2)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2>)?.Invoke(t1, t2);
        }

        public void InvokeAsync<T1, T2, T3>(uint sign, T1 t1, T2 t2, T3 t3)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2, T3>)?.Invoke(t1, t2, t3);
        }

        public void InvokeAsync<T1, T2, T3, T4>(uint sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2, T3, T4>)?.Invoke(t1, t2, t3, t4);
        }

        public void InvokeAsync(string sign)
        {
            (GetEventModel(sign) as EventBaseAsync2)?.Invoke();
        }

        public void InvokeAsync<T1>(string sign, T1 t1)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1>)?.Invoke(t1);
        }

        public void InvokeAsync<T1, T2>(string sign, T1 t1, T2 t2)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2>)?.Invoke(t1, t2);
        }

        public void InvokeAsync<T1, T2, T3>(string sign, T1 t1, T2 t2, T3 t3)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2, T3>)?.Invoke(t1, t2, t3);
        }

        public void InvokeAsync<T1, T2, T3, T4>(string sign, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            (GetEventModel(sign) as EventBaseAsync2<T1, T2, T3, T4>)?.Invoke(t1, t2, t3, t4);
        }

        #endregion 发布-异步
    }
}