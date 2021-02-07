using UnityEngine;

namespace MGame
{
    public class Game : Singleton<Game>
    {
        public ObjectPool ObjectPool { private set; get; }
        public Scene Scene { private set; get; }
        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        public override void Init()
        {
            base.Init();
            GameObject = new GameObject("GameRoot");
            UnityEngine.GameObject.DontDestroyOnLoad(GameObject);
            Transform = GameObject.transform;

            ObjectPool = new ObjectPool().Init() as ObjectPool;

            Scene = new Scene().Init() as Scene;
        }

        public override void Dispose()
        {
            base.Dispose();

            Scene?.Dispose();
            Scene = null;

            ObjectPool?.Dispose();
            ObjectPool = null;
        }
    }
}