using System;
using System.Collections.Generic;

namespace MGame.Model
{
    public class ComponentPoolComponent : Component
    {
        private Dictionary<Type, Queue<Component>> componentDic;
        private List<int> list = new List<int> { 1, 2, 3, 4, 5 };

        public ComponentPoolComponent()
        {
        }

        public override Component Init()
        {
            base.Init();
            componentDic = new Dictionary<Type, Queue<Component>>();
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            componentDic = null;
        }

        public T FetchComponent<T>() where T : Component
        {
            return (T)FetchComponent(typeof(T));
        }

        public Component FetchComponent(Type type)
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