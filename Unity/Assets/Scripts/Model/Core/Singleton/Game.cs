using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Model
{
    public class Game : Singleton<Game>
    {
        public Scene Scene { private set; get; }

        public EventSystem EventSystem { private set; get; }

        public LifecycleSystem LifecycleSystem { private set; get; }

        public Hotfix Hotfix { private set; get; }

        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        private Dictionary<System.Type, Component> ComponentDic;

        public override void Init()
        {
            GameObject = new GameObject("Model");
            Transform = GameObject.transform;
            Object.DontDestroyOnLoad(GameObject);

            ComponentDic = new Dictionary<Type, Component>();

            EventSystem = new EventSystem(false);

            LifecycleSystem = new LifecycleSystem();

            Scene = new Scene();

            Hotfix = new Hotfix();
        }

        public override void Dispose()
        {
            Scene?.Dispose();
            Scene = null;

            LifecycleSystem?.Dispose();
            LifecycleSystem = null;

            EventSystem?.Dispose();
            EventSystem = null;

            Hotfix?.Dispose();
            Hotfix = null;

            ComponentDic = null;
        }

        public void GAddComponent<T>(T component) where T : Component
        {
            var type = typeof(T);
            if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
            {
                type = croos.ILInstance.Type.ReflectionType;
            }
            if (!ComponentDic.ContainsKey(type))
            {
                ComponentDic.Add(type, component);
            }
        }

        public T GGetComponent<T>() where T : Component
        {
            var type = typeof(T);
            if (ComponentDic.ContainsKey(type))
            {
                return (T)ComponentDic[type];
            }

            return null;
        }

        public void GRemoveComponent<T>(T component) where T : Component
        {
            var type = typeof(T);
            if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
            {
                type = croos.ILInstance.Type.ReflectionType;
            }
            if (ComponentDic.ContainsKey(type))
            {
                ComponentDic.Remove(type);
            }
        }
    }
}