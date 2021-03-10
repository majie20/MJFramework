using System;
using System.Collections.Generic;

namespace MGame.Model
{
    public class EntityPoolComponent : Component
    {
        private Dictionary<Type, Queue<Entity>> entityDic;

        public EntityPoolComponent()
        {
        }

        public override Component Init()
        {
            base.Init();
            entityDic = new Dictionary<Type, Queue<Entity>>();
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            entityDic = null;
        }

        public T GetEntity<T>() where T : Entity, new()
        {
            return (T)GetEntity(typeof(T));
        }

        public Entity GetEntity(Type type)
        {
            if (entityDic.ContainsKey(type))
            {
                return entityDic[type].Dequeue();
            }

            var component = (Entity)Activator.CreateInstance(type);

            return component;
        }

        public void RecycleEntity(Entity entity)
        {
            var type = entity.GetType();
            Queue<Entity> queue;
            if (entityDic.ContainsKey(type))
            {
                queue = entityDic[type];
            }
            else
            {
                queue = new Queue<Entity>();
                entityDic.Add(type, queue);
            }

            queue.Enqueue(entity);
        }
    }
}