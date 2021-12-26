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

            ObjectHelper.CreateComponent<UIManagerComponent>(ObjectHelper.CreatEntity(Game.Instance.Scene, null, UIManagerComponent.UIROOT_PATH, true), false);
            ObjectHelper.CreateComponent<TestComponent, string>(ObjectHelper.CreatEntity(Game.Instance.Scene, null, "Assets/Res/Prefabs/Sphere", true), "majie");
        }

        private static void OnUpdate(float tick)
        {
            Game.Instance.LifecycleSystem.Update(tick);
        }

        private static void OnLateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
        }

        private static void OnApplicationQuit()
        {
            Game.Instance.Dispose();
        }
    }
}