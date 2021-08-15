using UnityEngine;

namespace MGame.Hotfix
{
    public class Game : Singleton<Game>
    {
        public ObjectPool ObjectPool { private set; get; }

        public Scene Scene { private set; get; }

        public EventSystem EventSystem { private set; get; }

        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        public override void Init()
        {
            base.Init();

            GameObject = new GameObject("Hotfix");
            Transform = GameObject.transform;
            Object.DontDestroyOnLoad(GameObject);

            EventSystem = new EventSystem().Init();

            ObjectPool = new ObjectPool().Init("ObjectPool", Transform) as ObjectPool;

            Scene = new Scene().Init("Scene", Transform) as Scene;
        }

        public override void Dispose()
        {
            base.Dispose();
            EventSystem?.Dispose();
            EventSystem = null;

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;
        }
    }
}