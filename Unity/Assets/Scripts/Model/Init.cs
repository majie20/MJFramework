using System.Threading;
using Cysharp.Threading.Tasks;
using NPBehave;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Model
{
    public class Init : MonoBehaviour
    {
        public bool isUseABPackPlay;

        //private Root behaviorTree;
        public BaseGraph BaseGraph;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Inject()
        {
            SynchronizationContext.SetSynchronizationContext(new UniTaskSynchronizationContext());
        }

        // AfterAssembliesLoaded is called before BeforeSceneLoad
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitUniTaskLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Game.Instance.Init();

            ObjectHelper.CreateComponent<AssetsComponent, bool>(Game.Instance.Scene, isUseABPackPlay, false);
            ObjectHelper.CreateComponent<SpriteComponent>(Game.Instance.Scene, false);

            var uiRootComponent = ObjectHelper.CreateComponent<UIRootComponent>(
                ObjectHelper.CreateEntity(Game.Instance.Scene, null, UIRootComponent.UIROOT_PATH, true), false);
            var uiManagerComponent = ObjectHelper.CreateComponent<UIManagerComponent>(
                ObjectHelper.CreateEntity(uiRootComponent.Entity,
                    uiRootComponent.Entity.Transform.Find(UIManagerComponent.GAME_OBJECT_NAME).gameObject), false);
            uiManagerComponent.UIBlackMaskComponent = ObjectHelper.OpenUIView<UIBlackMaskComponent>();
        }

        private void Start()
        {
            ObjectHelper.OpenUIView<LoadingViewComponent>();
            NP_BaseBehaviorTree behaviorTree = new NP_BaseBehaviorTree(BaseGraph);
            behaviorTree.Init();
            behaviorTree.Start();


            //#if UNITY_EDITOR
            //            var debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
            //            debugger.BehaviorTree = behaviorTree;
            //#endif

        }

        //异步方法，会在最后返回一个string

        private void Update()
        {
            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameUpdate(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //Game.Instance.Hotfix.GotoHotfix();
            }
        }

        private void LateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
            if (Game.Instance.Hotfix.IsRuning) Game.Instance.Hotfix.GameLateUpdate();
        }

        private void OnEnable()
        {
            Game.Instance.EventSystem.AddListener(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void OnApplicationQuit()
        {
            Game.Instance.Dispose();
        }

        private void OnGameLoadComplete()
        {
            Debug.Log("------完成------");
            Game.Instance.EventSystem.RemoveListener(EventType.GameLoadComplete, OnGameLoadComplete);

            Game.Instance.Scene.GetComponent<SpriteComponent>().InitDic(true);
            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();

            ObjectHelper.CloseUIView<LoadingViewComponent>();
            ObjectHelper.OpenUIView<VirtualJoystickViewComponent>();
        }
    }
}