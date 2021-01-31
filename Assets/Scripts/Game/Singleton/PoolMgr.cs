using System;
using System.Collections.Generic;
using UnityEngine;

namespace MGame
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<Type, Queue<Entity>> entityDic;

        public override void Init()
        {
            base.Init();
            entityDic = new Dictionary<Type, Queue<Entity>>();
            GameObjPool.Instance.Init();
            ComponentPool.Instance.Init();
        }

        public override void Dispose()
        {
            base.Dispose();
            entityDic = null;
            GameObjPool.Instance.Dispose();
            ComponentPool.Instance.Dispose();
        }

        #region Entity

        public T GetEntity<T>() where T : Entity, new()
        {
            Type type = typeof(T);
            if (entityDic.ContainsKey(type))
            {
                return (T)entityDic[type].Dequeue();
            }

            return new T();
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

        #endregion

        #region 游戏物体

        public GameObject GetGameObjByName(string name)
        {
            return GameObjPool.Instance.GetGameObjByName(name);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            GameObjPool.Instance.RecycleGameObj(sign, obj);
        }

        #endregion 游戏物体

        #region 组件

        public T GetComponent<T>() where T : Component, new()
        {
            return ComponentPool.Instance.GetComponent<T>();
        }

        public Component GetComponent(Type type)
        {
            return ComponentPool.Instance.GetComponent(type);
        }

        public void RecycleComponent(Component component)
        {
            ComponentPool.Instance.RecycleComponent(component);
        }

        #endregion 组件
    }
}