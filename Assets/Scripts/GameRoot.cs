using Game.Event;
using MGame;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private GameObject cube;

    private void Awake()
    {
        EventSystem.Instance.Init();
        PrefabAssociateMgr.Instance.Init();
        PoolMgr.Instance.Init();
        ABMgr.Instance.Init();
        StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestByIO("./AssetBundleRes/AssetBundleRes"));
        //StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestByUWR("http://localhost/AssetBundleRes/AssetBundleRes"));
        //ABMgr.Instance.LoadAssetBundleManifestByIOAsync("./AssetBundleRes/AssetBundleRes");
    }

    private void OnEnable()
    {
        EventSystem.Instance.Add<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
    }

    private void OnDisable()
    {
        EventSystem.Instance.Remove<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            cube = PoolMgr.Instance.GetGameObjByName("Cube");
            cube.transform.SetParent(null);
            cube.SetActive(true);
            cube.GetComponent<BodyManageComponent>()?.Assemble("Cube");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            cube.GetComponent<BodyManageComponent>()?.Dismemberment();
            cube = null;
        }
    }

    private void OnPrefabAssociateDataLoadComplete()
    {
        TestComponent component = new TestComponent();
        PoolMgr.Instance.RecycleComponent(component);
    }
}