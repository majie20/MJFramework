using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGame.Model
{
    public class Init : MonoBehaviour
    {
        void Start()
        {
            Game.Instance.Init();
            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByIO("./AssetBundleRes/AssetBundleRes"));
        }

        void Update()
        {

        }
    }
}
