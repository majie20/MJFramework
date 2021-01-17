using System;
using Game.Event;
using Game.Singleton;
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
        StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestFileByIO("./AssetBundleRes/AssetBundleRes"));
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
            cube.GetComponent<BodyConstructionComponent>()?.Assemble("Cube");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            cube.GetComponent<BodyConstructionComponent>()?.Dismemberment();
            PoolMgr.Instance.RecycleGameObj("Cube", cube);
            cube = null;
        }
    }

    private void OnPrefabAssociateDataLoadComplete()
    {

    }
}