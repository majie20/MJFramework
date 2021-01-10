using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameRoot : MonoBehaviour
{
    private List<string> paths = new List<string>();

    private List<AssetBundle> assetBundles = new List<AssetBundle>();

    void Start()
    {
        //paths.Add ("http://localhost:80/AssetBundleRes/prefabs.res");
        //paths.Add ("http://localhost:80/AssetBundleRes/share_res.res");

        //StartCoroutine (ReadAssetBundleFile (paths.ToArray ()));

        Debug.Log(ABMgr.Instance.a);
    }

    void Update()
    {

    }

    private IEnumerator ReadAssetBundleFile(string[] paths)
    {
        var r = UnityWebRequest.Get("http://localhost:80/AssetBundleRes/AssetBundleRes");
        yield return r.SendWebRequest();
        if (r.isNetworkError || r.isHttpError)
        {
            Debug.Log(r.error);
        }
        else
        {
            var ab = AssetBundle.LoadFromMemoryAsync(r.downloadHandler.data);
            yield return ab;
            var abRequest = ab.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return abRequest;
            var manifest = abRequest.asset as AssetBundleManifest;
            foreach (var item in manifest.GetAllAssetBundles())
            {
                Debug.Log(item);
                var request = UnityWebRequestAssetBundle.GetAssetBundle("http://localhost:80/AssetBundleRes/" + item);

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    assetBundles.Add(DownloadHandlerAssetBundle.GetContent(request));
                }
            }

            var abr = assetBundles[0].LoadAssetAsync<GameObject>("Cube");
            yield return r;
            // GameObject cubePrefab = assetBundles[0].LoadAsset<GameObject> ("Cube");
            GameObject cubePrefab = abr.asset as GameObject;
            Instantiate(cubePrefab);
            GameObject spherePrefab = assetBundles[0].LoadAsset<GameObject>("Sphere");
            Instantiate(spherePrefab);

            ab.assetBundle.Unload(true);
            Debug.Log(ab);
            Debug.Log(ab.assetBundle);
        }
    }
}