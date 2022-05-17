using System;
using System.Collections.Generic;

namespace Hotfix
{
    public class LifecycleSystem : IDisposable
    {
        private HashSet<Type> updateSystems = new HashSet<Type>();
        private HashSet<Type> lateUpdateSystems = new HashSet<Type>();
        private HashSet<Type> startSystems = new HashSet<Type>();

        private StaticLinkedListDictionary<Model.Component, IStartSystem> starts =
            new StaticLinkedListDictionary<Model.Component, IStartSystem>(50);

        private StaticLinkedListDictionary<Model.Component, IUpdateSystem> updates =
            new StaticLinkedListDictionary<Model.Component, IUpdateSystem>(50);

        private StaticLinkedListDictionary<Model.Component, ILateUpdateSystem> lateUpdates =
            new StaticLinkedListDictionary<Model.Component, ILateUpdateSystem>(50);

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
        }

        public void Add(Model.Component component)
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

        public void Remove(Model.Component component)
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