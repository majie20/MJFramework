using System;
using System.Collections.Generic;

namespace MGame.Hotfix
{
    [LifeCycle]
    public class ComponentPoolComponent : Component, IAwake
    {
        private Dictionary<Type, Queue<Component>> componentDic;

        public void Awake()
        {
            componentDic = new Dictionary<Type, Queue<Component>>();
        }

        public override void Dispose()
        {
            base.Dispose();
            componentDic = null;
        }

        public T HatchComponent<T>() where T : Component
        {
            return (T)HatchComponent(typeof(T));
        }

        public Component HatchComponent(Type type)
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