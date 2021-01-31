using System;
using System.Collections.Generic;

namespace MGame
{
    public class ComponentPool : Singleton<ComponentPool>
    {
        private Dictionary<Type, Queue<Component>> componentDic;

        public override void Init()
        {
            base.Init();
            componentDic = new Dictionary<Type, Queue<Component>>();
        }

        public override void Dispose()
        {
            base.Dispose();
            componentDic = null;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            Type type = typeof(T);
            if (componentDic.ContainsKey(type))
            {
                return (T)componentDic[type].Dequeue();
            }

            return new T();
        }

        public Component GetComponent(Type type)
        {
            if (componentDic.ContainsKey(type))
            {
                return componentDic[type].Dequeue();
            }

            var component = (Component)Activator.CreateInstance(type);

            return component;
        }

        public void RecycleComponent(Component component)
        {
            var type = component.GetType();
            Queue<Component> queue;
            if (componentDic.ContainsKey(type))
            {
                queue = componentDic[type];
            }
            else
            {
                queue = new Queue<Component>();
                componentDic.Add(type, queue);
            }

            queue.Enqueue(component);
        }
    }
}