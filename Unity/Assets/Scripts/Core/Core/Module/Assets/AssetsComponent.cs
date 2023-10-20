using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using YooAsset;
using static Model.AssetReferenceSettings;

namespace Model
{
    //public struct LoadData
    //{
    //    public OperationHandleBase OperationHandle;
    //    public long                FileSize;
    //    public long                LastFileSize;
    //    public bool                IsRawFile;
    //    public string              Path;
    //}

    public class AssetsComponent : Component, IAwake
    {
        private Dictionary<string, AssetOperationHandle>     _assetOperationDic;
        private Dictionary<string, SubAssetsOperationHandle> _subAssetOperationDic;
        private Dictionary<string, string>                   _rawFilePathDic;

        private Dictionary<string, HashSet<string>> _assetReferenceMap;

        //private HashSet<string>                              _spriteAssetMap;
        private List<string> _tempList;

        private AssetsBundleSettings _abSettings;

        public AssetsBundleSettings ABSettings
        {
            private set { _abSettings = value; }
            get { return _abSettings; }
        }

        private EPlayMode _playMode;

        public EPlayMode PlayMode
        {
            get { return _playMode; }
        }

        private string _hostServerURL;

        public string HostServerURL
        {
            get { return _hostServerURL; }
        }

        public void Awake()
        {
            ABSettings = Resources.Load<AssetsBundleSettings>("AssetsBundleSettings");
            _playMode = ABSettings.PlayMode;
            _hostServerURL = GetHostServerURL();
            _assetReferenceMap = new Dictionary<string, HashSet<string>>();
            _assetOperationDic = new Dictionary<string, AssetOperationHandle>(50);
            _subAssetOperationDic = new Dictionary<string, SubAssetsOperationHandle>(10);
            _rawFilePathDic = new Dictionary<string, string>();
            //_spriteAssetMap = new HashSet<string>(20);
            _tempList = new List<string>();
        }

        public override void Dispose()
        {
            Resources.UnloadAsset(ABSettings);
            ABSettings = null;

            foreach (var handle in _assetOperationDic)
            {
                handle.Value.Release();
            }

            foreach (var handle in _subAssetOperationDic)
            {
                handle.Value.Release();
            }

            _assetReferenceMap = null;
            _assetOperationDic = null;
            _subAssetOperationDic = null;
            _rawFilePathDic = null;

            base.Dispose();
        }

        public async UniTask Init()
        {
            NLog.Log.Debug($"------开始初始化资源系统------");
            // 初始化资源系统
            YooAssets.Initialize();

            // 创建默认的资源包
            var package = YooAssets.CreatePackage(ABSettings.PackageName);

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);

            InitializeParameters parameters = null;

            if (PlayMode == EPlayMode.WebPlayMode)
            {
                YooAssets.SetCacheSystemDisableCacheOnWebGL();

                YooAssets.SetDownloadSystemUnityWebRequest(url => { return new UnityWebRequest($"{_hostServerURL}/{Path.GetFileName(url)}", UnityWebRequest.kHttpVerbGET); });

                var initParameters = new WebPlayModeParameters();
                initParameters.BuildinQueryServices = new QueryStreamingAssetsFileServices();
                initParameters.RemoteServices = new QueryHostServerFileServices();
                parameters = initParameters;
            }
            else
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

                if (PlayMode == EPlayMode.EditorSimulateMode) // 编辑器模拟模式
                {
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(ABSettings.PackageName);
                    initParameters.DecryptionServices = services;
                    parameters = initParameters;
                }
                else if (PlayMode == EPlayMode.OfflinePlayMode) // 单机模式
                {
                    var initParameters = new OfflinePlayModeParameters();
                    initParameters.DecryptionServices = services;
                    parameters = initParameters;
                }
                else if (PlayMode == EPlayMode.HostPlayMode) // 联机模式
                {
                    var initParameters = new HostPlayModeParameters();
                    initParameters.DecryptionServices = services;
                    initParameters.BuildinQueryServices = new QueryStreamingAssetsFileServices();
                    initParameters.RemoteServices = new QueryHostServerFileServices();
                    parameters = initParameters;
                }
            }

