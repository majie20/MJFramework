using Cysharp.Threading.Tasks;
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

            ObjectHelper.CreateComponent<GameDataComponent>(Model.Game.Instance.Scene, false);
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