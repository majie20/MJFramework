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

            ObjectHelper.OpenUIView<NetTestComponent>();

            var entity = Model.ObjectHelper.CreateEntity(Model.Game.Instance.Scene, null, "Assets/Res/Prefabs/Sphere", true);
            ObjectHelper.CreateComponent<TestComponent, string>(entity, "majie");
            var c = entity.GetComponent<TestComponent>();
            Debug.Log(c); // MDEBUG:
            //Model.ObjectHelper.RemoveEntity(entity);
            //Debug.Log(typeof(NetTestComponent).GetCustomAttributes(typeof(Model.UIBaseDataAttribute), false).Length); // MDEBUG:
            //var a = typeof(NetTestComponent).GetCustomAttributes(false);
            //Debug.Log(a.Length); // MDEBUG:
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