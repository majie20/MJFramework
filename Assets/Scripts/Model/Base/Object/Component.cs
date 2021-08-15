namespace MGame.Model
{
    public class Component
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

        public virtual Component Init()
        {
            return this;
        }

        public virtual Component Init(Entity entity)
        {
            this.Entity = entity;
            return Init();
        }

        public virtual void Dispose()
        {
            Entity = null;
        }
    }
}