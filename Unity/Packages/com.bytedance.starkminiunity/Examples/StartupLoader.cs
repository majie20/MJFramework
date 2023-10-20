// created by StarkMini

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using StarkMini;
using UnityEngine.SceneManagement;

namespace StarkMini.Examples
{
    /// <summary>可以自定义load选项</summary>
    public class StartupLoadOptions {
        public StartupLoadOptions(bool loadAll) {
            scene = loadAll;
            preload_assets = loadAll;
        }
        public bool scene;
        public bool preload_assets;
    }

    /// <summary>一项 startup task 的数据</summary>
    public class StartupTaskData {
        public StartupTaskData(string name, float size, StartupLoader.EResLoadMode load_mode) {
            this.name = name;
            this.size = size;
            this.load_mode = load_mode;
        }

        public float getStepPercent() {
            var f = 0f;
            if (handle.IsValid()) {
                f = handle.PercentComplete;
            }
            if (f > percent) {
                percent = f;
            }
            return percent;
        }

        public string name;
        public float percent;
        public float size;
        public bool ignore_load_time;
        public StartupLoader.EResLoadMode load_mode;
        public AsyncOperationHandle handle;
    }

    /// <summary>负责启动时的加载任务。 目前具体加载：主场景, preload assets.</summary>
    public class StartupLoader
    {
        enum ELoadState
        {
            None = 0,
            /// <summary>load started, is loading assets</summary>
            LoadingAssets,
            /// <summary>load started, is changing scene</summary>
            ChangingScene,
            /// <summary>load done</summary>
            Loaded,
        }
        public enum EResLoadMode
        {
            ResourceSync = 0,
            AddressableAsync,
        }

        public static string StartupSceneName = "Startup.unity";
        public static string MainSceneName = "Main.unity";
        public static EResLoadMode LoadSceneMode = EResLoadMode.ResourceSync;
        public static EResLoadMode LoadPreloadsMode = EResLoadMode.AddressableAsync;

        public static UnityEngine.UI.Text loadingText;

        private static void Init() {
            Debug.Log($"StartupLoader - Init");
#if ADDRESSABLES_LOG_ALL
            Debug.Log($"Addressables ADDRESSABLES_LOG_ALL enabled");
#endif
            Addressables.InitializeAsync().Completed += OnAddressablesInit;
            BackgroundLoader.Init();
            _loadState = ELoadState.None;
            var _ = Load(); // async
        }

        static void OnAddressablesInit(AsyncOperationHandle<IResourceLocator> obj)
        {
            Debug.Log($"StartupLoader - OnAddressablesInit - time: {SimpleTimer.NowTime:f3}");
        }

        /// <summary>is ever used once</summary>
        public static bool IsUsed {
            get { return _loadState > ELoadState.None; }
        }
        /// <summary>is all startup loaded</summary>
        public static bool IsLoaded {
            get { return _loadState >= ELoadState.Loaded; }
        }
        /// <summary>is running loading something</summary>
        public static bool IsRunning {
            get { return _loadState > ELoadState.None && _loadState < ELoadState.Loaded; }
        }

        public static bool IsFaulted { get; private set; }

        public static int ErrorRetriedCount { get; private set; }

        public static async Task Load(StartupLoadOptions options = null) {
            if (IsRunning) {
                return;
            }
            Debug.Log("StartupLoader - Load");
            SimpleTimer.StartCheck("StartupLoad");
            options = options ?? new StartupLoadOptions(true);
            IsFaulted = false;
            ErrorRetriedCount = 0;
            _loadState = ELoadState.LoadingAssets;
            initTasksData();

            BackgroundLoader.SetGameStateBusy(true);

            if (options.preload_assets) {
                addTask("preloadAssets", 1.5f, LoadPreloadsMode);
            }
            if (options.scene) {
                var taskData = addTask("loadScene", 1.1f, LoadSceneMode);
                taskData.ignore_load_time = true;
            }

            if (options.preload_assets) {
                await PreLoadAssets();
            }

            _loadState = ELoadState.ChangingScene;
            if (options.scene) {
                await LoadScene();
            }

            onLoadAllFinish();
		}

        static void onLoadAllFinish() {
            Debug.Log("StartupLoader - onLoadAllFinish");
            _loadState = ELoadState.Loaded;
            SimpleTimer.FinishCheck("StartupLoad");
            BackgroundLoader.SetGameStateBusy(false);
        }

        static void onLoadStepFinish(StartupTaskData taskData) {
            string name = taskData.name;
            switch (taskData.load_mode)
            {
                case EResLoadMode.ResourceSync:
                {
                    Debug.Log($"StartupLoader - {name} done.");
                    break;
                }
                default:
                {
                    if (taskData.handle.Status == AsyncOperationStatus.Succeeded) {
                        Debug.Log($"StartupLoader - {name} done.");
                    } else {
                        Debug.LogError($"StartupLoader - {name} failed!");
                    }
                    break;
                }
            }
            Debug.Log($" - task: {name} finish. time: {SimpleTimer.NowTime:F3}");
        }

