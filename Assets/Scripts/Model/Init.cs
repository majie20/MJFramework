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
            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByIO("./AssetBundleRes/AssetBundleRes"));
            //StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestByUWR("http://localhost/AssetBundleRes/AssetBundleRes"));
            //ABMgr.Instance.LoadAssetBundleManifestByIOAsync("./AssetBundleRes/AssetBundleRes");
        }

        private void OnEnable()
        {
            Game.Instance.EventSystem.Add<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
            Game.Instance.EventSystem.Add<TextDataLoadComplete>(OnTextDataLoadComplete);
        }

        private void OnDisable()
        {
            Game.Instance.EventSystem.Remove<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
            Game.Instance.EventSystem.Remove<TextDataLoadComplete>(OnTextDataLoadComplete);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    cube = Game.Instance.ObjectPool.GetGameObjByName("Cube");
            //    cube.transform.SetParent(null);
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