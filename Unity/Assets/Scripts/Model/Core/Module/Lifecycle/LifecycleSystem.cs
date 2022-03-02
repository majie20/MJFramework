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

        private ArrayQueue<Component> starts = new ArrayQueue<Component>(50);
        private ArrayQueue<Component> updates = new ArrayQueue<Component>(50);
        private ArrayQueue<Component> lateUpdates = new ArrayQueue<Component>(50);
        private ArrayQueue<Component> temps = new ArrayQueue<Component>(50);

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

            updateSystems.Clear();
            lateUpdateSystems.Clear();
            startSystems.Clear();

            if (types.ContainsKey(typeof(LifeCycleAttribute)))
            {
                foreach (var v in types[typeof(LifeCycleAttribute)])
                {
                    object obj = Activator.CreateInstance(v);

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

        public void Remove(Component component)
        {
            Type type = component.GetType();

            if (this.starts.Contains(component))
            {
                temps.Clear();
                while (this.starts.GetSize() > 0)
                {
                    var _component = this.starts.Dequeue();
                    if (_component != component)
                    {
                        temps.Enqueue(_component);
                    }
                }
                while (this.temps.GetSize() > 0)
                {
                    starts.Enqueue(this.temps.Dequeue());
                }
            }

            if (this.updates.Contains(component))
            {
                temps.Clear();
                while (this.updates.GetSize() > 0)
                {
                    var _component = this.updates.Dequeue();
                    if (_component != component)
                    {
                        temps.Enqueue(_component);
                    }
                }
                while (this.temps.GetSize() > 0)
                {
                    updates.Enqueue(this.temps.Dequeue());
                }
            }

            if (this.lateUpdates.Contains(component))
            {
                temps.Clear();
                while (this.lateUpdates.GetSize() > 0)
                {
                    var _component = this.lateUpdates.Dequeue();
                    if (_component != component)
                    {
                        temps.Enqueue(_component);
                    }
                }
                while (this.temps.GetSize() > 0)
                {
                    lateUpdates.Enqueue(this.temps.Dequeue());
                }
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
    }
}