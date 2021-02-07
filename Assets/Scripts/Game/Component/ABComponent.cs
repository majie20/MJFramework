using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MGame
{
    public class ABComponent : Component
    {
        private Dictionary<string, ReferenceCollector> prefabCollectors;
        private Dictionary<string, ReferenceCollector> jsonDataCollectors;

        private Dictionary<string, string> prefabDic;

        public ABComponent()
        {
        }

        public override Component Init()
        {
            prefabCollectors = new Dictionary<string, ReferenceCollector>();
            jsonDataCollectors = new Dictionary<string, ReferenceCollector>();
            prefabDic = new Dictionary<string, string>();
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
            prefabCollectors = null;
            jsonDataCollectors = null;
            prefabDic = null;
        }

        public IEnumerable<ReferenceCollector> GetAllPrefabJsonDataCollector()
        {
            return jsonDataCollectors.Values;
        }

        public GameObject GetPrefabByName(string name)
        {
            return prefabCollectors[prefabDic[name]].Get<GameObject>(name);
        }

        #region 协程

        public IEnumerator LoadAssetBundleManifestByUWR(string path)
        {
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(path);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败----{uwr.error}");
                yield break;
            }

            AssetBundle ab = (uwr.downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;

            yield return ProcessAssetBundleManifest(ab, path, LoadMode.UWR);
        }

        public IEnumerator LoadAssetBundleManifestByIO(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败------");
                yield break;
            }

            var abcr = AssetBundle.LoadFromFileAsync(path);
            yield return abcr;

            yield return ProcessAssetBundleManifest(abcr.assetBundle, path, LoadMode.IO);
        }

        private IEnumerator ProcessAssetBundleManifest(AssetBundle ab, string path, LoadMode mode)
        {
            var abr = ab.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return abr;

            var manifest = abr.asset as AssetBundleManifest;
            var anNames = manifest.GetAllAssetBundles();
            foreach (var name in anNames)
            {
                switch (mode)
                {
                    case LoadMode.UWR:
                        yield return ProcessAssetBundleToUWR(path, name);
                        break;

                    case LoadMode.IO:
                        yield return ProcessAssetBundleToIO(path, name);
                        break;
                }
            }

            //foreach (var key in prefabDic.Keys)
            //{
            //    Debug.Log($"{key}----{prefabDic[key]}");
            //}

            EventSystem.Instance.Run<AssetBundleLoadComplete>();
        }

        private IEnumerator ProcessAssetBundleToUWR(string path, string name)
        {
            var p = $"{Path.GetDirectoryName(path)}/{name}";

            UnityWebRequest uwr = UnityWebRequest.Get(p);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning($"路径[{p}]的AB包获取失败----{uwr.error}");
                yield break;
            }

            var abcr = AssetBundle.LoadFromMemoryAsync(uwr.downloadHandler.data);
            yield return abcr;

            yield return CollectoratePrefab(abcr, name);
        }

        private IEnumerator ProcessAssetBundleToIO(string path, string name)
        {
            var p = $"{Path.GetDirectoryName(path)}/{name}";

            if (!File.Exists(p))
            {
                Debug.LogWarning($"路径[{p}]的AB包获取失败------");
                yield break;
            }

            var abcr = AssetBundle.LoadFromFileAsync(p);
            yield return abcr;

            yield return CollectoratePrefab(abcr, name);
        }

        private IEnumerator CollectoratePrefab(AssetBundleCreateRequest abcr, string name)
        {
            var abr1 = abcr.assetBundle.LoadAssetAsync<GameObject>("PrefabReferenceCollector");
            yield return abr1;

            if (abr1.asset is GameObject obj1)
            {
                var collector = obj1.GetComponent<ReferenceCollector>();
                for (int i = 0; i < collector.data.Count; i++)
                {
                    if (!prefabDic.ContainsKey(collector.data[i].key))
                    {
                        prefabDic.Add(collector.data[i].key, name);
                    }
                }
                prefabCollectors.Add(name, collector);
            }

            var abr2 = abcr.assetBundle.LoadAssetAsync<GameObject>("JsonReferenceCollector");
            yield return abr2;

            if (abr2.asset is GameObject obj2)
            {
                jsonDataCollectors.Add(name, obj2.GetComponent<ReferenceCollector>());
            }
        }

        #endregion 协程

        #region 异步

        public async void LoadAssetBundleManifestByUWRAsync(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败------");
                return;
            }

            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            await Task.Run(() => { while (!abcr.isDone) { } });

            await ProcessAssetBundleManifestAsync(abcr, path, LoadMode.IO);
        }

        public async void LoadAssetBundleManifestByIOAsync(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败------");
                return;
            }

            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            await Task.Run(() => { while (!abcr.isDone) { } });

            await ProcessAssetBundleManifestAsync(abcr, path, LoadMode.IO);
        }

        private async Task ProcessAssetBundleManifestAsync(AssetBundleCreateRequest abcr, string path, LoadMode mode)
        {
            var abr = abcr.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            await Task.Run(() => { while (!abr.isDone) { } });

            var manifest = abr.asset as AssetBundleManifest;
            var anNames = manifest.GetAllAssetBundles();
            foreach (var name in anNames)
            {
                switch (mode)
                {
                    case LoadMode.UWR:
                        await ProcessAssetBundleToUWRAsync(path, name);
                        break;

                    case LoadMode.IO:
                        await ProcessAssetBundleToIOAsync(path, name);
                        break;
                }
            }

            foreach (var key in prefabDic.Keys)
            {
                Debug.Log($"{key}----{prefabDic[key]}");
            }

            EventSystem.Instance.Run<AssetBundleLoadComplete>();
        }

        private async Task ProcessAssetBundleToUWRAsync(string path, string name)
        {
            var p = $"{Path.GetDirectoryName(path)}/{name}";

            if (!File.Exists(p))
            {
                Debug.LogWarning($"路径[{p}]的AB包获取失败------");
                return;
            }

            var abcr = AssetBundle.LoadFromFileAsync(p);
            await Task.Run(() => { while (!abcr.isDone) { } });

            await CollectoratePrefabAsync(abcr, name);
        }

        private async Task ProcessAssetBundleToIOAsync(string path, string name)
        {
            var p = $"{Path.GetDirectoryName(path)}/{name}";

            if (!File.Exists(p))
            {
                Debug.LogWarning($"路径[{p}]的AB包获取失败------");
                return;
            }

            var abcr = AssetBundle.LoadFromFileAsync(p);
            await Task.Run(() => { while (!abcr.isDone) { } });

            await CollectoratePrefabAsync(abcr, name);
        }

        private async Task CollectoratePrefabAsync(AssetBundleCreateRequest abcr, string name)
        {
            var abr1 = abcr.assetBundle.LoadAssetAsync<GameObject>("PrefabReferenceCollector");
            await Task.Run(() => { while (!abr1.isDone) { } });

            if (abr1.asset is GameObject obj1)
            {
                var collector = obj1.GetComponent<ReferenceCollector>();
                for (int i = 0; i < collector.data.Count; i++)
                {
                    if (!prefabDic.ContainsKey(collector.data[i].key))
                    {
                        prefabDic.Add(collector.data[i].key, name);
                    }
                }
                prefabCollectors.Add(name, collector);
            }

            var abr2 = abcr.assetBundle.LoadAssetAsync<GameObject>("JsonReferenceCollector");
            await Task.Run(() => { while (!abr2.isDone) { } });

            if (abr2.asset is GameObject obj2)
            {
                jsonDataCollectors.Add(name, obj2.GetComponent<ReferenceCollector>());
            }
        }

        #endregion 异步

        public enum LoadMode
        {
            UWR,
            IO
        }
    }
}