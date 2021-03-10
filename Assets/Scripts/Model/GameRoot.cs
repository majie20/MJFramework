using MGame.Model;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private GameObject cube;

    private void Awake()
    {
        Game.Instance.Init();
        StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByIO("./AssetBundleRes/AssetBundleRes"));
        //StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestByUWR("http://localhost/AssetBundleRes/AssetBundleRes"));
        //ABMgr.Instance.LoadAssetBundleManifestByIOAsync("./AssetBundleRes/AssetBundleRes");
    }

    private void OnEnable()
    {
        //EventSystem.Instance.Add<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
    }

    private void OnDisable()
    {
        //EventSystem.Instance.Remove<PrefabAssociateDataLoadComplete>(OnPrefabAssociateDataLoadComplete);
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
    }
}