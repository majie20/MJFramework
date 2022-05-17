using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Model
{
    public class Init : MonoBehaviour
    {
        [Tooltip("显示注释")]
        public bool isUseABPackPlay;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Inject()
        {
            SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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

            ObjectHelper.CreateComponent<UIRootComponent>(await ObjectHelper.CreateEntity(Game.Instance.Scene, null, UIRootComponent.UIROOT_PATH, true), false);

            NLog.Log.Debug("-----这里有一段播放背景音乐播放测试");
            Game.Instance.EventSystem.Invoke<E_PlayMusic, string>("event:/BGM");

            Game.Instance.EventSystem.AddListenerAsync<E_GameLoadComplete>(this, OnGameLoadComplete);

            if (component.PlayMode == YooAsset.YooAssets.EPlayMode.HostPlayMode)
            {
                await ObjectHelper.OpenUIView<LoadingViewComponent>();
            }
            else
            {
                Game.Instance.EventSystem.InvokeAsync<E_GameLoadComplete>();
            }
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

        //private TimerBase t;
        //private TimerBase tt;

        /// <summary>
        /// PostProcessing和Timer的测试
        /// </summary>
        //private void TestAwake()
        //{
        //    t = TimerHelper.DelayDo(6, () => { Debug.Log("测试按时间执行"); }, true, 2);
        //    tt = TimerHelper.DelayDoFrame(6, () => { Debug.Log("测试按帧执行"); }, true, 2);
        //}

        /// <summary>
        /// PostProcessing和Timer的测试
        /// </summary>
        //private void TestUpdate()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        t.Pause();
        //        tt.Pause();
        //        t = TimerHelper.DelayDo(6, () => { Debug.Log("测试按时间执行"); }, false, 1);
        //        tt = TimerHelper.DelayDoFrame(6, () => { Debug.Log("测试按帧执行"); }, false, 1);
        //    }
        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        t.Start();
        //        tt.Start();
        //    }
        //    if (Input.GetMouseButtonDown(2))
        //    {
        //        t.Resume();
        //        tt.Resume();
        //    }

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        PostProcessingHelper.ShowEffect<BloodDropEffect>();
        //    }
        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        PostProcessingHelper.RecoverEffect<BloodDropEffect>();
        //    }
        //    //if (Input.GetMouseButtonDown(0))
        //    //{
        //    //    DOTween.To(() => 0.0f, t =>
        //    //    {
        //    //        postProcessing.ShowLerpEffect<BloodDropEffect>(t);
        //    //    },1.0f,1.0f);
        //    //}
        //    //if (Input.GetMouseButtonDown(1))
        //    //{
        //    //    DOTween.To(() => 1.0f, t =>
        //    //    {
        //    //        postProcessing.RecoverLerpEffect<BloodDropEffect>(t);
        //    //    }, 0.0f, 1.0f).Complete();
        //    //}
        //}

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
            Game.Instance.EventSystem.RemoveListenerAsync<E_GameLoadComplete>(this);

            ObjectHelper.CreateComponent<SpriteComponent>(Game.Instance.Scene, false);
            ObjectHelper.CloseUIView<LoadingViewComponent>();

            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();

            await ObjectHelper.OpenUIView<StartViewComponent>();
        }
    }
}