using UnityEngine;

namespace Hotfix
{
    public static class Init
    {
        public static void Start()
        {
            Game.Instance.Init();

            Model.Game.Instance.Hotfix.GameUpdate = OnUpdate;
            Model.Game.Instance.Hotfix.GameLateUpdate = OnLateUpdate;
            Model.Game.Instance.Hotfix.GameApplicationQuit = OnApplicationQuit;

            //ObjectHelper.CreateComponent<TestComponent, string>(ObjectHelper.CreatEntity(Game.Instance.Scene, "Cylinder", true), "majie");
            //ObjectHelper.CreateComponent<NetTestComponent>(ObjectHelper.CreatEntity(Game.Instance.Scene, "NetTest", true));


            //var test1 = Game.Instance.ObjectPool.HatchEntity().Init(true, "NetTest", Game.Instance.Scene);
            //ObjectHelper.CreateComponent<NetTestComponent>(test1);
        }

        private static void OnUpdate(float tick)
        {
            //Debug.Log($"OnUpdate:{tick}"); // MDEBUG:
            Game.Instance.LifecycleSystem.Update(tick);
        }

        private static void OnLateUpdate()
        {
            //Debug.Log("OnLateUpdate"); // MDEBUG:
            Game.Instance.LifecycleSystem.LateUpdate();
        }

        private static void OnApplicationQuit()
        {
            Game.Instance.Dispose();
            //Debug.Log("OnApplicationQuit"); // MDEBUG:
        }
    }
}