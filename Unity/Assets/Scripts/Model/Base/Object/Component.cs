using System;

namespace Model
{
    public class Component : IDisposable
    {
        private long guid;

        public long Guid
        {
            private set
            {
                guid = value;
            }
            get
            {
                return guid;
            }
        }

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
            Guid = GuidHelper.GuidToLongID();
        }

        public virtual void Dispose()
        {
            Entity = null;
        }
    }
}