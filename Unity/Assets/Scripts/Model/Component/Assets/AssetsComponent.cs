using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Model
{
    [LifeCycle]
    public class AssetsComponent : Component, IAwake
    {
        /// <summary>
        /// AB包名、收藏家
        /// </summary>
        private Dictionary<string, ReferenceCollector> prefabCollectors;

        private Dictionary<string, ReferenceCollector> jsonDataCollectors;
        private Dictionary<string, ReferenceCollector> textCollectors;

        /// <summary>
        /// 预制体名、AB包名
        /// </summary>
        private Dictionary<string, string> prefabDic;

        public void Awake()
        {
            prefabCollectors = new Dictionary<string, ReferenceCollector>();
            jsonDataCollectors = new Dictionary<string, ReferenceCollector>();
            textCollectors = new Dictionary<string, ReferenceCollector>();
            prefabDic = new Dictionary<string, string>();
        }

        public override void Dispose()
        {
            prefabCollectors = null;
            jsonDataCollectors = null;
            textCollectors = null;
            prefabDic = null;
            Entity = null;
        }

        public IEnumerable<ReferenceCollector> GetAllPrefabJsonDataCollector()
        {
            return jsonDataCollectors.Values;
        }

        public IEnumerable<ReferenceCollector> GetAllTextDataCollector()
        {
            return textCollectors.Values;
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

            yield return ProcessAssetBundleManifest(ab, path, FileHelper.LoadMode.UnityWebRequest);
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

            yield return ProcessAssetBundleManifest(abcr.assetBundle, path, FileHelper.LoadMode.Stream);
        }

        private IEnumerator ProcessAssetBundleManifest(AssetBundle ab, string path, FileHelper.LoadMode mode)
        {
            var abr = ab.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return abr;

            var manifest = abr.asset as AssetBundleManifest;
            var abNames = manifest.GetAllAssetBundles();
            foreach (var name in abNames)
            {
                switch (mode)
                {
                    case FileHelper.LoadMode.UnityWebRequest:
                        yield return ProcessAssetBundleToUWR(path, name);
                        break;

                    case FileHelper.LoadMode.Stream:
                        yield return ProcessAssetBundleToIO(path, name);
                        break;
                }
            }

            //foreach (var key in prefabDic.Keys)
            //{
            //    Debug.Log($"{key}----{prefabDic[key]}");
            //}

            Game.Instance.EventSystem.Invoke<AssetBundleLoadComplete>();
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

            yield return CollectPrefab(abcr, name);
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

            yield return CollectPrefab(abcr, name);
        }

        private IEnumerator CollectPrefab(AssetBundleCreateRequest abcr, string name)
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

            var abr3 = abcr.assetBundle.LoadAssetAsync<GameObject>("TextReferenceCollector");
            yield return abr3;

            if (abr3.asset is GameObject obj3)
            {
                textCollectors.Add(name, obj3.GetComponent<ReferenceCollector>());
            }
        }

        #endregion 协程

        #region 异步

        public async Task LoadAssetBundleManifestByUWRAsync(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败------");
                return;
            }

            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            while (!abcr.isDone)
                await Task.Delay(50);

            await ProcessAssetBundleManifestAsync(abcr, path, FileHelper.LoadMode.Stream);
        }

        public async Task LoadAssetBundleManifestByIOAsync(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"路径[{path}]的AB包获取失败------");
                return;
            }

            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
            while (!abcr.isDone)
                await Task.Delay(50);

            await ProcessAssetBundleManifestAsync(abcr, path, FileHelper.LoadMode.Stream);
        }

        private async Task ProcessAssetBundleManifestAsync(AssetBundleCreateRequest abcr, string path, FileHelper.LoadMode mode)
        {
            var abr = abcr.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            while (!abr.isDone)
                await Task.Delay(50);

            var manifest = abr.asset as AssetBundleManifest;
            var abNames = manifest.GetAllAssetBundles();
            foreach (var name in abNames)
            {
                switch (mode)
                {
                    case FileHelper.LoadMode.UnityWebRequest:
                        await ProcessAssetBundleToUWRAsync(path, name);
                        break;

                    case FileHelper.LoadMode.Stream:
                        await ProcessAssetBundleToIOAsync(path, name);
                        break;
                }
            }

            //foreach (var key in prefabDic.Keys)
            //{
            //    Debug.Log($"{key}----{prefabDic[key]}");
            //}

            Game.Instance.EventSystem.Invoke<AssetBundleLoadComplete>();
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
            while (!abcr.isDone)
                await Task.Delay(50);

            await CollectPrefabAsync(abcr, name);
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
            while (!abcr.isDone)
                await Task.Delay(50);

            await CollectPrefabAsync(abcr, name);
        }

        private async Task CollectPrefabAsync(AssetBundleCreateRequest abcr, string name)
        {
            var abr1 = abcr.assetBundle.LoadAssetAsync<GameObject>("PrefabReferenceCollector");
            while (!abr1.isDone)
                await Task.Delay(50);

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
            while (!abr2.isDone)
                await Task.Delay(50);

            if (abr2.asset is GameObject obj2)
            {
                jsonDataCollectors.Add(name, obj2.GetComponent<ReferenceCollector>());
            }

            var abr3 = abcr.assetBundle.LoadAssetAsync<GameObject>("TextReferenceCollector");
            while (!abr3.isDone)
                await Task.Delay(50);

            if (abr3.asset is GameObject obj3)
            {
                textCollectors.Add(name, obj3.GetComponent<ReferenceCollector>());
            }
        }

        #endregion 异步
    }
}