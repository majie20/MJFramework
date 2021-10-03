using System;
using System.Collections.Generic;

namespace MGame.Hotfix
{
    public class LifecycleSystem
    {
        private readonly Dictionary<Type, IAwakeSystem> awakeSystems = new Dictionary<Type, IAwakeSystem>();
        private readonly Dictionary<Type, IUpdateSystem> updateSystems = new Dictionary<Type, IUpdateSystem>();
        private readonly Dictionary<Type, ILateUpdateSystem> lateUpdateSystems = new Dictionary<Type, ILateUpdateSystem>();
        private readonly Dictionary<Type, IStartSystem> startSystems = new Dictionary<Type, IStartSystem>();
        private readonly Dictionary<Type, IDestroySystem> destroySystems = new Dictionary<Type, IDestroySystem>();

        private Queue<Component> starts = new Queue<Component>();
        private Queue<Component> updates = new Queue<Component>();
        private Queue<Component> lateUpdates = new Queue<Component>();

        public LifecycleSystem Init()
        {
            awakeSystems.Clear();
            updateSystems.Clear();
            lateUpdateSystems.Clear();
            startSystems.Clear();
            destroySystems.Clear();

            foreach (var v in Model.Game.Instance.Hotfix.GetHotfixTypes())
            {
                var atts = v.GetCustomAttributes(typeof(LifeCycleAttribute), false);
                if (atts.Length != 0)
                {
                    object obj = Activator.CreateInstance(v);

                    if (obj is IAwakeSystem awakeSystem)
                    {
                        awakeSystems.Add(v, awakeSystem);
                    }
                    if (obj is IUpdateSystem updateSystem)
                    {
                        updateSystems.Add(v, updateSystem);
                    }
                    if (obj is ILateUpdateSystem lateUpdateSystem)
                    {
                        lateUpdateSystems.Add(v, lateUpdateSystem);
                    }
                    if (obj is IStartSystem startSystem)
                    {
                        startSystems.Add(v, startSystem);
                    }
                    if (obj is IDestroySystem destroySystem)
                    {
                        destroySystems.Add(v, destroySystem);
                    }
                }
            }

            return this;
        }

        public void Dispose()
        {
        }

        public void Add(Component component)
        {
            Type type = component.GetType();

            if (this.startSystems.ContainsKey(type))
            {
                this.starts.Enqueue(component);
            }
            if (this.updateSystems.ContainsKey(type))
            {
                this.updates.Enqueue(component);
            }
            if (this.lateUpdateSystems.ContainsKey(type))
            {
                this.lateUpdates.Enqueue(component);
            }
        }

        public void Start()
        {
            while (this.starts.Count > 0)
            {
                var component = this.starts.Dequeue();
                if (startSystems.TryGetValue(component.GetType(), out IStartSystem iStartSystem))
                {
                    iStartSystem?.Start();
                }
            }
        }

        public void Update(float tick)
        {
            this.Start();
            foreach (var component in updates)
            {
                if (updateSystems.TryGetValue(component.GetType(), out IUpdateSystem iUpdateSystem))
                {
                    iUpdateSystem?.OnUpdate(tick);
                }
            }
        }

        public void LateUpdate()
        {
            foreach (var component in lateUpdates)
            {
                if (lateUpdateSystems.TryGetValue(component.GetType(), out ILateUpdateSystem iLateUpdateSystem))
                {
                    iLateUpdateSystem?.OnLateUpdate();
                }
            }
        }

        #region Awake

        public void Awake(Component component)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake iAwake = iAwakeSystem as IAwake;

                iAwake?.Awake();
            }
        }

        public void Awake<A>(Component component, A a)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake<A> iAwake = iAwakeSystem as IAwake<A>;

                iAwake?.Awake(a);
            }
        }

        public void Awake<A, B>(Component component, A a, B b)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake<A, B> iAwake = iAwakeSystem as IAwake<A, B>;

                iAwake?.Awake(a, b);
            }
        }

        public void Awake<A, B, C>(Component component, A a, B b, C c)
        {
            if (awakeSystems.TryGetValue(component.GetType(), out IAwakeSystem iAwakeSystem))
            {
                IAwake<A, B, C> iAwake = iAwakeSystem as IAwake<A, B, C>;

                iAwake?.Awake(a, b, c);
            }
        }

        #endregion Awake

        #region Destroy

        public void Destroy(Component component)
        {
            if (destroySystems.TryGetValue(component.GetType(), out IDestroySystem iDestroySystem))
            {
                IDestroy iAwake = iDestroySystem as IDestroy;

                iAwake?.Destroy();
            }
        }

        public void Destroy<A>(Component component, A a)
        {
            if (destroySystems.TryGetValue(component.GetType(), out IDestroySystem iDestroySystem))
            {
                IDestroy<A> iAwake = iDestroySystem as IDestroy<A>;

                iAwake?.Destroy(a);
            }
        }

        public void Destroy<A, B>(Component component, A a, B b)
        {
            if (destroySystems.TryGetValue(component.GetType(), out IDestroySystem iDestroySystem))
            {
                IDestroy<A, B> iAwake = iDestroySystem as IDestroy<A, B>;

                iAwake?.Destroy(a, b);
            }
        }

        public void Destroy<A, B, C>(Component component, A a, B b, C c)
        {
            if (destroySystems.TryGetValue(component.GetType(), out IDestroySystem iDestroySystem))
            {
                IDestroy<A, B, C> iAwake = iDestroySystem as IDestroy<A, B, C>;

                iAwake?.Destroy(a, b, c);
            }
        }

        #endregion Destroy
    }
}