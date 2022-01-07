using System;
using UnityEngine;

namespace Hotfix
{
    public class Game : Singleton<Game>
    {
        //public Scene Scene { private set; get; }

        public LifecycleSystem LifecycleSystem { private set; get; }

        public GameObject GameObject { private set; get; }
        public Transform Transform { private set; get; }

        public override void Init()
        {
            GameObject = new GameObject("Hotfix");
            Transform = GameObject.transform;
            GameObject.DontDestroyOnLoad(GameObject);

            LifecycleSystem = new LifecycleSystem();

            //Scene = new Scene();
        }

        public override void Dispose()
        {
            //Scene?.Dispose();
            //Scene = null;

            LifecycleSystem?.Dispose();
            LifecycleSystem = null;
        }
    }
}