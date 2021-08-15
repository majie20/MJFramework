using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MGame.Hotfix
{
    public class Entity
    {
        protected Dictionary<Type, Component> componentDic;

        protected Model.ComponentView componentView;

        private Entity parent;

        public Entity Parent
        {
            set
            {
                parent = value;
            }
            get
            {
                return parent;
            }
        }

        private GameObject gameObject;

        public GameObject GameObject
        {
            set
            {
                gameObject = value;
            }
            get
            {
                return gameObject;
            }
        }

        private Transform transform;

        public Transform Transform
        {
            set
            {
                transform = value;
            }
            get
            {
                return transform;
            }
        }

        private string sign;

        public string Sign
        {
            set
            {
                sign = value;
            }
            get
            {
                return sign;
            }
        }

        public Entity()
        {
        }

        public virtual void Init()
        {
            componentDic = new Dictionary<Type, Component>();
        }

        public virtual Entity Init(bool isAB, string sign, Entity parent)
        {
            Init();

            GameObject = Game.Instance.ObjectPool.HatchGameObjByName(sign, isAB);
            if (isAB)
            {
                this.Sign = sign;
            }
            else
            {
                this.Sign = "OrdinaryGameObject";
                GameObject.name = sign;
            }

            Transform = GameObject.transform;
            Transform.SetParent(parent.Transform);

            AddComponentView();

            return this;
        }

        protected void AddComponentView()
        {
            var component = GameObject.GetComponent<Model.ComponentView>();
            componentView = component == null ? GameObject.AddComponent<Model.ComponentView>() : component;
            componentView.isHotfix = true;
        }

        public virtual void Dispose()
        {
            foreach (var component in GetComponents())
            {
                RemoveComponent(component);
            }
            componentDic = null;

            if (GameObject != null)
            {
                Game.Instance.ObjectPool.RecycleGameObj(Sign, GameObject);
                Transform = null;
                GameObject = null;
            }
        }

        #region 添加组件

        public Component AddComponent(Component component)
        {
            Type type = component.GetType();
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type];
            }

            AddToComponentView(component);
            componentDic.Add(type, component);

            return component;
        }

        public Component AddComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type];
            }

            var component = Game.Instance.ObjectPool.HatchComponent(type);
            AddToComponentView(component);
            componentDic.Add(type, component);

            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
        }

        private void AddToComponentView(Component component)
        {
            componentView.dic.Add(component, component.GetType());
        }

        #endregion 添加组件

        #region 获取组件

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (componentDic.TryGetValue(type, out Component component))
            {
                return (T)component;
            }

            return null;
        }

        public Component GetComponent(Type type)
        {
            if (componentDic.TryGetValue(type, out Component component))
            {
                return component;
            }

            return null;
        }

        public IEnumerable<Component> GetComponents()
        {
            return componentDic.Values;
        }

        public Component[] GetComponentsToArray()
        {
            return componentDic.Values.ToArray();
        }

        #endregion 获取组件

        #region 删除组件

        public bool RemoveComponent<T>() where T : Component
        {
            return RemoveComponent(typeof(T));
        }

        public bool RemoveComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                var component = componentDic[type];
                RemoveToComponentView(component);
                componentDic.Remove(type);
                component.Dispose();

                Game.Instance.ObjectPool.RecycleComponent(component);
                return true;
            }

            return false;
        }

        public bool RemoveComponent(Component component)
        {
            Type type = component.GetType();
            if (componentDic.ContainsKey(type))
            {
                RemoveToComponentView(component);
                componentDic.Remove(type);
                component.Dispose();

                Game.Instance.ObjectPool.RecycleComponent(component);
                return true;
            }

            return false;
        }

        private void RemoveToComponentView(Component component)
        {
            componentView.dic.Remove(component);
        }

        #endregion 删除组件
    }
}