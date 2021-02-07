using System;
using UnityEngine;

namespace MGame
{
    //[HideInHierarchy]
    public class ObjectPool : SingletonEntity
    {
        public ObjectPool()
        {
        }

        public override Entity Init()
        {
            base.Init();
            AddComponent(new ComponentPoolComponent().Init(this));
            AddComponent(new GameObjPoolComponent().Init(this));
            AddComponent(new EntityPoolComponent().Init(this));
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #region Entity

        public T GetEntity<T>() where T : Entity, new()
        {
            return GetComponent<EntityPoolComponent>().GetEntity<T>();
        }

        public Entity GetEntity(Type type)
        {
            return GetComponent<EntityPoolComponent>().GetEntity(type);
        }

        public void RecycleEntity(Entity entity)
        {
            GetComponent<EntityPoolComponent>().RecycleEntity(entity);
        }

        #endregion Entity

        #region 游戏物体

        public GameObject GetGameObjByName(string name)
        {
            return GetComponent<GameObjPoolComponent>().GetGameObjByName(name);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            GetComponent<GameObjPoolComponent>().RecycleGameObj(sign, obj);
        }

        #endregion 游戏物体

        #region 组件

        public T FetchComponent<T>() where T : Component, new()
        {
            return GetComponent<ComponentPoolComponent>().FetchComponent<T>();
        }

        public Component FetchComponent(Type type)
        {
            return GetComponent<ComponentPoolComponent>().FetchComponent(type);
        }

        public void RecycleComponent(Component component)
        {
            GetComponent<ComponentPoolComponent>().RecycleComponent(component);
        }

        #endregion 组件
    }
}