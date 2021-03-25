using UnityEngine;

namespace MGame.Model
{
    public class Init : MonoBehaviour
    {
        private void Start()
        {
            Game.Instance.Init();
            StartCoroutine(Game.Instance.Scene.GetComponent<ABComponent>().LoadAssetBundleManifestByIO("./AssetBundleRes/AssetBundleRes"));
        }

        private void Update()
        {
        }
    }
}