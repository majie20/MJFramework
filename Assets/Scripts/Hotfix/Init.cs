using UnityEngine;

namespace MGame.Hotfix
{
    public static class Init
    {
        public static void Start()
        {
            Game.Instance.Init();

            var test = new Entity().Init(true, "Cylinder", Game.Instance.Scene);
            test.AddComponent<TestComponent>().Init(test);
        }
    }
}