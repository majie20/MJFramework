using System;
using System.Collections.Generic;

namespace Model
{
    public class LifecycleSystem : IDisposable
    {
        private HashSet<Type> updateSystems = new HashSet<Type>();
        private HashSet<Type> lateUpdateSystems = new HashSet<Type>();
        private HashSet<Type> startSystems = new HashSet<Type>();
        private Dictionary<Type, List<Type>> types = new Dictionary<Type, List<Type>>();

        private StaticLinkedListDictionary<Component, IStartSystem> starts =
            new StaticLinkedListDictionary<Component, IStartSystem>(50);

        private StaticLinkedListDictionary<Component, IUpdateSystem> updates =
            new StaticLinkedListDictionary<Component, IUpdateSystem>(50);

        private StaticLinkedListDictionary<Component, ILateUpdateSystem> lateUpdates =
            new StaticLinkedListDictionary<Component, ILateUpdateSystem>(50);

        public LifecycleSystem()
        {
            types.Clear();
            updateSystems.Clear();
            lateUpdateSystems.Clear();
            startSystems.Clear();
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

            if (types.ContainsKey(typeof(LifeCycleAttribute)))
            {
                foreach (var v in types[typeof(LifeCycleAttribute)])
                {
                    object obj = Activator.CreateInstance(v);

                    if (obj is IUpdateSystem)
                    {
                        updateSystems.Add(v);
                    }
                    if (obj is ILateUpdateSystem)
                    {
                        lateUpdateSystems.Add(v);
                    }
                    if (obj is IStartSystem)
                    {
                        startSystems.Add(v);
                    }
                }
            }
        }

        public void Dispose()
        {
            updateSystems = null;
            lateUpdateSystems = null;
            startSystems = null;
            starts = null;
            updates = null;
            lateUpdates = null;
            types = null;
        }

        public void Add(Component component)
        {
            Type type = component.GetType();

            if (this.startSystems.Contains(type) && !this.starts.ContainsKey(component))
            {
                this.starts.Add(component, component as IStartSystem);
            }
            if (this.updateSystems.Contains(type) && !this.updates.ContainsKey(component))
            {
                this.updates.Add(component, component as IUpdateSystem);
            }
            if (this.lateUpdateSystems.Contains(type) && !this.lateUpdates.ContainsKey(component))
            {
                this.lateUpdates.Add(component, component as ILateUpdateSystem);
            }
        }

        public void Remove(Component component)
        {
            if (this.starts.ContainsKey(component))
            {
                this.starts.Remove(component);
            }
            if (this.updates.ContainsKey(component))
            {
                this.updates.Remove(component);
            }
            if (this.lateUpdates.ContainsKey(component))
            {
                this.lateUpdates.Remove(component);
            }
        }

        public void Start()
        {
            var data = this.starts[1];
            while (data.right != 0)
            {
                data = this.starts[data.right];
                data.element.Start();
            }
        }

        public void Update(float tick)
        {
            this.Start();

            var data = this.updates[1];
            while (data.right != 0)
            {
                data = this.updates[data.right];
                data.element.OnUpdate(tick);
            }
        }

        public void LateUpdate()
        {
            var data = this.lateUpdates[1];
            while (data.right != 0)
            {
                data = this.lateUpdates[data.right];
                data.element.OnLateUpdate();
            }
        }
    }
}