        // load and changes the scene
        static async Task LoadScene() {
            Debug.Log("StartupLoader - LoadScene ...");
            var taskData = getTask("loadScene");
            switch (LoadSceneMode)
            {
                case EResLoadMode.AddressableAsync:
                {
                    var handle = Addressables.LoadSceneAsync(MainSceneName);
                    taskData.handle = handle;
                    Debug.Log("StartupLoader - await loadScene Task ...");
                    await handle.Task;
                    Debug.Log("StartupLoader - resolve loadScene Task");
                    onLoadStepFinish(taskData);
                    break;
                }
                case EResLoadMode.ResourceSync:
                default:
                {
                    var name = ResUtil.ToSimpleKey(MainSceneName);
                    SceneManager.LoadScene(name);
                    onLoadStepFinish(taskData);
                    break;
                }
            }
        }

		// preload assets, save to cache, save prefabs
		public static async Task PreLoadAssets() {
            bool isSuccess = false;
            bool loop = true;
            int retry = 0;
            int retriedCount = 0;
            int retryMax = 10;
            // retryMax = 3; // local debug code for fake error
            int retryDelayMs = 100;
            StartupTaskData taskData = getTask("preloadAssets");
            while (!isSuccess && loop && retry <= retryMax)
            {
                retriedCount = retry;
                string retryMsg = retry > 0 ? (" (retry: #" + retry + ")") : "";
                Debug.Log("StartupLoader - preloadAssets ..." + retryMsg);
                string assetsLabel = "preload";
                // assetsLabel += (retry < 10 ? "XXX" : ""); // local debug code for fake error
                var handle = ResLoader.LoadAssetsAsync<UnityEngine.Object>(assetsLabel, null);
                taskData.handle = handle;
                Debug.Log("StartupLoader - await preloadAssets Task ...");
                await handle.Task;
                Debug.Log("StartupLoader - resolve preloadAssets Task");
                isSuccess = handle.Status == AsyncOperationStatus.Succeeded;
                Debug.Log($"StartupLoader - preloadAssets isSuccess: {isSuccess}, handle.Status: {handle.Status}");
                if (!isSuccess) {
                    retry++;
                    Debug.LogWarning("StartupLoader - preloadAssets fail! Wait for retry: #" + retry);
                    await TaskUtil.Delay(retryDelayMs);
                }

                loop = !isSuccess && Application.isPlaying;
            }
            ErrorRetriedCount += retriedCount;
            if (isSuccess && retriedCount > 0) {
                Debug.Log("StartupLoader - preloadAssets retry success! retriedCount: " + retriedCount.ToString());
            }
            if (!isSuccess) {
                IsFaulted = true;
                Debug.LogError("StartupLoader - preloadAssets IsFaulted!");
            }
            onLoadStepFinish(taskData);
		}

        // Unity Update
        private void Update() {
            var showLoading = false;
            var showPercent = false;
            if (IsRunning) {
                showLoading = true;
                showPercent = true;
            }

            if (loadingText) {
                if (!showLoading) {
                    loadingText.text = "";
                } else if (!showPercent) {
                    loadingText.text = "Loading...";
                } else {
                    loadingText.text = String.Format("Loading... ({0}%)", (getPercent() * 100).ToString("0"));
                }
            }
        }

        private static float getPercent() {
            float loadedSize = 0f;
            float totalSize = 0.001f;
            string log_msg = "";
            foreach (var item in map_tasksData)
            {
                var data = item.Value;
                float perc = data.getStepPercent();
                float size = data.size;
                if (data.ignore_load_time) {
                    continue;
                }
                loadedSize += perc * size;
                totalSize += size;
            }

            if (Debug.isDebugBuild) {
                Debug.Log($" - loading: {log_msg}  loaded: {loadedSize}/{totalSize}");
            }

            var percent = (loadedSize) / (totalSize + 0.0001f);
            return percent;
        }

        private static void initTasksData() {
            map_tasksData = new Dictionary<string, StartupTaskData>();
        }

        private static StartupTaskData getTask(string name) {
            if (map_tasksData == null) initTasksData();
            StartupTaskData data;
            map_tasksData.TryGetValue(name, out data);
            return data;
        }
        
        private static StartupTaskData addTask(string name, float size, EResLoadMode load_mode) {
            if (map_tasksData == null) initTasksData();
            Debug.Assert(!map_tasksData.ContainsKey(name));
            map_tasksData[name] = new StartupTaskData(name, size, load_mode);
            return map_tasksData[name];
        }

        private static ELoadState _loadState = ELoadState.None;

        private static Dictionary<string, StartupTaskData> map_tasksData;
    }
}