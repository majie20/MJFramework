using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

using UnityEngine;

#endif

namespace MGame.Model
{
    public class Entity
    {
        private Dictionary<Type, Component> componentDic;

        public long id { set; get; }
#if UNITY_EDITOR
        public GameObject GameObject { set; get; }
        public Transform Transform { set; get; }
        public Entity parent { set; get; }
#endif

        public Entity()
        {
        }

        public virtual Entity Init()
        {
            componentDic = new Dictionary<Type, Component>();
#if UNITY_EDITOR
            if (!this.GetType().IsDefined(typeof(HideInHierarchyAttribute), true))
            {
                GameObject = new GameObject(GetType().Name);
                Transform = GameObject.transform;
                Transform.SetParent(parent != null ? parent.Transform : Game.Instance.Transform);
                //this.gameObject.AddComponent<ComponentView>().Component = this;
            }
#endif
            return this;
        }

        public virtual void Dispose()
        {
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }
            componentDic = null;
#if UNITY_EDITOR
            if (!this.GetType().IsDefined(typeof(HideInHierarchyAttribute), true))
            {
                Transform = null;
                UnityEngine.GameObject.Destroy(GameObject);
            }
#endif
        }

        #region 添加组件

        public Component AddComponent(Component component)
        {
            Type type = component.GetType();
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type];
            }

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
            componentDic.Add(type, component);

            return component;
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
                componentDic.Remove(type);
                component.Dispose();

                Game.Instance.ObjectPool.RecycleComponent(component);
                return true;
            }

            return false;
        }

        #endregion 删除组件
    }
}