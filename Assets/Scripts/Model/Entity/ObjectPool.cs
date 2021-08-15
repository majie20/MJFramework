using System;
using UnityEngine;

namespace MGame.Model
{
    //[HideInHierarchy]
    public class ObjectPool : Entity
    {
        public ObjectPool()
        {
        }

        public Entity Init(string name, Transform parent)
        {
            Init();

            GameObject = new GameObject(name);
            Transform = GameObject.transform;
            Transform.SetParent(parent);
            GameObject.SetActive(false);

            AddComponentView();

            AddComponent(new ComponentPoolComponent().Init(this));
            AddComponent(new GameObjPoolComponent().Init(this));
            AddComponent(new EntityPoolComponent().Init(this));

            return this;
        }

        public override void Dispose()
        {
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }
            componentDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
        }

        #region Entity

        public T HatchEntity<T>() where T : Entity, new()
        {
            return GetComponent<EntityPoolComponent>().HatchEntity<T>();
        }

        public Entity HatchEntity(Type type)
        {
            return GetComponent<EntityPoolComponent>().HatchEntity(type);
        }

        public void RecycleEntity(Entity entity)
        {
            GetComponent<EntityPoolComponent>().RecycleEntity(entity);
        }

        #endregion Entity

        #region 游戏物体

        public GameObject HatchGameObjByName(string name, bool isAB)
        {
            return GetComponent<GameObjPoolComponent>().HatchGameObjByName(name, isAB);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            GetComponent<GameObjPoolComponent>().RecycleGameObj(sign, obj);
        }

        #endregion 游戏物体

        #region 组件

        public T HatchComponent<T>() where T : Component, new()
        {
            return GetComponent<ComponentPoolComponent>().HatchComponent<T>();
        }

        public Component HatchComponent(Type type)
        {
            return GetComponent<ComponentPoolComponent>().HatchComponent(type);
        }

        public void RecycleComponent(Component component)
        {
            GetComponent<ComponentPoolComponent>().RecycleComponent(component);
        }

        #endregion 组件
    }
}