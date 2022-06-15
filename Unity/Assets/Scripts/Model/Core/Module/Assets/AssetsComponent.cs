using CatJson;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using YooAsset;

namespace Model
{
    public class AssetsComponent : Component, IAwake
    {
        private Dictionary<string, AssetOperationHandle> AssetOperationDic;
        private Dictionary<string, SubAssetsOperationHandle> SubAssetOperationDic;

        private AssetsBundleSettings _abSettings;

        public AssetsBundleSettings ABSettings
        {
            private set { _abSettings = value; }
            get { return _abSettings; }
        }

        private Dictionary<string, UIPrefabToAtlasInfo> _uiPrefabToAtlasInfoDic;

        public Dictionary<string, UIPrefabToAtlasInfo> UIPrefabToAtlasInfoDic
        {
            private set { _uiPrefabToAtlasInfoDic = value; }
            get { return _uiPrefabToAtlasInfoDic; }
        }

        private YooAssets.EPlayMode _playMode;

        public YooAssets.EPlayMode PlayMode
        {
            private set { _playMode = value; }
            get { return _playMode; }
        }

        public void Awake()
        {
            ABSettings = Resources.Load<AssetsBundleSettings>("AssetsBundleSettings");
            PlayMode = ABSettings.PlayMode;
            AssetOperationDic = new Dictionary<string, AssetOperationHandle>();
            SubAssetOperationDic = new Dictionary<string, SubAssetsOperationHandle>();
        }

        public override void Dispose()
        {
            Resources.UnloadAsset(ABSettings);
            ABSettings = null;
            UIPrefabToAtlasInfoDic = null;
            foreach (var handle in AssetOperationDic)
            {
                handle.Value.Release();
            }

            foreach (var handle in SubAssetOperationDic)
            {
                handle.Value.Release();
            }

            AssetOperationDic = null;
            SubAssetOperationDic = null;

            base.Dispose();
        }

        public async UniTask Init()
        {
            IDecryptionServices services;
            if (_abSettings.EncryptType == EncryptType.Empty)
            {
                services = new EmptyDecrypt();
            }
            else if (_abSettings.EncryptType == EncryptType.Offset)
            {
                services = new OffsetDecrypt();
            }
            else
            {
                services = new EmptyDecrypt();
            }

            if (PlayMode == YooAssets.EPlayMode.EditorSimulateMode)// 编辑器模拟模式
            {
                var createParameters = new YooAssets.EditorSimulateModeParameters();
                createParameters.LocationServices = new DefaultLocationServices("");
                createParameters.DecryptionServices = services;
                await YooAssets.InitializeAsync(createParameters);
            }
            else if (PlayMode == YooAssets.EPlayMode.OfflinePlayMode)// 单机模式
            {
                var createParameters = new YooAssets.OfflinePlayModeParameters();
                createParameters.LocationServices = new DefaultLocationServices("");
                createParameters.DecryptionServices = services;
                await YooAssets.InitializeAsync(createParameters);
            }
            else if (PlayMode == YooAssets.EPlayMode.HostPlayMode)// 联机模式
            {
                var createParameters = new YooAssets.HostPlayModeParameters();
                createParameters.LocationServices = new DefaultLocationServices("");
                createParameters.DecryptionServices = services;
                createParameters.ClearCacheWhenDirty = false;
                createParameters.DefaultHostServer = GetHostServerURL();
                createParameters.FallbackHostServer = GetHostServerURL();
                await YooAssets.InitializeAsync(createParameters);
            }

            await Run();
        }

        #region 资源加载

        #region 同步

        public UnityEngine.Object LoadSync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                Unload(sign);
            }
            if (!AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetSync(sign, type);
                AssetOperationDic.Add(sign, handle);
            }

