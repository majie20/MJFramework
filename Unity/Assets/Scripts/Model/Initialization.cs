using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    public class Initialization
    {
        public static void Start()
        {
            EventType.Init();
            Game.Instance.Init();
            Game.Instance.LifecycleSystem.InitAssembly(typeof(Init).Assembly);
            Game.Instance.LifecycleSystem.InitAssembly(typeof(Initialization).Assembly);
            Game.Instance.LifecycleSystem.InitAttr();

            UniTask.Create(Run);
        }

        private static async UniTask Run()
        {
            var scene = Game.Instance.Scene;
#if WX&&!UNITY_EDITOR
            ObjectHelper.CreateComponent<WXFileSystemManagerComponent>(scene, false);
#endif
            ObjectHelper.CreateComponent<ComponentPoolComponent>(scene, false);
            ObjectHelper.CreateComponent<GameObjPoolComponent>(scene, false);
            ObjectHelper.CreateComponent<EntityPoolComponent>(scene, false);
            ObjectHelper.CreateComponent<PlayerManagerComponent>(scene, false);
            //ObjectHelper.CreateComponent<HttpComponent>(scene, false);
            AssetsComponent assetsComponent = ObjectHelper.CreateComponent<AssetsComponent>(scene, false);
            ObjectHelper.CreateComponent<HotComponent>(scene, false);
            ObjectHelper.CreateComponent<GamePlayDataComponent>(scene, false);
            ObjectHelper.CreateComponent<NPContextComponent>(scene, false);
            ObjectHelper.CreateComponent<GameManagerComponent>(scene, false);
            ObjectHelper.CreateComponent<NPNodePoolComponent>(scene, false);

            await assetsComponent.Init();

            ObjectHelper.CreateComponent<SpriteComponent>(scene, false);
            ObjectHelper.CreateComponent<AudioComponent>(scene, false);

            ObjectHelper.CreateComponent<UIRootComponent>(ObjectHelper.CreateEntity<Entity>(scene, null, ConstData.UI_ROOT, true), false);

            LoadSceneData data = new LoadSceneData() { ScenePath = ConstData.MAIN_SCENE, AssetPaths = LoadInfo.InfosMap[1], Call = OnGameLoadComplete };
            UIHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Hot, data);
        }

        private static async UniTask OnGameLoadComplete()
        {
            NLog.Log.Debug($"------完成------");
            AssetsComponent assetsComponent = Game.Instance.Scene.GetComponent<AssetsComponent>();
            assetsComponent.Unload(ConstData.GAME_LOAD_COMPLETE_ARS);
            assetsComponent.Unload(ConstData.INIT_ARS);
            await assetsComponent.ClearPackageUnusedCacheFilesAsync();

            GameConfigDataComponent gameConfigDataComponent = ObjectHelper.CreateComponent<GameConfigDataComponent>(Game.Instance.Scene, false);

            UnitDataComponent unitDataComponent = ObjectHelper.CreateComponent<UnitDataComponent>(Game.Instance.Scene, false);
            var code = Game.Instance.Scene.GetComponent<GamePlayDataComponent>().GetGameData(GameDataKey.CUR_PLAYER_TYPE_CODE, gameConfigDataComponent.GameConst.DefPlayer);
            var cfg = gameConfigDataComponent.JsonTables.TbRole.Get(code);
            await unitDataComponent.LoadPlayerReferenceRes(cfg.TypeCode);

#if ILRuntime||HybridCLR
            await Game.Instance.Hotfix.LoadHotfixAssembly();

            Game.Instance.Hotfix.GotoHotfix();
#endif

            UIHelper.OpenUIView<StartViewComponent>();

            Rigidbody2DComponent.PM2D_FullFriction = await assetsComponent.LoadAsync<PhysicsMaterial2D>(ConstData.Full_Friction);
            Rigidbody2DComponent.PM2D_NoFriction = await assetsComponent.LoadAsync<PhysicsMaterial2D>(ConstData.No_Friction);
            NLog.Log.Debug("-----这里有一段播放背景音乐播放测试");

            //UniTask.Void(async () => await Game.Instance.Scene.GetComponent<AudioComponent>().PlayMusic(100002));

            //WeChatWASM.WX.GetPrivacySetting(new WeChatWASM.GetPrivacySettingOption()
            //{
            //    success = result =>
            //    {
            //        NLog.Log.Debug($"{result.privacyContractName}：\n{result.errMsg}");

            //        if (result.needAuthorization)
            //        {
            //            WeChatWASM.WX.OpenPrivacyContract(new WeChatWASM.OpenPrivacyContractOption()
            //            {
            //                success = callbackResult => { NLog.Log.Debug(callbackResult.errMsg); }, fail = callbackResult => { NLog.Log.Debug(callbackResult.errMsg); }
            //            });
            //        }
            //    }
            //});
        }
    }
}