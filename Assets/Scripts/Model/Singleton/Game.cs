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
            GameObject = Object.FindObjectOfType<Init>().gameObject;
            Object.DontDestroyOnLoad(GameObject);
            Transform = GameObject.transform;

            EventSystem = new EventSystem().Init();

            ObjectPool = new ObjectPool().Init() as ObjectPool;

            Scene = new Scene().Init() as Scene;

            Hotfix = new Hotfix().Init();
        }

        public override void Dispose()
        {
            base.Dispose();
            EventSystem?.Dispose();
            EventSystem = null;

            Scene?.Dispose();
            Scene = null;

            Hotfix?.Dispose();
            Hotfix = null;

            ObjectPool?.Dispose();
            ObjectPool = null;
        }
    }
}