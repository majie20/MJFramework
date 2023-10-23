using CatJson;
using Cysharp.Threading.Tasks;
using GameFramework;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

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
            Application.targetFrameRate = 120;
            Application.runInBackground = true;

            NLog.Log.Debug($"------开始------");
            InitLogger();

            using (zstring.Block())
            {
            }

            if (Application.isEditor)
            {
                JsonParser.Default.IsFormat = true;
            }
            else
            {
                JsonParser.Default.IsFormat = false;
            }

            JsonParser.Default.IsPolymorphic = true;

            DontDestroyOnLoad(gameObject);

            //UnityEngine.InputSystem.EnhancedTouch.TouchSimulation.Enable();
            //#if WX&&!UNITY_EDITOR
            //            UnityEngine.InputSystem.EnhancedTouch.TouchSimulation.Enable();
            //#endif

#if !UNITY_WEBGL
            BetterStreamingAssets.Initialize();
#endif

            ConstData.RAW_FILE_SAVE_PATH = FileHelper.JoinPath("RawFile", FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
            ConstData.LOCAL_GAME_DATA_PATH = FileHelper.JoinPath(ConstData.LOCAL_GAME_DATA, FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);
        }

        private void InitLogger()
        {
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
        }

        private void Start()
        {
#if WX&&!UNITY_EDITOR
            WeChatWASM.WXBase.InitSDK(i =>
            {
                WXTouchInputOverride inputOverride = transform.Find("EventSystem").gameObject.AddComponent<WXTouchInputOverride>();
                inputOverride.enabled = true;

                var type = Assembly.Load("Unity.Model").GetType("Model.Initialization");
                type.InvokeMember("Start", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
            });
#elif TT
            StarkSDKSpace.StarkSDK.Init();
            var type = Assembly.Load("Unity.Model").GetType("Model.Initialization");
            type.InvokeMember("Start", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
#else
            var type = Assembly.Load("Unity.Model").GetType("Model.Initialization");
            type.InvokeMember("Start", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
#endif
        }

        private void Update()
        {
            if (!Game.Instance.IsRunning)
            {
                return;
            }

            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
#if ILRuntime
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameUpdate(Time.deltaTime);
#endif

            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Game.Instance.EventSystem.Invoke<E_GameQuit>();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
            }
        }

        private void LateUpdate()
        {
            if (!Game.Instance.IsRunning)
            {
                return;
            }

            Game.Instance.LifecycleSystem.LateUpdate();
#if ILRuntime
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameLateUpdate();
#endif
        }

        private void FixedUpdate()
        {
            if (!Game.Instance.IsRunning)
            {
                return;
            }

            Game.Instance.LifecycleSystem.FixedUpdate(Time.fixedDeltaTime);
#if ILRuntime
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameFixedUpdate(Time.fixedDeltaTime);
#endif
        }

        private void OnApplicationQuit()
        {
            Game.Instance.Dispose();
        }
    }
}