using UnityEngine;

namespace MGame.Model
{
    public class Game : Singleton<Game>
    {
        public ObjectPool ObjectPool { private set; get; }

        public Scene Scene { private set; get; }

        public EventSystem EventSystem { private set; get; }

        public Hotfix Hotfix { private set; get; }

        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        public override void Init()
        {
            base.Init();
            GameObject = new GameObject("GameRoot");
            Object.DontDestroyOnLoad(GameObject);
            Transform = GameObject.transform;

            ObjectPool = new ObjectPool().Init() as ObjectPool;

            Scene = new Scene().Init() as Scene;

            EventSystem = new EventSystem().Init();

            Hotfix = new Hotfix().Init();
        }

        public override void Dispose()
        {
            base.Dispose();

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;

            EventSystem?.Dispose();
            EventSystem = null;

            Hotfix?.Dispose();
            Hotfix = null;
        }
    }
}