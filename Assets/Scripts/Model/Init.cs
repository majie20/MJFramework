using UnityEngine;

namespace MGame.Model
{
    public class Init : MonoBehaviour
    {
        private int completeValue = 0;
        private bool isComplete = false;

        private void Awake()
        {
            Game.Instance.Init();
            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByUWR($"{Application.streamingAssetsPath}/AssetBundleRes/AssetBundleRes"));
        }

        private void OnEnable()
        {
            Game.Instance.EventSystem.AddListener<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete, this);
            Game.Instance.EventSystem.AddListener<TextDataLoadComplete>(OnTextDataLoadComplete, this);
        }

        private void OnDisable()
        {
            Game.Instance.EventSystem.RemoveListener<PrefabAssociateDataLoadComplete>(this);
            Game.Instance.EventSystem.RemoveListener<TextDataLoadComplete>(this);
        }

        private void Update()
        {
            if (Game.Instance.Hotfix.IsRuning)
            {
                Game.Instance.Hotfix.GameUpdate(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
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

        private void OnPrefabAssociateDataLoadComplete()
        {
            JuageComplete();
        }

        private void OnTextDataLoadComplete()
        {
            JuageComplete();
        }

        private void JuageComplete()
        {
            if (!isComplete)
            {
                completeValue++;
                if (completeValue >= 2)
                {
                    isComplete = true;
                    Debug.Log($"------完成------");
                    Game.Instance.Hotfix.LoadHotfixAssembly();
                    Game.Instance.Hotfix.GotoHotfix();
                }
            }
        }
    }
}