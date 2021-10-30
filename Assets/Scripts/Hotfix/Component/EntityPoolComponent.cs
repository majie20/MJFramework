using System.Collections.Generic;

namespace MGame.Hotfix
{
    [LifeCycle]
    public class EntityPoolComponent : Component, IAwake
    {
        private Queue<Entity> entityDic;

        public void Awake()
        {
            entityDic = new Queue<Entity>(50);
        }

        public override void Dispose()
        {
            base.Dispose();
            entityDic = null;
        }

        public Entity HatchEntity()
        {
            if (entityDic.Count > 0)
            {
                return entityDic.Dequeue();
            }

            var component = new Entity();

            return component;
        }

        public void RecycleEntity(Entity entity)
        {
            entityDic.Enqueue(entity);
        }
    }
}