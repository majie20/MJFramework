using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class AssetsComponent : Component, IAwake
    {
        private Dictionary<string, AssetBundle> assetBundleDic;

        private Dictionary<string, Object> assetsCiteMatchConfigDic;

        public void Awake()
        {
            assetBundleDic = new Dictionary<string, AssetBundle>();
            assetsCiteMatchConfigDic = new Dictionary<string, Object>();
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
            Debug.LogWarning("正在加载资源"); // MDEBUG:
            HotComponent component = Game.Instance.Scene.GetComponent<HotComponent>();

            for (int i = 0; i < component.AbConfigs.Count; i++)
            {
                var abName = component.AbConfigs[i].ABName;
                var abPackPath = FileHelper.JoinPath(
                    $"{HotConfig.AB_SAVE_RELATIVELY_PATH}{Path.GetFileNameWithoutExtension(abName)}{component.Settings.ABExtension}",
                    isHot ? FileHelper.FilePos.PersistentDataPath : FileHelper.FilePos.StreamingAssetsPath,
                    FileHelper.LoadMode.UnityWebRequest);
                var buffer = await FileHelper.LoadFileByUnityWebRequestAsync(abPackPath);
                buffer = AESHelper.Decrypt(buffer, component.Settings.EncryptPassword);
                var abcr = AssetBundle.LoadFromMemoryAsync(buffer);
                await UniTask.WaitUntil(() => abcr.isDone);

                assetBundleDic.Add(Regex.Replace(abName, FileConfig.FILE_EXTENSION_PATTERN, ""), abcr.assetBundle);
            }

            var abr = assetBundleDic["config"]
                .LoadAssetAsync<AssetsCiteMatchConfigSettings>("AssetsCiteMatchConfigSettings");
            await UniTask.WaitUntil(() => abr.isDone);

            if (abr.asset is AssetsCiteMatchConfigSettings settings)
            {
                var pathList = settings.PathList;
                var assetsList = settings.AssetsList;
                for (int i = 0; i < pathList.Count; i++)
                {
                    assetsCiteMatchConfigDic.Add(pathList[i], assetsList[i]);
                }
            }

            var abAtlas = assetBundleDic["config"]
                .LoadAssetAsync<AssetsAtlasSettings>("AssetsAtlasSettings");
            await UniTask.WaitUntil(() => abAtlas.isDone);
            if (abAtlas.asset is AssetsAtlasSettings atlasInfo)
            {
                var nameList = atlasInfo.atlasNameList;
                var objAtlas = atlasInfo.atlasList;
                for (int i = 0; i < objAtlas.Count; i++)
                {
                    assetsCiteMatchConfigDic.Add(nameList[i], objAtlas[i]);
                }
            }


            Game.Instance.EventSystem.Invoke(EventType.GameLoadComplete, 1000);
        }
    }
}