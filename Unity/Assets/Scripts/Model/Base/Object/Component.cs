using System;

namespace Model
{
    public class Component : IDisposable
    {
        private Entity entity;

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

        public Component()
        {
        }

        public virtual void Dispose()
        {
            Entity.RemoveComponent(this);
            Entity = null;
        }
    }
}