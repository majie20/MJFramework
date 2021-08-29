using UnityEngine;

namespace MGame.Model
{
    public class Game : Singleton<Game>
    {
        public ObjectPool ObjectPool { private set; get; }

        public Scene Scene { private set; get; }

        public EventSystem EventSystem { private set; get; }

        public LifeCycleSystem LifeCycleSystem { private set; get; }

        public Hotfix Hotfix { private set; get; }

        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        public override void Init()
        {
            base.Init();

            GameObject = new GameObject("Model");
            Transform = GameObject.transform;
            Object.DontDestroyOnLoad(GameObject);

            EventSystem = new EventSystem().Init();

            LifeCycleSystem = new LifeCycleSystem().Init();

            ObjectPool = new ObjectPool().Init("ObjectPool", Transform) as ObjectPool;

            Scene = new Scene().Init("Scene", Transform) as Scene;

            Hotfix = new Hotfix().Init();
        }

        public override void Dispose()
        {
            base.Dispose();

            Hotfix?.Dispose();
            Hotfix = null;

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;

            LifeCycleSystem?.Dispose();
            LifeCycleSystem = null;

            EventSystem?.Dispose();
            EventSystem = null;
        }
    }
}