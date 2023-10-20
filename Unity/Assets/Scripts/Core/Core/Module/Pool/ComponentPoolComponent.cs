using System;
using System.Collections.Generic;
using M.Algorithm;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class ComponentPoolComponent : Component, IAwake, ILateUpdateSystem
    {
        private Dictionary<Type, Queue<Component>> componentDic;
        private List<Component>                    components;

        private StaticLinkedListDictionary<Type, TwoStaticLinkedList<Component>> hatchDic;

        public void Awake()
        {
            componentDic = new Dictionary<Type, Queue<Component>>();
            components = new List<Component>();
            hatchDic = new StaticLinkedListDictionary<Type, TwoStaticLinkedList<Component>>(4, 4);
        }

        public override void Dispose()
        {
            componentDic = null;
            components = null;
            hatchDic = null;
            base.Dispose();
        }

        [FunctionSort(FunctionLayer.Low)]
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
#if ILRuntime
                if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
                {
                    type = croos.ILInstance.Type.ReflectionType;
                }
#endif
                Queue<Component> queue;

                if (!componentDic.TryGetValue(type, out queue))
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
            if (!hatchDic.TryGetValue(type, out var list))
            {
                list = new TwoStaticLinkedList<Component>(4, 4);
                hatchDic.Add(type, list);
            }

            if (componentDic.TryGetValue(type, out var queue) && queue.Count > 0)
            {
                var c = queue.Dequeue();
                list.Add(c);

                return c;
            }

            Component component;
#if ILRuntime
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                component = Game.Instance.Hotfix.AppDomain.Instantiate<Component>(type.FullName);
            }
            else
            {
                component = (Component)Activator.CreateInstance(type);
            }
#else
            component = (Component)Activator.CreateInstance(type);
#endif
            list.Add(component);

            return component;
        }

        public void RecycleComponent(Component component)
        {
            components.Add(component);
        }

        public void RecycleAllComponent<T>() where T : Component
        {
            RecycleAllComponent(typeof(T));
        }

        public void RecycleAllComponent(Type type)
        {
            if (hatchDic.TryGetValue(type, out var list))
            {
                var data = list[1];

                while (data.right != 0)
                {
                    data = list[data.right];
                    RecycleComponent(data.element);
                }
            }
        }
    }
}