using System;
using System.Reflection;

namespace Hotfix
{
    public static class Init
    {
        public static void Start()
        {
            Game.Instance.Init();
#if ILRuntime
            Model.Game.Instance.Hotfix.GameUpdate = OnUpdate;
            Model.Game.Instance.Hotfix.GameLateUpdate = OnLateUpdate;
            Model.Game.Instance.Hotfix.GameFixedUpdate = OnFixedUpdate;
#elif HybridCLR
            Model.Game.Instance.LifecycleSystem.InitAssembly(typeof(Init).Assembly);
            Model.Game.Instance.LifecycleSystem.InitAttr();
#endif
            Model.Game.Instance.Hotfix.GameApplicationQuit = OnApplicationQuit;

            Model.AsyncTimerHelper.TimeHandle(
                () =>
                {
                    ObjectHelper.CreateComponent<TestComponent, string>(Model.ObjectHelper.CreateEntity<Model.Entity>(Model.Game.Instance.Scene), "dadjajdlkjadlkjaljdalkd", false);
                }, 100, 1, null, false, () => NLog.Log.Error("aaaaaaaa"));

            //ObjectHelper.CreateComponent<TestComponent>(Model.ObjectHelper.CreateEntity<Model.Entity>(Model.Game.Instance.Scene).GetAwaiter().GetResult(), false);
            //ObjectHelper.CreateComponent<GameDataComponent>(Model.Game.Instance.Scene, false);

            //UniTask.Void(async () =>
            //{
            //    await UniTask.Delay(5500);
            //    ObjectHelper.RemoveComponent<GameDataComponent>(Model.Game.Instance.Scene);
            //});

            //Debug.Log(Model.Game.Instance.Scene.GetComponent<GameDataComponent>().JsonTables.TbItem.Get(10000)); // MDEBUG:

            //var configPath = Model.FileHelper.JoinPath($"{Model.HotConfig.AB_SAVE_RELATIVELY_PATH}{Model.HotConfig.AB_CONFIG_NAME}.json", Model.FileHelper.FilePos.StreamingAssetsPath, Model.FileHelper.LoadMode.UnityWebRequest);
            //byte[] bytes = Model.FileHelper.LoadFileByUnityWebRequest(configPath);
            //string configStr = Encoding.UTF8.GetString(bytes);
            //Debug.Log(configStr); // MDEBUG:
            //var AbConfigs = JsonMapper.ToObject<List<ABConfig>>(configStr);
            //Debug.Log(AbConfigs[0]); // MDEBUG:
            //NLog.Log.Error(Model.ProtobufHelper.SerializeToString_PB(AbConfigs[0])); // MDEBUG:

            //ObjectHelper.OpenUIView<NetTestComponent>();
            //ObjectHelper.OpenUIView<StartViewComponent>();
        }

#if ILRuntime
        private static void OnUpdate(float tick)
        {
            Game.Instance.LifecycleSystem.Update(tick);
        }

        private static void OnLateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
        }

        private static void OnFixedUpdate(float tick)
        {
            Game.Instance.LifecycleSystem.FixedUpdate(tick);
        }
#endif

        private static void OnApplicationQuit()
        {
            Game.Instance.Dispose();
        }
    }
}