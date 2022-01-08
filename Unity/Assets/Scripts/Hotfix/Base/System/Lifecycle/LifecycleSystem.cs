using System;
using System.Collections.Generic;

namespace Hotfix
{
    public class LifecycleSystem : IDisposable
    {
        private readonly HashSet<Type> updateSystems = new HashSet<Type>();
        private readonly HashSet<Type> lateUpdateSystems = new HashSet<Type>();
        private readonly HashSet<Type> startSystems = new HashSet<Type>();

        private ArrayQueue<Model.Component> starts = new ArrayQueue<Model.Component>(20);
        private ArrayQueue<Model.Component> updates = new ArrayQueue<Model.Component>(20);
        private ArrayQueue<Model.Component> lateUpdates = new ArrayQueue<Model.Component>(20);
        private ArrayQueue<Model.Component> temps = new ArrayQueue<Model.Component>(20);

        public LifecycleSystem()
        {
            updateSystems.Clear();
            lateUpdateSystems.Clear();
            startSystems.Clear();

            foreach (var v in Model.Game.Instance.Hotfix.GetHotfixTypes())
            {
                var atts = v.GetCustomAttributes(typeof(Model.LifeCycleAttribute), false);
                if (atts.Length != 0)
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

        public void Add(Model.Component component)
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

        public void Remove(Model.Component component)
        {
            Type type = component.GetType();

            if (this.startSystems.Contains(type))
            {
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
            }
            if (this.updateSystems.Contains(type))
            {
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
            }
            if (this.lateUpdateSystems.Contains(type))
            {
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