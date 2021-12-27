using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class Init : MonoBehaviour
    {
        private int aa = 0;
        public Button button;

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            Game.Instance.Init();
        }

        private void OnEnable()
        {
            Game.Instance.EventSystem.AddListener<int>(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void Start()
        {
            Game.Instance.Scene.GetComponent<HotComponent>().Run(false);
        }

        private void OnDisable()
        {
            //Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void Update()
        {
            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameUpdate(Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                //Game.Instance.Hotfix.GotoHotfix();
            }
        }

        private void LateUpdate()
        {
            Game.Instance.LifecycleSystem.LateUpdate();
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameLateUpdate();
            }
        }

        private void OnApplicationQuit()
        {
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameApplicationQuit();
            }
            Game.Instance.Dispose();
        }

        private void OnGameLoadComplete(int a)
        {
            Debug.Log($"------完成------{a}");
            Game.Instance.EventSystem.RemoveListener<int>(EventType.GameLoadComplete, OnGameLoadComplete);
            UIRootComponent uiRootComponent = ObjectHelper.CreateComponent<UIRootComponent>(ObjectHelper.CreatEntity(Game.Instance.Scene, null, UIRootComponent.UIROOT_PATH, true), false);
            ObjectHelper.CreateComponent<UIManagerComponent>(ObjectHelper.CreatEntity2(uiRootComponent.Entity, uiRootComponent.Entity.Transform.Find(UIManagerComponent.GAME_OBJECT_NAME).gameObject, GameObjPoolComponent.None_GameObject), false);

            ObjectHelper.OpenUIView<NetTestComponent>();
            ObjectHelper.CreateComponent<SpriteComponent>(Game.Instance.Scene, false);
            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();
        }
    }
}