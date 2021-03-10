namespace MGame.Model
{
    public class Component
    {
        public long id { set; get; }

        public Entity entity { set; get; }

        public virtual Component Init()
        {
            return this;
        }

        public virtual Component Init(Entity entity)
        {
            this.entity = entity;
            return Init();
        }

        public virtual void Dispose()
        {
            entity = null;
        }
    }
}