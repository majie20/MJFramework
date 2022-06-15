using System;
using System.Collections.Generic;

namespace Model
{
    [LifeCycle]
    public class ComponentPoolComponent : Component, IAwake, ILateUpdateSystem
    {
        private Dictionary<Type, Queue<Component>> componentDic;
        private List<Component> components;

        public void Awake()
        {
            componentDic = new Dictionary<Type, Queue<Component>>();
            components = new List<Component>();
        }

        public override void Dispose()
        {
            componentDic = null;
            components = null;
            base.Dispose();
        }

        public void OnLateUpdate()
        {
            if (components.Count == 0)
            {
                return;
            }
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                var type = component.GetType();
                if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
                {
                    type = croos.ILInstance.Type.ReflectionType;
                }
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
            components.Clear();
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

            Component component;
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                component = Game.Instance.Hotfix.AppDomain.Instantiate<Component>(type.FullName);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }

            return component;
        }

        public void RecycleComponent(Component component)
        {
            components.Add(component);
        }
    }
}