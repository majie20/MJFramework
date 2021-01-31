using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MGame
{
    public class Entity
    {
        private Dictionary<Type, Component> componentDic = new Dictionary<Type, Component>();

        public long id { set; get; }
        public GameObject gameObject { set; get; }
        public Transform transform { set; get; }
        public Entity parent { set; get; }

        public Entity()
        {
            componentDic = new Dictionary<Type, Component>();
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

            var component = PoolMgr.Instance.GetComponent(type);
            componentDic.Add(type, component);

            return component;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            Type type = typeof(T);
            if (componentDic.ContainsKey(type))
            {
                return (T)componentDic[type];
            }

            var component = PoolMgr.Instance.GetComponent<T>();
            componentDic.Add(type, component);

            return component;
        }

        #endregion 添加组件

        #region 获取组件

        public T GetComponent<T>() where T : Component, new()
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

        #endregion
    }
}