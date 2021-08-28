using UnityEngine;

namespace MGame.Model
{
    public class Init : MonoBehaviour
    {
        private GameObject cube;

        private int completeValue = 0;
        private bool isComplete = false;

        private void Awake()
        {
            Game.Instance.Init();
            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByUWR($"{Application.streamingAssetsPath}/AssetBundleRes/AssetBundleRes"));
            //StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestByUWR("http://localhost/AssetBundleRes/AssetBundleRes"));
            //ABMgr.Instance.LoadAssetBundleManifestByIOAsync("./AssetBundleRes/AssetBundleRes");
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
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    cube = Game.Instance.ObjectPool.HatchGameObjByName("Cube");
            //    cube.Transform.SetParent(null);
            //    cube.SetActive(true);
            //    cube.GetComponent<BodyManageComponent>()?.Assemble("Cube");
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    cube.GetComponent<BodyManageComponent>()?.Dismemberment();
            //    cube = null;
            //}
        }

        private void OnPrefabAssociateDataLoadComplete()
        {
            //TestComponent component = new TestComponent();
            //Game.Instance.ObjectPool.RecycleComponent(component);
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