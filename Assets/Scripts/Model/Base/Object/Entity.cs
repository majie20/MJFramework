using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MGame.Model
{
    public class Entity
    {
        protected Dictionary<Type, Component> componentDic;

        protected ComponentView componentView;

        public long id { set; get; }
        public Entity parent { set; get; }
        public GameObject gameObject { set; get; }
        public Transform transform { set; get; }
        public string sign { set; get; }

        public Entity()
        {
        }

        public virtual Entity Init(bool isAB)
        {
            componentDic = new Dictionary<Type, Component>();

            //if (!this.GetType().IsDefined(typeof(HideInHierarchyAttribute), true))
            //{
            //}

            gameObject = Game.Instance.ObjectPool.GetGameObjByName(sign, isAB);
            transform = gameObject.transform;

            if (isAB)
            {
                Transform t = null;
                if (parent != null)
                {
                    var e = parent;
                    while (true)
                    {
                        if (e.transform != null)
                        {
                            t = e.transform;
                            break;
                        }

                        if (e.parent == null)
                        {
                            break;
                        }

                        e = e.parent;
                    }
                }

                transform.SetParent(t ? t : Game.Instance.Transform);
            }
            else
            {
                transform.SetParent(Game.Instance.Transform);
            }

            var component = gameObject.GetComponent<ComponentView>();
            if (component == null)
            {
                componentView = gameObject.AddComponent<ComponentView>();
            }
            else
            {
                componentView = component;
            }

            return this;
        }

        public virtual Entity Init()
        {
            return Init(true);
        }

        public virtual void Dispose()
        {
            foreach (var component in GetComponents())
            {
                RemoveComponent(component);
            }
            componentDic = null;

            if (gameObject != null)
            {
                Game.Instance.ObjectPool.RecycleGameObj(sign, gameObject);
                transform = null;
                gameObject = null;
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

            var component = Game.Instance.ObjectPool.FetchComponent(type);
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
            componentView.components.Add(component);
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
            componentView.components.Remove(component);
        }

        #endregion 删除组件
    }
}