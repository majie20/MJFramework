using UnityEngine;

namespace MGame.Model
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
            Game.Instance.EventSystem.AddListener2(EventType.GameLoadComplete, 0, OnGameLoadComplete);

            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByUWR($"{Application.streamingAssetsPath}/AssetBundleRes/AssetBundleRes"));
        }

        private void OnDisable()
        {
            Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
        }

        private void Update()
        {
            Game.Instance.LifecycleSystem.Update(Time.deltaTime);
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameUpdate(Time.deltaTime);
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
        }

        private void OnGameLoadComplete(object[] args)
        {
            Game.Instance.EventSystem.RemoveListener2(EventType.GameLoadComplete, OnGameLoadComplete);
            Debug.Log($"------完成------{args.Length}");
            Game.Instance.Hotfix.LoadHotfixAssembly();
            Game.Instance.Hotfix.GotoHotfix();
        }
    }
}