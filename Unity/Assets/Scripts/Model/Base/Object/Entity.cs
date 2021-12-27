using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class Entity : IDisposable
    {
        protected Dictionary<Type, Component> componentDic;
        protected Dictionary<string, Entity> childDic;
        protected ComponentView componentView;

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
            componentDic = new Dictionary<Type, Component>();
            childDic = new Dictionary<string, Entity>();
        }

        public virtual void Dispose()
        {
            foreach (var child in childDic.Values)
            {
                child.Dispose();
            }
            foreach (var component in this.GetComponents())
            {
                component.Dispose();
            }
            componentDic = null;
            componentView = null;
            childDic = null;

            if (GameObject != null)
            {
                if (Sign != GameObjPoolComponent.None_GameObject)
                {
                    Game.Instance.ObjectPool.RecycleGameObj(Sign, GameObject);
                }
                Transform = null;
                GameObject = null;
            }
        }

        public void AddComponentView()
        {
            var component = GameObject.GetComponent<Model.ComponentView>();
            componentView = component == null ? GameObject.AddComponent<Model.ComponentView>() : component;
            componentView.isHotfix = false;
        }

        private void AddToComponentView(Component component)
        {
            componentView.dic.Add(component, component.GetType());
        }

        public void SetParent(Entity entity)
        {
            Parent = entity;
            entity.AddChild(this);
        }

        public void AddChild(Entity child)
        {
            childDic.Add(child.Sign == GameObjPoolComponent.None_GameObject ? child.GameObject.name : child.Sign, child);
        }

        public Entity GetChild(string sign)
        {
            if (childDic.ContainsKey(sign))
            {
                return childDic[sign];
            }

            return null;
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
            return ObjectHelper.CreateComponent(type, this, false);
        }

        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
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