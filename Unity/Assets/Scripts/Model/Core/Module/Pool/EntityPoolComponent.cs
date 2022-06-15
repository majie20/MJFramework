using System;
using System.Collections.Generic;

namespace Model
{
    public class EntityPoolComponent : Component, IAwake, ILateUpdateSystem
    {
        private Dictionary<Type, Queue<Entity>> _entityDic;
        private List<Entity> _entitys;

        public void Awake()
        {
            _entityDic = new Dictionary<Type, Queue<Entity>>();
            _entitys = new List<Entity>();
        }

        public override void Dispose()
        {
            _entityDic = null;
            _entitys = null;
            base.Dispose();
        }

        public void OnLateUpdate()
        {
            if (_entitys.Count == 0)
            {
                return;
            }
            for (int i = 0; i < _entitys.Count; i++)
            {
                var entity = _entitys[i];
                var type = entity.GetType();

                Queue<Entity> queue;
                if (_entityDic.ContainsKey(type))
                {
                    queue = _entityDic[type];
                }
                else
                {
                    queue = new Queue<Entity>();
                    _entityDic.Add(type, queue);
                }

                queue.Enqueue(entity);
            }
            _entitys.Clear();
        }

        public T HatchEntity<T>() where T : Entity
        {
            return (T)HatchEntity(typeof(T));
        }

        public Entity HatchEntity(Type type)
        {
            if (_entityDic.ContainsKey(type))
            {
                return _entityDic[type].Dequeue();
            }

            Entity entity = (Entity)Activator.CreateInstance(type);

            return entity;
        }

        public void RecycleEntity(Entity entity)
        {
            _entitys.Add(entity);
        }
    }
}