using System;
using System.Collections.Generic;
using UnityEngine;

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

        public bool IsRunning { private set; get; }

        public override void Init()
        {
            GameObject = new GameObject("Model");
            Transform = GameObject.transform;
            UnityEngine.Object.DontDestroyOnLoad(GameObject);

            ComponentDic = new Dictionary<Type, Component>();

            LifecycleSystem = new LifecycleSystem();

            Scene = new Scene();

            EventSystem = Scene.EventSystem;

            Hotfix = new Hotfix();

            IsRunning = true;
        }

        public override void Dispose()
        {
            IsRunning = false;
            EventSystem = null;

            Scene?.Dispose();
            Scene = null;

            LifecycleSystem?.Dispose();
            LifecycleSystem = null;

#if ILRuntime
            if (Hotfix.IsRuning) Hotfix.GameApplicationQuit();
#endif
            Hotfix?.Dispose();
            Hotfix = null;

            ComponentDic = null;
        }

        public void GAddComponent<T>(T component) where T : Component
        {
            var type = typeof(T);
#if ILRuntime
            if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
            {
                type = croos.ILInstance.Type.ReflectionType;
            }
#endif
            if (!ComponentDic.ContainsKey(type))
            {
                ComponentDic.Add(type, component);
            }
        }

        public T GGetComponent<T>() where T : Component
        {
            var type = typeof(T);

            ComponentDic.TryGetValue(type, out var component);

            return (T) component;
        }

        public void GRemoveComponent<T>(T component) where T : Component
        {
            var type = typeof(T);
#if ILRuntime
            if (component is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType croos)
            {
                type = croos.ILInstance.Type.ReflectionType;
            }
#endif
            ComponentDic.Remove(type);
        }
    }
}