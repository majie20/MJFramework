using System;
using UnityEngine;

namespace MGame.Model
{
    public class ObjectPool : Entity
    {
        public ObjectPool()
        {
            GameObject = new GameObject("ObjectPool");
            Transform = GameObject.transform;
            Transform.SetParent(Game.Instance.Transform);
            GameObject.SetActive(false);

            this.AddComponentView();

            ObjectHelper.CreateComponent<ComponentPoolComponent>(this, false);
            ObjectHelper.CreateComponent<GameObjPoolComponent>(this, false);
            ObjectHelper.CreateComponent<EntityPoolComponent>(this, false);
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

        public Entity HatchEntity()
        {
            return this.GetComponent<EntityPoolComponent>().HatchEntity();
        }

        public void RecycleEntity(Entity entity)
        {
            this.GetComponent<EntityPoolComponent>().RecycleEntity(entity);
        }

        #endregion Entity

        #region 游戏物体

        public GameObject HatchGameObjByName(string name, bool isAB)
        {
            return this.GetComponent<GameObjPoolComponent>().HatchGameObjByName(name, isAB);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            this.GetComponent<GameObjPoolComponent>().RecycleGameObj(sign, obj);
        }

        #endregion 游戏物体

        #region 组件

        public T HatchComponent<T>() where T : Component, new()
        {
            return this.GetComponent<ComponentPoolComponent>().HatchComponent<T>();
        }

        public Component HatchComponent(Type type)
        {
            return this.GetComponent<ComponentPoolComponent>().HatchComponent(type);
        }

        public void RecycleComponent(Component component)
        {
            this.GetComponent<ComponentPoolComponent>().RecycleComponent(component);
        }

        #endregion 组件
    }
}