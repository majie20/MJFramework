using System;
using System.Collections;
using System.Collections.Generic;
using Game.Event;
using Game.Singleton;
using UnityEngine;
using UnityEngine.Networking;

public class GameRoot : MonoBehaviour
{
    void Awake()
    {
        
        EventSystem.Instance.Init();
        PrefabAssociateMgr.Instance.Init();
        EventSystem.Instance.Run<AssetBundleLoadComplete>();
        //PoolMgr.Instance.Init();
        //ABMgr.Instance.Init();
        //StartCoroutine(ABMgr.Instance.LoadAssetBundleManifestFileByIO("./AssetBundleRes/AssetBundleRes"));
    }

    private void OnEnable()
    {
        EventSystem.Instance.Add<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
    }

    private void OnDisable()
    {
        EventSystem.Instance.Remove<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
    }

    private void OnAssetBundleLoadComplete()
    {
       
    }
}
