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

            //UIRootComponent uiRootComponent = ObjectHelper.CreateComponent<UIRootComponent>(ObjectHelper.CreatEntity(Game.Instance.Scene, null, UIRootComponent.UIROOT_PATH, true), false);
            //ObjectHelper.CreateComponent<UIManagerComponent>(ObjectHelper.CreatEntity2(uiRootComponent.Entity, uiRootComponent.Entity.Transform.Find(UIManagerComponent.GAME_OBJECT_NAME).gameObject, Model.GameObjPoolComponent.None_GameObject), false);

            //ObjectHelper.CreateComponent<TestComponent, string>(ObjectHelper.CreatEntity(Game.Instance.Scene, null, "Assets/Res/Prefabs/Sphere", true), "majie");
            //Model.ObjectHelper.CreateComponent<Test1Component, string>(Model.ObjectHelper.CreatEntity(Model.Game.Instance.Scene), "majie");
            ObjectHelper.CreateComponent<Test1Component>(ObjectHelper.CreatEntity(Model.Game.Instance.Scene));
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