using UnityEngine;

namespace Model
{
    public class Init : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            Game.Instance.Init();
        }

        private void OnEnable()
        {
            Game.Instance.EventSystem.AddListener(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void Start()
        {
            Game.Instance.Scene.GetComponent<HotComponent>().Run(false);
        }

        private void OnDisable()
        {
            //Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private async void Update()
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

        private void OnGameLoadComplete(object[] args)
        {
            Game.Instance.EventSystem.RemoveListenerMult(EventType.GameLoadComplete, OnGameLoadComplete);
            Debug.Log($"------完成------{args.Length}");
            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();
        }
    }
}