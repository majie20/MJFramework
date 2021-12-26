using UnityEngine;

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
        }

        public override void Dispose()
        {
            Hotfix?.Dispose();
            Hotfix = null;

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;

            LifecycleSystem?.Dispose();
            LifecycleSystem = null;

            EventSystem?.Dispose();
            EventSystem = null;
        }
    }
}