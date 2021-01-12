using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Singleton
{
    public class ABMgr : Singleton<ABMgr>
    {
        private Dictionary<string, ReferenceCollector> prefabCollectors;
        private Dictionary<string, ReferenceCollector> jsonDataCollectors;

        private Dictionary<string, string> prefabDic;

        public override void Init()
        {
            base.Init();

            prefabCollectors = new Dictionary<string, ReferenceCollector>();
            jsonDataCollectors = new Dictionary<string, ReferenceCollector>();
            prefabDic = new Dictionary<string, string>();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public IEnumerator LoadAssetBundleByUnityWebRequest(List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                var r = UnityWebRequest.Get(paths[i]);
                yield return r.SendWebRequest();
                if (r.isNetworkError || r.isHttpError)
                {
                    Debug.Log(r.error);
                }
                else
                {
                    var abcr = AssetBundle.LoadFromMemoryAsync(r.downloadHandler.data);
                    while (!abcr.isDone)
                    {
                        yield return null;    // 协程等待
                    }
                    var abRequest = abcr.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                    yield return abRequest;
                    while (!abRequest.isDone)
                    {
                        yield return null;    // 协程等待
                    }
                    var manifest = abRequest.asset as AssetBundleManifest;
                    foreach (var item in manifest.GetAllAssetBundles())
                    {
                        Debug.Log(item);
                        //var abr = assetBundles[0].LoadAssetAsync<GameObject>("Cube");
                        //yield return r;
                        //ab.assetBundle.lo
                    }
                }
            }
        }

        public IEnumerator LoadAssetBundleManifestFileByIO(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log($"路径[{path}]的AB包获取失败------");
                yield break;
            }

            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            while (!abcr.isDone)
            {
                yield return null; // 协程等待
            }

            var abRequest = abcr.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            while (!abRequest.isDone)
            {
                yield return null; // 协程等待
            }

            var manifest = abRequest.asset as AssetBundleManifest;
            var anNames = manifest.GetAllAssetBundles();
            foreach (var item in anNames)
            {
                var p = $"{Path.GetDirectoryName(path)}/{item}";
                if (!File.Exists(p))
                {
                    Debug.Log($"路径[{path}]的AB包获取失败------");
                    break;
                }
                AssetBundleCreateRequest abcrSub = AssetBundle.LoadFromFileAsync(p);
                while (!abcrSub.isDone)
                {
                    yield return null; // 协程等待
                }

                var abRequestSub1 = abcrSub.assetBundle.LoadAssetAsync<GameObject>("PrefabReferenceCollector");
                while (!abRequestSub1.isDone)
                {
                    yield return null; // 协程等待
                }

                if (abRequestSub1.asset is GameObject obj)
                {
                    var component = obj.GetComponent<ReferenceCollector>();
                    for (int i = 0; i < component.data.Count; i++)
                    {
                        if (!prefabDic.ContainsKey(component.data[i].key))
                        {
                            prefabDic.Add(component.data[i].key, item);
                        }
                    }
                    prefabCollectors.Add(item, component);
                }

                var abRequestSub2 = abcrSub.assetBundle.LoadAssetAsync<GameObject>("JsonReferenceCollector");
                while (!abRequestSub2.isDone)
                {
                    yield return null; // 协程等待
                }
                if (abRequestSub2.asset is ReferenceCollector rc2)
                    jsonDataCollectors.Add(item, rc2);
            }

            foreach (var key in prefabDic.Keys)
            {
                Debug.Log($"{key}----{prefabDic[key]}");
            }

            EventSystem.Instance.Run<AssetBundleLoadComplete>();
        }
    }
}