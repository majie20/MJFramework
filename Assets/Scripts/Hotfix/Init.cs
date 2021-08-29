using UnityEngine;

namespace MGame.Hotfix
{
    public static class Init
    {
        public static void Start()
        {
            Game.Instance.Init();

            Model.Game.Instance.Hotfix.GameUpdate = OnUpdate;
            Model.Game.Instance.Hotfix.GameLateUpdate = OnLateUpdate;
            Model.Game.Instance.Hotfix.GameApplicationQuit = OnApplicationQuit;

            var test = new Entity().Init(true, "Cylinder", Game.Instance.Scene);
            var component = test.AddComponent<TestComponent>().Init(test);
            Game.Instance.LifeCycleSystem.Awake(component);
            Game.Instance.LifeCycleSystem.Awake(component, "majie");
        }

        private static void OnUpdate(float tick)
        {
            //Debug.Log($"OnUpdate:{tick}"); // MDEBUG:
        }

        private static void OnLateUpdate()
        {
            //Debug.Log("OnLateUpdate"); // MDEBUG:
        }

        private static void OnApplicationQuit()
        {
            //Debug.Log("OnApplicationQuit"); // MDEBUG:
        }
    }
}