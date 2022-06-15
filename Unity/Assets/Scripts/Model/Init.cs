using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Model
{
    public class Init : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Inject()
        {
            SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            Screen.SetResolution(1280, 720, FullScreenMode.FullScreenWindow);

            var logger = new Logger(Debug.unityLogger.logHandler);
            NLog.Unity.NLogConfig nLogConfig = GameObject.Find("NLog").GetComponent<NLog.Unity.NLogConfig>();
            if (nLogConfig)
            {
                if (nLogConfig.logLevel == NLog.LogLevel.Debug)
                {
                    logger.filterLogType = LogType.Log;
                }
                else if (nLogConfig.logLevel == NLog.LogLevel.Warn)
                {
                    logger.filterLogType = LogType.Warning;
                }
                else if (nLogConfig.logLevel == NLog.LogLevel.Error)
                {
                    logger.filterLogType = LogType.Error;
                }
                else if (nLogConfig.logLevel == NLog.LogLevel.On)
                {
                    logger.logEnabled = true;
                }
                else if (nLogConfig.logLevel == NLog.LogLevel.Off)
                {
                    logger.logEnabled = false;
                }
            }
            Debug.unityLogger.logHandler = logger;

            EventType.Init();
            Game.Instance.Init();
#if MBuild
            CatJson.GenJsonCodesHelper.Init();
#endif
        }

        private void Start()
        {
            UniTask.Create(Run);
        }

        private async UniTask Run()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            await component.Init();

            ObjectHelper.CreateComponent<SpriteComponent>(Game.Instance.Scene, false);

            ObjectHelper.CreateComponent<UIRootComponent>(await ObjectHelper.CreateEntity(Game.Instance.Scene, null, UIRootComponent.UIROOT_PATH, true), false);

            LoadSceneData data = new LoadSceneData()
            {
                ScenePath = $"Assets/Res/Scenes/Main.unity",
                SettingsPath = FileValue.GAME_LOAD_COMPLETE_ARS,
                Call = OnGameLoadComplete
            };
            await ObjectHelper.OpenUIView<LoadingViewComponent, LoadUseType, LoadSceneData>(LoadUseType.Hot, data);
        }

        private void Update()
        {
            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameUpdate(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //Game.Instance.Hotfix.GotoHotfix();

                //ObjectHelper.OpenUIView<StartViewComponent>();
            }
        }

        private void LateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameLateUpdate();
        }

        private void OnApplicationQuit()
        {
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameApplicationQuit();
            Game.Instance.Dispose();
        }

        private async UniTask OnGameLoadComplete()
        {
            NLog.Log.Debug("------完成------");

            await Game.Instance.Scene.GetComponent<AssetsComponent>().LoadNecessary();
            Game.Instance.Scene.GetComponent<SpriteComponent>().Init();
            ObjectHelper.CreateComponent<GameConfigDataComponent>(Game.Instance.Scene, false);
            ObjectHelper.CreateComponent<MusicManagerComponent>(Game.Instance.Scene, false);

            await Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();

            await ObjectHelper.OpenUIView<StartViewComponent>();

            NLog.Log.Debug("-----这里有一段播放背景音乐播放测试");
            //Game.Instance.EventSystem.InvokeAsync<E_PlayMusic, int>(10001);
            ObjectHelper.SetMainCamera();
        }
    }
}