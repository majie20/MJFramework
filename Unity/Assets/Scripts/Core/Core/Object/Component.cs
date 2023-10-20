using System;
using System.Collections.Generic;
using System.Reflection;

namespace Model
{
    public class Component : IDisposable
    {
        protected long _guid;

        public long Guid
        {
            protected set { _guid = value; }
            get { return _guid; }
        }

        protected Entity _entity;

        public Entity Entity
        {
            set { _entity = value; }
            get { return _entity; }
        }

        protected bool _isRuning;

        public bool IsRuning
        {
            set { _isRuning = value; }
            get { return _isRuning; }
        }

        protected Dictionary<Type, Component> _componentChildDic;

        #region Event

        protected HashSet<IEvent> _eventList;

        public HashSet<IEvent> EventList
        {
            protected set { _eventList = value; }
            get { return _eventList; }
        }

        #endregion Event

        public Component()
        {
            EventList = new HashSet<IEvent>();

            _componentChildDic = new Dictionary<Type, Component>();

            Guid = GuidHelper.GuidToLongID();
        }

        public virtual void Dispose()
        {
            foreach (var v in _componentChildDic)
            {
                ObjectHelper.RemoveComponent(v.Key, this.Entity);
            }

            _componentChildDic.Clear();

            foreach (var v in EventList)
            {
                v.RemoveListener(this);
            }

            EventList.Clear();

            Entity = null;
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
                if (!this._componentChildDic.ContainsKey(type))
                {
                    this._componentChildDic.Add(type, component);
                }
            }
        }

        private ComponentOfAttribute GetComponentOfAttribute()
        {
            var type = this.GetType();
#if ILRuntime
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
#else
            return type.GetCustomAttribute<ComponentOfAttribute>();
#endif
        }
    }
}