            return handle.AssetObject;
        }

        public T LoadSync<T>(string sign, bool isCover = false) where T : UnityEngine.Object
        {
            return LoadSync(typeof(UnityEngine.Object), sign, isCover) as T;
        }

        public SubAssetsOperationHandle LoadSubSync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsSync(sign, type);
                SubAssetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public SubAssetsOperationHandle LoadSubSync<A>(string sign, bool isCover = false) where A : UnityEngine.Object
        {
            return LoadSubSync(typeof(A), sign, isCover);
        }

        public B LoadSubSync<A, B>(string sign, string subSign, bool isCover = false) where A : UnityEngine.Object where B : UnityEngine.Object
        {
            var handle = LoadSubSync<A>(sign, isCover);

            return handle.GetSubAssetObject<B>(subSign);
        }

        #endregion 同步

        #region 异步

        #region 只加载不返回

        public async UniTask LoadAsyncOnly(AssetInfo info, bool isCover = false)
        {
            if (isCover)
            {
                Unload(info.AssetPath);
            }
            if (!AssetOperationDic.TryGetValue(info.AssetPath, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(info);
                await handle.ToUniTask();
                AssetOperationDic.Add(info.AssetPath, handle);
            }
        }

        public async UniTask LoadAsyncOnly(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                Unload(sign);
            }
            if (!AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(sign, type);
                await handle.ToUniTask();
                AssetOperationDic.Add(sign, handle);
            }
        }

        public async UniTask LoadAsyncOnly(string sign, bool isCover = false)
        {
            await LoadAsyncOnly(typeof(UnityEngine.Object), sign, isCover);
        }

        public async UniTask LoadSubAsyncOnly(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsAsync(sign, type);
                await handle.ToUniTask();
                SubAssetOperationDic.Add(sign, handle);
            }
        }

        public async UniTask LoadSubAsyncOnly<A>(string sign, bool isCover = false) where A : UnityEngine.Object
        {
            await LoadSubAsyncOnly(typeof(A), sign, isCover);
        }

        public async UniTask LoadRawFileAsyncOnly(string sign, string copyPath = null)
        {
            var handle = YooAssets.GetRawFileAsync(sign, copyPath);
            await handle.ToUniTask();
        }

        #endregion 只加载不返回

        #region Sub

        public async UniTask<SubAssetsOperationHandle> LoadSubAsync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsAsync(sign, type);
                await handle.ToUniTask();
                SubAssetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public async UniTask<SubAssetsOperationHandle> LoadSubAsync<A>(string sign, bool isCover = false) where A : UnityEngine.Object
        {
            return await LoadSubAsync(typeof(A), sign, isCover);
        }

        public async UniTask<B> LoadSubAsync<A, B>(string sign, string subSign, bool isCover = false) where A : UnityEngine.Object where B : UnityEngine.Object
        {
            var handle = await LoadSubAsync<A>(sign, isCover);

            return handle.GetSubAssetObject<B>(subSign);
        }

        #endregion Sub

        public async UniTask<UnityEngine.Object> LoadAsync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                Unload(sign);
            }
            if (!AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(sign, type);
                await handle.ToUniTask();
                AssetOperationDic.Add(sign, handle);
            }

            return handle.AssetObject;
        }

        public async UniTask<T> LoadAsync<T>(string sign, bool isCover = false) where T : UnityEngine.Object
        {
            return await LoadAsync(typeof(UnityEngine.Object), sign, isCover) as T;
        }

        public async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string sign, UnityEngine.SceneManagement.LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            SceneOperationHandle handle = YooAssets.LoadSceneAsync(sign, sceneMode, activateOnLoad);
            await handle.ToUniTask();
            return handle.SceneObject;
        }

        public async UniTask<string> LoadRawFileAsyncToStr(string sign, string copyPath = null)
        {
            var handle = YooAssets.GetRawFileAsync(sign, copyPath);
            await handle.ToUniTask();
            return handle.LoadFileText();
        }

        public async UniTask<byte[]> LoadRawFileAsyncToByteArray(string sign, string copyPath = null)
        {
            var handle = YooAssets.GetRawFileAsync(sign, copyPath);
            await handle.ToUniTask();
            return handle.LoadFileData();
        }

        #endregion 异步

        #endregion 资源加载

        #region 资源卸载

        public void UnloadUnusedAssets()
        {
            YooAssets.UnloadUnusedAssets();
        }

        public void ForceUnloadAllAssets()
        {
            YooAssets.ForceUnloadAllAssets();
            GC.Collect();
        }

        public void Unload(string sign)
        {
            if (AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle.Release();
                AssetOperationDic.Remove(sign);
            }
        }

        public void UnloadSub(string sign)
        {
            if (SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle.Release();
                SubAssetOperationDic.Remove(sign);
            }
        }

        #endregion 资源卸载

        #region 创建加载处理

        public AssetOperationHandle CreateLoadHandle(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                Unload(sign);
            }
            if (!AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(sign, type);
                AssetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public SubAssetsOperationHandle CreateLoadSubHandle(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsAsync(sign, type);
                SubAssetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public RawFileOperation CreateLoadRawFileHandle(string sign, string copyPath)
        {
            var handle = YooAssets.GetRawFileAsync(sign, copyPath);

            return handle;
        }

        public SceneOperationHandle CreateLoadSceneHandle(string sign, UnityEngine.SceneManagement.LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            SceneOperationHandle handle = YooAssets.LoadSceneAsync(sign, sceneMode, activateOnLoad);

            return handle;
        }

        #endregion 创建加载处理

        public async UniTask Run()
        {
            await LoadAssetReferenceSettingsAsync(FileValue.INIT_ARS, true);
            Unload(FileValue.INIT_ARS);

            await LoadNecessary();
        }

        public async UniTask LoadNecessary()
        {
            UIPrefabToAtlasInfoDic = JsonParser.ParseJson<Dictionary<string, UIPrefabToAtlasInfo>>((await LoadAsync<TextAsset>(FileValue.UI_PREFAB_TO_ATLAS_INFO)).text);
            Unload(FileValue.UI_PREFAB_TO_ATLAS_INFO);
        }

        public async UniTask LoadAssetReferenceSettingsAsync(string sign, bool isCover = false)
        {
            AssetReferenceSettings settings = await LoadAsync<AssetReferenceSettings>(sign, isCover);
            var list = settings.AssetPathList;
            var len = list.Count;
            var assembly = typeof(UnityEngine.GameObject).Assembly;
            for (int i = 0; i < len; i++)
            {
                var e = list[i];
                var path = e.path;
                if (e.type == LoadType.Normal)
                {
                    await LoadAsyncOnly(assembly.GetType(e.typeName), path, isCover);
                }
                else if (e.type == LoadType.Sub)
                {
                    await LoadSubAsyncOnly(assembly.GetType(e.typeName), path, isCover);
                }
                else if (e.type == LoadType.RawFile)
                {
                    await LoadRawFileAsyncOnly(path, $"{FileValue.RAW_FILE_SAVE_PATH}/{Path.GetFileName(path)}");
                }
            }
        }

        public async UniTask LoadUIAtlas(string sign)
        {
            if (_uiPrefabToAtlasInfoDic.TryGetValue(sign, out UIPrefabToAtlasInfo value))
            {
                if (!value.IsEmpty)
                {
                    await LoadSubAsyncOnly<SpriteAtlas>($"{FileValue.ATLAS_PATH}{Path.GetFileName(value.Path)}Atlas.spriteatlas");
                }
            }
        }

        public void ClearUnusedCacheFiles()
        {
            YooAssets.ClearUnusedCacheFiles();
        }

        public AssetInfo[] GetAssetInfos(string tag)
        {
            return YooAssets.GetAssetInfos(tag);
        }

        public AssetInfo[] GetAssetInfos(string[] tags)
        {
            return YooAssets.GetAssetInfos(tags);
        }

        private string GetHostServerURL()
        {
            string platform = PlatformHelper.GetPlatformSign();
#if MBuild
            return $"http://127.0.0.1:8080/{platform}";
#else
            return $"http://127.0.0.1:8080/{platform}";
#endif
        }
    }
}