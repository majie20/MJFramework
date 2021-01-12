using System.Collections;
using System.Collections.Generic;
using Game.Event;
using Game.Singleton;
using UnityEngine;
using UnityEngine.Networking;

public class GameRoot : MonoBehaviour
{
    void Start()
    {
        EventSystem.Instance.Init();
        PrefabAssociateMgr.Instance.Init();
        PoolMgr.Instance.Init();
        ABMgr.Instance.Init();
        StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestFileByIO("./AssetBundleRes/AssetBundleRes"));
    }
}