            var initOperation = package.InitializeAsync(parameters);
            await initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            }

            await Run();
        }

        public async UniTask Run()
        {
#if WX&&!UNITY_EDITOR
            var wxFileSystemManagerComponent = Game.Instance.Scene.GetComponent<WXFileSystemManagerComponent>();
            var dirPath = FileHelper.JoinPath($"RawFile", FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);

            if (!await wxFileSystemManagerComponent.FileExists(dirPath))
            {
                await wxFileSystemManagerComponent.Mkdir(dirPath);
            }
#endif

            await LoadAssetReferenceSettingsAsync(ConstData.INIT_ARS, true);
        }

        #region 资源加载

        #region 同步

        public UnityEngine.Object LoadSync(Type type, string sign)
        {
            if (!_assetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                throw new Exception($"LoadSync type : {type},sign : {sign}的资源没有加载！");
            }

            return handle.AssetObject;
        }

        public T LoadSync<T>(string sign) where T : UnityEngine.Object
        {
            return LoadSync(typeof(T), sign) as T;
        }

        public SubAssetsOperationHandle LoadSubSync(Type type, string sign)
        {
            if (!_subAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                throw new Exception($"LoadSubSync type : {type},sign : {sign}的资源没有加载！");
            }

            return handle;
        }

        public SubAssetsOperationHandle LoadSubSync<A>(string sign) where A : UnityEngine.Object
        {
            return LoadSubSync(typeof(A), sign);
        }

        public B LoadSubSync<A, B>(string sign, string subSign) where A : UnityEngine.Object where B : UnityEngine.Object
        {
            var handle = LoadSubSync<A>(sign);

            return handle.GetSubAssetObject<B>(subSign);
        }

        #endregion 同步

        #region 异步

        #region Sub

        public async UniTask<SubAssetsOperationHandle> LoadSubAsync(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }

            if (!_subAssetOperationDic.TryGetValue(sign, out SubAssetsOperationHandle handle))
            {
                handle = YooAssets.LoadSubAssetsAsync(sign, type);
                _subAssetOperationDic.Add(sign, handle);
            }

            if (!handle.IsDone)
            {
                await handle.ToUniTask();
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

            if (!_assetOperationDic.TryGetValue(sign, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(sign, type);
                _assetOperationDic.Add(sign, handle);
            }

            if (!handle.IsDone)
            {
                await handle.ToUniTask();
            }

            return handle.AssetObject;
        }

        public async UniTask<T> LoadAsync<T>(string sign, bool isCover = false) where T : UnityEngine.Object
        {
            return await LoadAsync(typeof(T), sign, isCover) as T;
        }

        public async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string sign, LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = false)
        {
            SceneOperationHandle handle = YooAssets.LoadSceneAsync(sign, sceneMode, activateOnLoad);
            await handle.ToUniTask();

            return handle.SceneObject;
        }

        public async UniTask<string> LoadRawFileAsync(string sign, bool isCover = false)
        {
            if (_rawFilePathDic.TryGetValue(sign, out var path) && !isCover)
            {
                return path;
            }

#if WX&&!UNITY_EDITOR
            var (result, rawFilePath) = await HasSaveRawFile(sign);

            if (result)
            {
                _rawFilePathDic[sign] = rawFilePath;
                return rawFilePath;
            }
#endif
            var handle = YooAssets.LoadRawFileAsync(sign);
            await handle.ToUniTask();

            await SaveRawFile(sign);

            return _rawFilePathDic[sign];
        }

        public async UniTask<AssetReferenceSettings> LoadAssetReferenceSettingsAsync(string sign, bool isCover = false)
        {
            var handle = CreateLoadHandle(typeof(AssetReferenceSettings), sign, isCover);

            await handle.ToUniTask();
            var settings = handle.AssetObject as AssetReferenceSettings;

            List<UniTask> tasks = new List<UniTask>();
            var assembly = typeof(GameObject).Assembly;

            foreach (var info in settings.InstantlyAssetDataList)
            {
                HandleAssetReference(info.Path, sign, isCover);

                var classType = assembly.GetType(info.TypeName);

                if (info.Type == LoadType.Normal)
                {
                    if (classType == typeof(AssetReferenceSettings))
                    {
                        tasks.Add(LoadAssetReferenceSettingsAsync(info.Path, isCover));
                    }
                    else
                    {
                        tasks.Add(CreateLoadHandle(classType, info.Path, isCover).ToUniTask());
                    }
                }
                else if (info.Type == LoadType.Sub)
                {
                    tasks.Add(CreateLoadSubHandle(classType, info.Path, isCover).ToUniTask());
                }
            }

            tasks.AddRange(SaveRawFileList(settings.InstantlyAssetDataList));

            await UniTask.WhenAll(tasks);

            if (settings.RearAssetDataList.Count > 0)
            {
                LoadAssetDataListAsync(settings.RearAssetDataList, sign, isCover);
            }

            return settings;
        }

        public async void LoadAssetDataListAsync(List<Info> infoList, string sign, bool isCover = false)
        {
            var assembly = typeof(GameObject).Assembly;

            foreach (var info in infoList)
            {
                var path = info.Path;
                var type = info.Type;
                var typeName = info.TypeName;

                if (type == LoadType.Normal)
                {
                    var classType = assembly.GetType(typeName);

                    if (classType == typeof(AssetReferenceSettings))
                    {
                        await LoadAssetReferenceSettingsAsync(path, isCover);
                    }
                    else
                    {
                        await CreateLoadHandle(classType, path, isCover);
                    }

                    HandleAssetReference(path, sign, isCover);
                }
                else if (type == LoadType.Sub)
                {
                    await CreateLoadSubHandle(assembly.GetType(typeName), path, isCover);
                    HandleAssetReference(path, sign, isCover);
                }
                else if (type == LoadType.RawFile)
                {
#if WX &&!UNITY_EDITOR
                    var (result, rawFilePath) = await HasSaveRawFile(path);

                    if (result)
                    {
                        _rawFilePathDic[path] = rawFilePath;
                        NLog.Log.Debug(_rawFilePathDic[path]);

                        continue;
                    }
#endif

                    await CreateLoadRawFileHandle(path);
                    await SaveRawFile(path);
                }
            }
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

        public async UniTask ClearPackageUnusedCacheFilesAsync()
        {
            await YooAssets.ClearPackageUnusedCacheFilesAsync().ToUniTask();
        }

        public void Unload(string sign)
        {
            if (_assetOperationDic.TryGetValue(sign, out var handle))
            {
                handle.Release();
                _assetOperationDic.Remove(sign);
            }
        }

        public void UnloadSub(string sign)
        {
            if (_subAssetOperationDic.TryGetValue(sign, out var handle))
            {
                handle.Release();
                _subAssetOperationDic.Remove(sign);
            }
        }

        public void UnloadUIView(string prefabPath, string atlasPath)
        {
            Unload(prefabPath);

            _tempList.Clear();

            foreach (var v in _assetOperationDic)
            {
                if (v.Key.StartsWith(atlasPath))
                {
                    _tempList.Add(v.Key);
                }
            }

            for (int i = _tempList.Count - 1; i >= 0; i--)
            {
                Unload(_tempList[i]);
            }

            _tempList.Clear();
        }

        public void UnloadAssetReferenceSettings(string sign)
        {
            var settings = LoadSync<AssetReferenceSettings>(sign);

            UnloadAssetInfoList(settings.InstantlyAssetDataList, sign);
            UnloadAssetInfoList(settings.RearAssetDataList, sign);

            if (!_assetReferenceMap.ContainsKey(sign))
            {
                Unload(sign);
            }
        }

        public void UnloadAssetInfoList(List<Info> infoList, string sign)
        {
            var fullName = typeof(AssetReferenceSettings).FullName;

            for (int i = infoList.Count - 1; i >= 0; i--)
            {
                var path = infoList[i].Path;
                var type = infoList[i].Type;

                if (infoList[i].TypeName == fullName)
                {
                    UnloadAssetReferenceSettings(path);
                }

                if (_assetReferenceMap.TryGetValue(path, out var set))
                {
                    if (set.Contains(sign))
                    {
                        set.Remove(sign);
                    }

                    if (set.Count == 0)
                    {
                        if (type == LoadType.Normal)
                        {
                            Unload(path);
                        }
                        else if (type == LoadType.Sub)
                        {
                            UnloadSub(path);
                        }

                        _assetReferenceMap.Remove(path);
                    }
                }
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

            if (!_assetOperationDic.TryGetValue(sign, out var handle))
            {
                handle = YooAssets.LoadAssetAsync(sign, type);
                _assetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public SubAssetsOperationHandle CreateLoadSubHandle(Type type, string sign, bool isCover = false)
        {
            if (isCover)
            {
                UnloadSub(sign);
            }

            if (!_subAssetOperationDic.TryGetValue(sign, out var handle))
            {
                handle = YooAssets.LoadSubAssetsAsync(sign, type);
                _subAssetOperationDic.Add(sign, handle);
            }

            return handle;
        }

        public RawFileOperationHandle CreateLoadRawFileHandle(string sign)
        {
            return YooAssets.LoadRawFileAsync(sign);
        }

        public SceneOperationHandle CreateLoadSceneHandle(string sign, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false)
        {
            return YooAssets.LoadSceneAsync(sign, sceneMode, suspendLoad);
        }

        #endregion 创建加载处理

        public void HandleAssetReference(string path, string sign, bool isCover)
        {
            if (_assetReferenceMap.TryGetValue(path, out var set))
            {
                if (isCover)
                {
                    set.Clear();
                }
            }
            else
            {
                set = new HashSet<string>();

                _assetReferenceMap.Add(path, set);
            }

            if (!set.Contains(sign))
            {
                set.Add(sign);
            }
        }

#if WX &&!UNITY_EDITOR
        public async UniTask<(bool, string)> HasSaveRawFile(string path)
        {
            var rawFilePath = FileHelper.JoinPath($"RawFile/{GetBundleName(path)}", FileHelper.FilePos.PersistentDataPath, FileHelper.LoadMode.Stream);

            return (await Game.Instance.Scene.GetComponent<WXFileSystemManagerComponent>().FileExists(rawFilePath), rawFilePath);
        }
#endif

        public IEnumerable<UniTask> SaveRawFileList(List<Info> list)
        {
            foreach (var info in list)
            {
                if (info.Type == LoadType.RawFile)
                {
                    yield return SaveRawFile(info.Path);
                }
            }
        }

        public async UniTask SaveRawFile(string path)
        {
#if WX &&!UNITY_EDITOR
            var (result, rawFilePath) = await HasSaveRawFile(path);

            await Game.Instance.Scene.GetComponent<WXFileSystemManagerComponent>()
               .CopyFile($"{WeChatWASM.WXBase.PluginCachePath}/{PlatformHelper.GetPlatformSign()}/StreamingAssets/{GetBundleName(path)}", rawFilePath);

            _rawFilePathDic[path] = rawFilePath;
            NLog.Log.Debug(_rawFilePathDic[path]);
#else
            _rawFilePathDic[path] = GetCachedDataFilePath(path);
            NLog.Log.Debug(_rawFilePathDic[path]);
#endif
        }

        public string GetBundleName(string path)
        {
            var package = YooAssets.GetPackage(ABSettings.PackageName);
            var assetInfo = YooAssets.GetAssetInfo(path);

            return package.BundleServices.GetBundleInfo(assetInfo).Bundle.FileName;
        }

        public long GetFileSize(string path)
        {
            var package = YooAssets.GetPackage(ABSettings.PackageName);
            var assetInfo = YooAssets.GetAssetInfo(path);

            return package.BundleServices.GetBundleInfo(assetInfo).Bundle.FileSize;
        }

        public string GetCachedDataFilePath(string path)
        {
            var package = YooAssets.GetPackage(ABSettings.PackageName);
            var assetInfo = YooAssets.GetAssetInfo(path);

            return package.BundleServices.GetBundleInfo(assetInfo).Bundle.CachedDataFilePath;
        }

        public AssetInfo[] GetAssetInfos(string tag)
        {
            return YooAssets.GetAssetInfos(tag);
        }

        public AssetInfo[] GetAssetInfos(string[] tags)
        {
            return YooAssets.GetAssetInfos(tags);
        }

        public string GetRawFilePath(string sign)
        {
            if (_rawFilePathDic.TryGetValue(sign, out var path))
            {
                return path;
            }

            return null;
        }

        public bool ContainsKey(string sign)
        {
            return _assetOperationDic.ContainsKey(sign) || _subAssetOperationDic.ContainsKey(sign);
        }

        public string GetHostServerURL()
        {
            string platform = PlatformHelper.GetPlatformSign();

            if (_playMode == EPlayMode.WebPlayMode)
            {
#if MBuild
#if WX
                return $"http://192.168.31.141:8080/{platform}/StreamingAssets";
#elif TT
                return $"https://636c-cloud1-4gc89nkzefa0a08a-1321213358.tcb.qcloud.la/{platform}";
#else
                return $"http://192.168.1.7:8080/{platform}";
#endif
                //return $"http://192.168.31.141:8080/{platform}";
#else
                return $"http://192.168.31.141:8080/{platform}";
#endif
            }
            else
            {
                string packageName = ABSettings.PackageName;
#if MBuild
                return $"http://192.168.31.141:8080/{platform}/{packageName}";
#else
                return $"http://192.168.31.141:8080/{platform}/{packageName}";
#endif
            }
        }
    }
}