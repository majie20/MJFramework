using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Model
{
    public class AssetsComponent : Component, IAwake<bool>
    {
        private Dictionary<string, AssetBundle> assetBundleDic;

        private Dictionary<string, Object> assetsCiteMatchConfigDic;

        private int curProgress;

        public int CurProgress
        {
            private set
            {
                curProgress = value;
            }
            get
            {
                return curProgress;
            }
        }

        private bool isUseABPackPlay;

        public bool IsUseABPackPlay
        {
            private set
            {
                isUseABPackPlay = value;
            }
            get
            {
                return isUseABPackPlay;
            }
        }

        public void Awake(bool a)
        {
            IsUseABPackPlay = a;
            assetBundleDic = new Dictionary<string, AssetBundle>();
            assetsCiteMatchConfigDic = new Dictionary<string, Object>();

            Add(Resources.Load<AssetsCiteMatchConfigSettings>("AssetsCiteMatchConfigSettings"));
        }

        public override void Dispose()
        {
            assetBundleDic = null;
            assetsCiteMatchConfigDic = null;
            Entity = null;
        }

        public T Load<T>(string sign) where T : UnityEngine.Object
        {
            if (assetsCiteMatchConfigDic.ContainsKey(sign))
            {
                return (T)assetsCiteMatchConfigDic[sign];
            }

            return default;
        }

        public Object Load(string sign)
        {
            if (assetsCiteMatchConfigDic.ContainsKey(sign))
            {
                return assetsCiteMatchConfigDic[sign];
            }

            return default;
        }

        public void Unload(string name, bool unloadAllLoadedObjects)
        {
            var ab = assetBundleDic[name];
            assetBundleDic.Remove(name);
            ab.Unload(unloadAllLoadedObjects);
        }

        public async UniTask Run(bool isHot)
        {
            EventSystem eventSystem = Game.Instance.EventSystem;
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssets);
            CurProgress = 0;
            HotComponent component = Game.Instance.Scene.GetComponent<HotComponent>();
            int sumSize = component.SumSize;
            int[] progressList = new int[component.AbConfigs.Count];

            for (int i = 0; i < component.AbConfigs.Count; i++)
            {
                var size = component.AbConfigs[i].Size;
                var encryptPassword = component.Settings.EncryptPassword;
                var abName = component.AbConfigs[i].ABName;
                var abPackPath = FileHelper.JoinPath(
                    $"{HotConfig.AB_SAVE_RELATIVELY_PATH}{Path.GetFileNameWithoutExtension(abName)}{component.Settings.ABExtension}",
                    isHot ? FileHelper.FilePos.PersistentDataPath : FileHelper.FilePos.StreamingAssetsPath,
                    FileHelper.LoadMode.UnityWebRequest);
                var buffer = await FileHelper.LoadFileByUnityWebRequestAsync(abPackPath, f =>
                {
                    CurProgress -= progressList[i];
                    progressList[i] = Mathf.FloorToInt(size * f);
                    CurProgress += progressList[i];
                    eventSystem.Invoke<LoadingViewProgressRefresh, float>((float)CurProgress / sumSize);
                });
                buffer = AESHelper.Decrypt(buffer, encryptPassword);
                var abcr = AssetBundle.LoadFromMemoryAsync(buffer);
                await UniTask.WaitUntil(() => abcr.isDone);

                assetBundleDic.Add(Regex.Replace(abName, FileValue.FILE_EXTENSION_PATTERN, ""), abcr.assetBundle);
            }

            var abr = assetBundleDic["config"].LoadAssetAsync<AssetsCiteMatchConfigSettings>("AssetsCiteMatchConfigSettings");
            await UniTask.WaitUntil(() => abr.isDone);

            if (abr.asset is AssetsCiteMatchConfigSettings settings)
            {
                Add(settings);
            }

            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsComplete);
            Game.Instance.EventSystem.Invoke(EventType.GameLoadComplete);
        }

#if UNITY_EDITOR

        public void Run()
        {
            EventSystem eventSystem = Game.Instance.EventSystem;
            Add(AssetDatabase.LoadAssetAtPath<AssetsCiteMatchConfigSettings>($"{FileValue.BUILD_AB_RES_PATH}Config/AssetsCiteMatchConfigSettings.asset"));
            eventSystem.Invoke<LoadStateSwitch, LoadType>(LoadType.LoadAssetsComplete);
            Game.Instance.EventSystem.Invoke(EventType.GameLoadComplete);
        }

#endif

        public void Add(AssetsCiteMatchConfigSettings settings)
        {
            var pathList = settings.PathList;
            var assetsList = settings.AssetsList;
            for (int i = 0; i < pathList.Count; i++)
            {
                assetsCiteMatchConfigDic.Add(pathList[i], assetsList[i]);
            }
        }
    }
}