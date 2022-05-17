using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Model
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
            ObjectHelper.CreateComponent<NPNodePoolComponent>(this, false);
        }

        public override void Dispose()
        {
            IsDispose = true;
            if (childDic.Count > 0)
            {
                foreach (var child in childDic.Values)
                {
                    child.Dispose();
                }
            }

            if (componentDic.Count > 0)
            {
                foreach (var value in componentDic.Values)
                {
                    value.Dispose();
                }
            }

            componentDic = null;
            componentView = null;
            childDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
            IsDispose = false;
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

        public async UniTask<GameObject> HatchGameObjByName(string name, Transform parent, bool isAB)
        {
            return await this.GetComponent<GameObjPoolComponent>().HatchGameObjByName(name, parent, isAB);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            this.GetComponent<GameObjPoolComponent>().RecycleGameObj(sign, obj);
        }

        #endregion 游戏物体

        #region 组件

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