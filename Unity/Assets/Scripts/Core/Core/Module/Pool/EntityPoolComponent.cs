using System;
using System.Collections.Generic;

namespace Model
{
    public class EntityPoolComponent : Component, IAwake, ILateUpdateSystem
    {
        private Dictionary<Type, Queue<Entity>> _entityDic;
        private List<Entity>                    _entitys;
        private Dictionary<long, Entity>        _allEntityMap;

        public void Awake()
        {
            _allEntityMap = new Dictionary<long, Entity>(10);
            _entityDic = new Dictionary<Type, Queue<Entity>>();
            _entitys = new List<Entity>();
        }

        public override void Dispose()
        {
            _allEntityMap = null;
            _entityDic = null;
            _entitys = null;
            base.Dispose();
        }

        [FunctionSort(FunctionLayer.Low)]
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

                if (!_entityDic.TryGetValue(type, out queue))
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
            if (_entityDic.TryGetValue(type, out var queue))
            {
                Entity e = queue.Dequeue();
                _allEntityMap.Remove(e.Guid);
                e.Guid = GuidHelper.GuidToLongID();
                _allEntityMap.Add(e.Guid, e);

                return e;
            }

            Entity entity = (Entity)Activator.CreateInstance(type);
            entity.Guid = GuidHelper.GuidToLongID();
            _allEntityMap.Add(entity.Guid, entity);

            return entity;
        }

        public void RecycleEntity(Entity entity)
        {
            _entitys.Add(entity);
        }

        public Entity GetEntity(long guid)
        {
            if (_allEntityMap.TryGetValue(guid, out var entity))
            {
                return entity;
            }

            return null;
        }
    }
}