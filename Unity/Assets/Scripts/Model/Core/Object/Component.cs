using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Model
{
    public class Component : IDisposable
    {
        protected long guid;

        public long Guid
        {
            protected set
            {
                guid = value;
            }
            get
            {
                return guid;
            }
        }

        protected Entity entity;

        public Entity Entity
        {
            set
            {
                entity = value;
            }
            get
            {
                return entity;
            }
        }

        protected bool isRuning;

        public bool IsRuning
        {
            set
            {
                isRuning = value;
            }
            get
            {
                return isRuning;
            }
        }

        protected Dictionary<Type, Component> componentChildDic;

        #region Event

        protected HashSet<IEvent> eventList;

        public HashSet<IEvent> EventList
        {
            protected set
            {
                eventList = value;
            }
            get
            {
                return eventList;
            }
        }

        protected HashSet<EventGroup<uint>> eventGroupList;

        public HashSet<EventGroup<uint>> EventGroupList
        {
            protected set
            {
                eventGroupList = value;
            }
            get
            {
                return eventGroupList;
            }
        }

        #endregion Event

        #region Task

        public bool awakeCalled = false;
        public bool called = false;
        protected CancellationTokenSource cancellationTokenSource;

        public CancellationToken CancellationToken
        {
            get
            {
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                }

                if (!awakeCalled)
                {
                    PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
                }

                return cancellationTokenSource.Token;
            }
        }

        private class AwakeMonitor : IPlayerLoopItem
        {
            private readonly Component trigger;

            public AwakeMonitor(Component trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.called) return false;
                if (trigger == null)
                {
                    trigger.Dispose();
                    return false;
                }
                return true;
            }
        }

        #endregion Task

        public Component()
        {
            EventList = new HashSet<IEvent>();
            EventGroupList = new HashSet<EventGroup<uint>>();

            componentChildDic = new Dictionary<Type, Component>();

            Guid = GuidHelper.GuidToLongID();
        }

        public virtual void Dispose()
        {
            foreach (var v in componentChildDic)
            {
                ObjectHelper.RemoveComponent(v.Key, this.Entity);
            }
            componentChildDic.Clear();

            Entity = null;
            foreach (var e in EventList)
            {
                e.RemoveListener(this);
            }
            foreach (var e in EventGroupList)
            {
                e.RemoveListener(this);
            }
            EventList.Clear();
            EventGroupList.Clear();

            called = true;
            awakeCalled = false;

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public void AddComponentParent()
        {
            var attr2 = GetComponentOfAttribute();

            if (attr2 != null && attr2.Types.Length > 0)
            {
                var types = attr2.Types;

                for (var i = types.Length - 1; i >= 0; i--)
                {
                    AddComponentParent(types[i]);
                }
            }
        }

        private void AddComponentParent(Type type)
        {
            Component parent;
            if (this.Entity.HasComponent(type))
            {
                parent = this.Entity.GetComponent(type);
            }
            else
            {
                parent = ObjectHelper.CreateComponent(type, this.Entity);
            }

            parent.AddComponentChild(this);
        }

        private void AddComponentChild(Component component)
        {
            var type = component.GetType();
            if (this.Entity.HasComponent(type))
            {
                if (!this.componentChildDic.ContainsKey(type))
                {
                    this.componentChildDic.Add(type, component);
                }
            }
        }

        private ComponentOfAttribute GetComponentOfAttribute()
        {
            var type = this.GetType();
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                var attrs = type.GetCustomAttributes(typeof(ComponentOfAttribute), false);
                if (attrs.Length > 0)
                {
                    if (attrs[0] is ComponentOfAttribute attr)
                    {
                        return attr;
                    }
                }
            }
            else
            {
                return type.GetCustomAttribute<ComponentOfAttribute>();
            }

            return null;
        }
    }
}