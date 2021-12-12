using System;
using System.Collections.Generic;

namespace Hotfix
{
    public class LifecycleSystem : IDisposable
    {
        private readonly HashSet<Type> awakeSystems = new HashSet<Type>();
        private readonly HashSet<Type> updateSystems = new HashSet<Type>();
        private readonly HashSet<Type> lateUpdateSystems = new HashSet<Type>();
        private readonly HashSet<Type> startSystems = new HashSet<Type>();
        private readonly HashSet<Type> destroySystems = new HashSet<Type>();

        private ArrayQueue<Component> starts = new ArrayQueue<Component>(20);
        private ArrayQueue<Component> updates = new ArrayQueue<Component>(20);
        private ArrayQueue<Component> lateUpdates = new ArrayQueue<Component>(20);

        public LifecycleSystem()
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
                        awakeSystems.Add(v);
                    }
                    if (obj is IUpdateSystem updateSystem)
                    {
                        updateSystems.Add(v);
                    }
                    if (obj is ILateUpdateSystem lateUpdateSystem)
                    {
                        lateUpdateSystems.Add(v);
                    }
                    if (obj is IStartSystem startSystem)
                    {
                        startSystems.Add(v);
                    }
                    if (obj is IDestroySystem destroySystem)
                    {
                        destroySystems.Add(v);
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        public void Add(Component component)
        {
            Type type = component.GetType();

            if (this.startSystems.Contains(type))
            {
                this.starts.Enqueue(component);
            }
            if (this.updateSystems.Contains(type))
            {
                this.updates.Enqueue(component);
            }
            if (this.lateUpdateSystems.Contains(type))
            {
                this.lateUpdates.Enqueue(component);
            }
        }

        public void Start()
        {
            while (this.starts.GetSize() > 0)
            {
                var component = this.starts.Dequeue();
                if (startSystems.Contains(component.GetType()))
                {
                    (component as IStartSystem)?.Start();
                }
            }
        }

        public void Update(float tick)
        {
            this.Start();
            for (int i = 0; i < this.updates.GetSize(); i++)
            {
                var component = this.updates.Peek(i);
                if (updateSystems.Contains(component.GetType()))
                {
                    (component as IUpdateSystem)?.OnUpdate(tick);
                }
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < this.lateUpdates.GetSize(); i++)
            {
                var component = this.lateUpdates.Peek(i);
                if (updateSystems.Contains(component.GetType()))
                {
                    (component as ILateUpdateSystem)?.OnLateUpdate();
                }
            }
        }

        #region Awake

        public void Awake(Component component)
        {
            if (awakeSystems.Contains(component.GetType()))
            {
                IAwake iAwake = component as IAwake;

                iAwake?.Awake();
            }
        }

        public void Awake<A>(Component component, A a)
        {
            if (awakeSystems.Contains(component.GetType()))
            {
                IAwake<A> iAwake = component as IAwake<A>;

                iAwake?.Awake(a);
            }
        }

        public void Awake<A, B>(Component component, A a, B b)
        {
            if (awakeSystems.Contains(component.GetType()))
            {
                IAwake<A, B> iAwake = component as IAwake<A, B>;

                iAwake?.Awake(a, b);
            }
        }

        public void Awake<A, B, C>(Component component, A a, B b, C c)
        {
            if (awakeSystems.Contains(component.GetType()))
            {
                IAwake<A, B, C> iAwake = component as IAwake<A, B, C>;

                iAwake?.Awake(a, b, c);
            }
        }

        #endregion Awake

        #region Destroy

        public void Destroy(Component component)
        {
            if (destroySystems.Contains(component.GetType()))
            {
                IDestroy iAwake = component as IDestroy;

                iAwake?.Destroy();
            }
        }

        public void Destroy<A>(Component component, A a)
        {
            if (destroySystems.Contains(component.GetType()))
            {
                IDestroy<A> iAwake = component as IDestroy<A>;

                iAwake?.Destroy(a);
            }
        }

        public void Destroy<A, B>(Component component, A a, B b)
        {
            if (destroySystems.Contains(component.GetType()))
            {
                IDestroy<A, B> iAwake = component as IDestroy<A, B>;

                iAwake?.Destroy(a, b);
            }
        }

        public void Destroy<A, B, C>(Component component, A a, B b, C c)
        {
            if (destroySystems.Contains(component.GetType()))
            {
                IDestroy<A, B, C> iAwake = component as IDestroy<A, B, C>;

                iAwake?.Destroy(a, b, c);
            }
        }

        #endregion Destroy
    }
}