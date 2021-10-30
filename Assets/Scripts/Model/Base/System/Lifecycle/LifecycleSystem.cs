using System;
using System.Collections.Generic;

namespace MGame.Model
{
    public class LifecycleSystem : IDisposable
    {
        private readonly HashSet<Type> awakeSystems = new HashSet<Type>();
        private readonly HashSet<Type> updateSystems = new HashSet<Type>();
        private readonly HashSet<Type> lateUpdateSystems = new HashSet<Type>();
        private readonly HashSet<Type> startSystems = new HashSet<Type>();
        private readonly HashSet<Type> destroySystems = new HashSet<Type>();
        private Dictionary<Type, List<Type>> types = new Dictionary<Type, List<Type>>();

        private Queue<Component> starts = new Queue<Component>();
        private Queue<Component> updates = new Queue<Component>();
        private Queue<Component> lateUpdates = new Queue<Component>();

        public LifecycleSystem()
        {
            types.Clear();
            var assembly = typeof(Init).Assembly;

            foreach (var v in assembly.GetTypes())
            {
                object[] objects = v.GetCustomAttributes(typeof(BaseAttribute), false);
                if (objects.Length != 0)
                {
                    for (int i = 0; i < objects.Length; i++)
                    {
                        Type type = ((BaseAttribute)objects[i]).AttributeType;
                        if (!types.ContainsKey(type))
                        {
                            types.Add(type, new List<Type>());
                        }
                        if (!types[type].Contains(v))
                        {
                            types[type].Add(v);
                        }
                    }
                }
            }

            awakeSystems.Clear();
            updateSystems.Clear();
            lateUpdateSystems.Clear();
            startSystems.Clear();
            destroySystems.Clear();

            if (types.ContainsKey(typeof(LifeCycleAttribute)))
            {
                foreach (var v in types[typeof(LifeCycleAttribute)])
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
            while (this.starts.Count > 0)
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
            foreach (var component in updates)
            {
                if (updateSystems.Contains(component.GetType()))
                {
                    (component as IUpdateSystem)?.OnUpdate(tick);
                }
            }
        }

        public void LateUpdate()
        {
            foreach (var component in lateUpdates)
            {
                if (lateUpdateSystems.Contains(component.GetType()))
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