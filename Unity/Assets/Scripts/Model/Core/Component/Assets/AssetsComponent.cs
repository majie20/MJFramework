using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;
using Object = UnityEngine.Object;

namespace Model
{
    public class AssetsComponent : Component, IAwake
    {
        public const string INIT_LOAD_CONFIG_PATH = "Assets/Res/Config/ScriptableObject/InitAssetReferenceSettings.asset";
        public const string UI_ATLAS_LINK_CONFIG_PATH = "Assets/Res/Config/ScriptableObject/UIPrefabToAtlasSettings.asset";

        private Dictionary<string, AssetOperationHandle> AssetOperationDic;
        private Dictionary<string, SubAssetsOperationHandle> SubAssetOperationDic;

        private AssetsBundleSettings _abSettings;

        public AssetsBundleSettings ABSettings
        {
            private set { _abSettings = value; }
            get { return _abSettings; }
        }

        private UIPrefabToAtlasSettings _uiptaSettings;

        public UIPrefabToAtlasSettings UIPtaSettings
        {
            private set { _uiptaSettings = value; }
            get { return _uiptaSettings; }
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

        #region 资源加载

        #region 同步

        public Object LoadSync(Type type, string sign, bool isCover = false)
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
            return LoadSync(typeof(T), sign, isCover) as T;
        }

        public B LoadSubSync<A, B>(string sign, string subSign, bool isCover = false) where A : UnityEngine.Object where B : UnityEngine.Object
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsSync<A>(sign);
                SubAssetOperationDic.Add(sign, handle);
            }

            return handle.GetSubAssetObject<B>(subSign);
        }

        #endregion 同步

        #region 异步

        public async UniTask<Object> LoadAsync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                Unload(sign);
            }
            if (!AssetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetSync(sign, type);
                await handle.ToUniTask();
                AssetOperationDic.Add(sign, handle);
            }

            return handle.AssetObject;
        }

        public async UniTask<T> LoadAsync<T>(string sign, bool isCover = false) where T : UnityEngine.Object
        {
            return await LoadAsync(typeof(T), sign, isCover) as T;
        }

        public async UniTask<B> LoadSubAsync<A, B>(string sign, string subSign, bool isCover = false) where A : UnityEngine.Object where B : UnityEngine.Object
        {
            if (isCover)
            {
                UnloadSub(sign);
            }
            if (!SubAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsSync<A>(sign);
                await handle.ToUniTask();
                SubAssetOperationDic.Add(sign, handle);
            }

            return handle.GetSubAssetObject<B>(subSign);
        }

        public async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string sign, UnityEngine.SceneManagement.LoadSceneMode sceneMode, bool activateOnLoad, bool isCover = false)
        {
            SceneOperationHandle handle = YooAssets.LoadSceneAsync(sign, sceneMode, activateOnLoad);
            await handle.ToUniTask();
            return handle.SceneObject;
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

            if (PlayMode == YooAssets.EPlayMode.EditorPlayMode)// 编辑器模拟模式
            {
                var createParameters = new YooAssets.EditorPlayModeParameters();
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

        public async UniTask Run()
        {
            UIPtaSettings = await LoadAsync<UIPrefabToAtlasSettings>(UI_ATLAS_LINK_CONFIG_PATH);

            await LoadAssetReferenceSettingsAsync(INIT_LOAD_CONFIG_PATH);
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
                await LoadAsync(assembly.GetType(e.typeName), path, isCover);
            }
        }

        public async UniTask LoadUIAtlas(string sign)
        {
            if (UIPtaSettings.InfoDic.TryGetValue(sign, out string value))
            {
                await LoadAsync<SpriteAtlas>(value);
            }
        }

        public void ClearUnusedCacheFiles()
        {
            YooAssets.ClearUnusedCacheFiles();
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