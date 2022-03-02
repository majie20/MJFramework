using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Model
{
    public class Game : Singleton<Game>
    {
        public ObjectPool ObjectPool { private set; get; }

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

            EventSystem = new EventSystem();

            LifecycleSystem = new LifecycleSystem();

            ObjectPool = new ObjectPool();

            Scene = new Scene();

            Hotfix = new Hotfix();

            ComponentDic = new Dictionary<Type, Component>();
        }

        public override void Dispose()
        {
            ComponentDic = null;

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;

            LifecycleSystem?.Dispose();
            LifecycleSystem = null;

            EventSystem?.Dispose();
            EventSystem = null;

            Hotfix?.Dispose();
            Hotfix = null;
        }

        public void AddComponent<T>(T component) where T : Component
        {
            var type = typeof(T);
            if (!ComponentDic.ContainsKey(type))
            {
                ComponentDic.Add(type, component);
            }
        }

        public T GetComponent<T>() where T : Component
        {
            var type = typeof(T);
            if (ComponentDic.ContainsKey(type))
            {
                return (T)ComponentDic[type];
            }

            return null;
        }
    }
}