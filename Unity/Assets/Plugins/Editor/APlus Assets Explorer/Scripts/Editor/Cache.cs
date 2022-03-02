//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Threading;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

namespace APlus
{
    public class APCache
    {
        public const string LOAD_FROM_LOCAL_KEY = "LOAD_FROM_LOCAL_KEY";
        static string ASSETS_PATH = Path.Combine(Application.persistentDataPath, "A+AssetsExplorer.cache");
        static string ICON_CACHE_PATH = Path.Combine(Application.persistentDataPath, "A+CacheIcon.cache");

        // data cache to improve preformance
        //
        private static Dictionary<int, Dictionary<string, APAsset>> AssetsCache = new Dictionary<int, Dictionary<string, APAsset>>();
        private static Dictionary<string, string> IconCache = new Dictionary<string, string>();
        private static Blacklist blacklistStorage = new Blacklist();

        /// <summary>
        /// Load cache data
        /// </summary>
        public static void LoadDataIntoCache(System.Action callback = null)
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("A+ Assets Explorer", "Refresh cache operation is not allowed in play mode", "OK");
                return;
            }

            AssetsCache.Clear();
            IconCache.Clear();

            EditorCoroutine.StartCoroutine(LoadDataIntoCacheCoroutine(callback));
        }

        private static IEnumerator LoadDataIntoCacheCoroutine(System.Action callback = null)
        {
            try
            {
                // Fix bug that Assets will be unloaded when refresh cache data.
                // -----
                // Resources.UnloadAsset() will unload asset in memory, objects loaded 
                // in current Scene will be unloaded too. that's not the correct way and 
                // will crash Unity Editor in some case. (Bug reported by Pete Rivett-Carnac)
                //
#if UNITY_5_3_OR_NEWER && UNITY_EDITOR
                var currentScene = EditorSceneManager.GetActiveScene();
                if (currentScene.isDirty)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                }

                string currentScenePath = currentScene.path;
                if (!EditorApplication.isPlaying)
                {
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                }
#else
                string currentScene = EditorApplication.currentScene;
                if (EditorApplication.isSceneDirty)
                {
                    EditorApplication.SaveCurrentSceneIfUserWantsTo();
                }

                EditorApplication.NewScene();
#endif

                float totalCategoriesCount = 14f;
                ShowProcessBar(0);
                LoadResourcesIntoCache(APAssetType.Texture);
                ShowProcessBar(1.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Model);
                ShowProcessBar(2.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.AudioClip);
                ShowProcessBar(3.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.MovieTexture);
                ShowProcessBar(4.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Font);
                ShowProcessBar(5.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Material);
                ShowProcessBar(6.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Shader);
                ShowProcessBar(7.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.AnimationClip);
                ShowProcessBar(8.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Prefab);
                ShowProcessBar(9.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.StreamingAssets);
                ShowProcessBar(10.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Script);
                ShowProcessBar(11.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Blacklist);
                ShowProcessBar(12.0f / totalCategoriesCount);
                LoadResourcesIntoCache(APAssetType.Others);

                if (IsShowProcessBarState())
                {
                    Debug.Log("A+ Assets Explorer cache data was created.");
                }

                EditorPrefs.DeleteKey(LOAD_FROM_LOCAL_KEY);
                SaveToLocal();
                ShowProcessBar(13.0f / totalCategoriesCount);
                System.GC.Collect();
                ShowProcessBar(1);

                if (callback != null)
                {
                    callback.Invoke();
                }

#if UNITY_5_3_OR_NEWER
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                }
#else
                if (!string.IsNullOrEmpty(currentScene))
                {
                    EditorApplication.OpenScene(currentScene);
                }
#endif
            }
            catch
            {
                throw;
            }
            finally
            {
                // Need to clear progress bar whether load cache successs
                // or not
                //
                EditorUtility.ClearProgressBar();
            }

            yield return Resources.UnloadUnusedAssets();
            yield return null;
        }

        public static void SaveToLocal()
        {
            CommitBlacklistChange();
            SaveToLocal(ASSETS_PATH, AssetsCache);
            SaveToLocal(ICON_CACHE_PATH, IconCache);
        }

        private static EditorCoroutine saveAsync;
        public static void SaveToLocalAsync()
        {
            if (saveAsync != null)
            {
                saveAsync.Stop();
            }

            saveAsync = EditorCoroutine.StartCoroutine(SaveToLocalAsyncCortinue());
        }

        private static IEnumerator SaveToLocalAsyncCortinue()
        {
            SaveToLocal();
            yield return null;
        }

        public static bool LoadFromLocal()
        {
            blacklistStorage.Load();
            LoadFromLocal<Dictionary<string, string>>(ICON_CACHE_PATH, ref IconCache);
            bool result = LoadFromLocal<Dictionary<int, Dictionary<string, APAsset>>>(ASSETS_PATH, ref AssetsCache);
            if (result)
            {
                LoadResourcesIntoCache(APAssetType.Blacklist);
            }

            return result;
        }

        private static void SaveToLocal(string filePath, object data)
        {
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#if APLUS_DEV
            var now = DateTime.Now;
#endif
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(stream, data);

                    byte[] bytes = new byte[stream.Length];

                    // Have to reset the stream Position to 0 to ensure read real bytes
                    //
                    stream.Position = 0;
                    stream.Read(bytes, 0, bytes.Length);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
#if APLUS_DEV
            Utility.DebugLog(String.Format("SaveToLocal cost {0} ms", (DateTime.Now - now).TotalMilliseconds));
#endif
        }

        private static bool LoadFromLocal<T>(string filePath, ref T data) where T : class
        {
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#if APLUS_DEV
            var now = DateTime.Now;
#endif

            try
            {
                if (File.Exists(filePath))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();

                        try
                        {
                            data = serializer.Deserialize(fileStream) as T;
#if APLUS_DEV
                            Utility.DebugLog(String.Format("LoadFromLocal cost {0} ms", (DateTime.Now - now).TotalMilliseconds));
#endif
                        }
                        catch
                        {
                            // Debug.LogError(e);
                            return false;
                        }

                        if (AssetsCache == null)
                        {
                            return false;
                        }

                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsShowProcessBarState()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode
                && !EditorApplication.isPlaying
                && !EditorApplication.isPaused
                && !EditorPrefs.HasKey(LOAD_FROM_LOCAL_KEY);
        }

        private static void ShowProcessBar(float progress)
        {
            if (IsShowProcessBarState())
            {
                string tile = "A+ Assets Explorer";
                string info = "A+ is creating cache data...";
                EditorUtility.DisplayProgressBar(tile, info, progress);
            }
        }

        public static void UpdateUsedStatus(HashSet<string> usedFiles)
        {
            SetAllAssetsToUnUsed();

            // check the assets in :
            //  1. the build report
            //  2. the enabled scenes
            //  3. the AssetBundles
            //  4. the PlayerSettings
            //
            UpdateUnusedStatusInternal(usedFiles);
            UpdateUsedStatusFromScene();
            UpdateUsedStatusFromAssetBundle();
            UpdateUnusedForProjectSettings();
        }

        public static void SetAllAssetsToUnUsed()
        {
            foreach (var assetDict in AssetsCache.Values)
            {
                foreach (var keyVal in assetDict)
                {
                    keyVal.Value.Used = false;
                }
            }
        }

        public static void UpdateUnusedForProjectSettings()
        {
            HashSet<string> unusedAssetsSet = new HashSet<string>();
            var textures = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
            foreach (var tex in textures)
            {
                var path = AssetDatabase.GetAssetPath(tex);
                if (!string.IsNullOrEmpty(path))
                {
                    unusedAssetsSet.Add(path);
                }
            }

#if UNITY_5_5_OR_NEWER
            if (PlayerSettings.defaultCursor != null)
            {
                unusedAssetsSet.Add(AssetDatabase.GetAssetPath(PlayerSettings.defaultCursor));
            }
#endif

            UpdateUnusedStatusInternal(unusedAssetsSet);
        }

        private static void UpdateUnusedStatusInternal(HashSet<string> usedFiles, bool includedInAssetBundle = false)
        {
            Utility.DebugLog("UpdateUnusedStatusInternal");
            foreach (var assetDict in AssetsCache.Values)
            {
                foreach (var keyVal in assetDict)
                {
                    if (( Utility.IsInResources(keyVal.Value.Path) ||
                        Utility.IsStreamingAssetsFile(keyVal.Value.Path)) && !includedInAssetBundle)
                    {
                        keyVal.Value.Used = true;
                    }
                    else
                    {
                        string filePath = keyVal.Value.Path.Replace('\\', '/');

                        // if it's the asset bundle assets set, we just set InAssetBundle to True
                        //
                        if (includedInAssetBundle)
                        {
                            keyVal.Value.InAssetBundle = usedFiles.Contains(filePath);
                        }
                        else
                        {
                            if (usedFiles.Contains(filePath))
                            {
                                keyVal.Value.Used = true;
                            }
                        }
                    }

                    Utility.UpdateJsonInAsset(keyVal.Value);
                }
            }
        }

        private static void UpdateUsedStatusFromScene()
        {
            var ScenesList = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            var unusedAssets = AssetDatabase.GetDependencies(ScenesList);
            HashSet<string> unusedAssetsSet = new HashSet<string>();

            foreach (var path in unusedAssets)
            {
                unusedAssetsSet.Add(path);
            }

            UpdateUnusedStatusInternal(unusedAssetsSet);
        }

        public static void UpdateUsedStatusFromAssetBundle()
        {
            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            HashSet<string> pathSet = new HashSet<string>();
            foreach (var name in assetBundleNames)
            {
                var paths = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPathsFromAssetBundle(name));
                foreach (var path in paths)
                {
                    pathSet.Add(path);
                }
            }

            UpdateUnusedStatusInternal(pathSet, true);
        }

        public static void ReloadCache(System.Action callback = null)
        {
            LoadDataIntoCache(callback);
        }

        public static void MoveTo(string assetid, string newPath)
        {
            foreach (var KeyValue in AssetsCache)
            {
                if (KeyValue.Value.ContainsKey(assetid))
                {
                    var asset = KeyValue.Value[assetid];
                    asset.Path = newPath;
                    asset.Used = null;
                    Utility.UpdateJsonInAsset(asset);

                    KeyValue.Value[assetid] = asset;
                    break;
                }
            }
        }

        public static void Remove(string assetid)
        {
            foreach (var KeyValue in AssetsCache)
            {
                if (KeyValue.Value.ContainsKey(assetid))
                {
                    KeyValue.Value.Remove(assetid);
                    break;
                }
            }
        }

        public static void Remove(APAssetType category, string assetid)
        {
            if (HasAsset(category, assetid))
            {
                AssetsCache[(int)category].Remove(assetid);
            }
        }

        public static APAsset GetValue(string assetid)
        {
            foreach (var keyVaue in AssetsCache)
            {
                if (keyVaue.Value.ContainsKey(assetid))
                {
                    return keyVaue.Value[assetid];
                }
            }

            return null;
        }

        public static APAsset GetValue(APAssetType category, string assetid)
        {
            if (HasAsset(category, assetid))
            {
                return AssetsCache[(int)category][assetid];
            }
            else
            {
                return null;
            }
        }

        public static void Add(APAssetType category, string assetid, APAsset value)
        {
            Utility.UpdateJsonInAsset(value);
            SetValue(category, assetid, value);
        }

        public static bool HasCategory(APAssetType category)
        {
            return AssetsCache.ContainsKey((int)category);
        }

        public static bool HasAsset(string assetid)
        {
            foreach (var keyVaue in AssetsCache)
            {
                if (keyVaue.Value.ContainsKey(assetid))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAsset(APAssetType category, string assetid)
        {
            if (HasCategory(category) && !string.IsNullOrEmpty(assetid))
            {
                return AssetsCache[(int)category].ContainsKey(assetid);
            }

            return false;
        }

        public static void SetValue(APAssetType category, string assetid, APAsset value)
        {
            Utility.UpdateJsonInAsset(value);
            if (HasCategory(category))
            {
                if (AssetsCache[(int)category].ContainsKey(assetid))
                {
                    AssetsCache[(int)category][assetid] = value;
                }
                else
                {
                    AssetsCache[(int)category].Add(assetid, value);
                }
            }
            else
            {
                var assetDict = new Dictionary<string, APAsset>();
                assetDict.Add(assetid, value);
                AssetsCache.Add((int)category, assetDict);
            }
        }

        public static List<T> GetAssetsListByTypeFromCache<T>(APAssetType type) where T : class
        {
            if (HasCategory(type))
            {
                List<T> data = new List<T>();
                foreach (var item in AssetsCache[(int)type].Values)
                {
                    data.Add(item as T);
                }

                return data;
            }
            else
            {
                return null;
            }
        }

        public static string GetAssetsLitJsonByTypeFromCache(APAssetType type)
        {
            if (HasCategory(type))
            {
                return GetJsonFromList(AssetsCache[(int)type].Values);
            }
            else
            {
                return "[]";
            }
        }

        public static string GetJsonFromList(IEnumerable<APAsset> assets)
        {
            if (assets == null || assets.Count() == 0)
            {
                return "[]";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in assets)
            {
                if (item != null && !string.IsNullOrEmpty(item.Json))
                {
                    sb.Append(item.Json);
                    sb.Append(",");
                }
            }

            if (sb.Length > 1)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }

        private static void LoadResourcesIntoCache(APAssetType type)
        {
            IList assets = null;
            Dictionary<string, APAsset> dict = new Dictionary<string, APAsset>();
            switch (type)
            {
                case APAssetType.Font:
                    assets = APResources.GetFonts();
                    break;
                case APAssetType.MovieTexture:
                    assets = APResources.GetMovies();
                    break;
                case APAssetType.Texture:
                    assets = APResources.GetTextures();
                    break;
                case APAssetType.Model:
                    assets = APResources.GetModels();
                    break;
                case APAssetType.AudioClip:
                    assets = APResources.GetAudios();
                    break;
                case APAssetType.Material:
                    assets = APResources.GetMaterials();
                    break;
                case APAssetType.Shader:
                    assets = APResources.GetShaders();
                    break;
                case APAssetType.AnimationClip:
                    assets = APResources.GetAnimations();
                    break;
                case APAssetType.Prefab:
                    assets = APResources.GetPrefabs();
                    break;
                case APAssetType.StreamingAssets:
                    assets = APResources.GetStreamingAssets();
                    break;
                case APAssetType.Script:
                    assets = APResources.GetCodeFiles();
                    break;
                case APAssetType.Blacklist:
                    assets = blacklistStorage.GetAssets();
                    break;
                case APAssetType.Others:
                    assets = APResources.GetOthers();
                    break;
            }

            if (assets == null)
            {
                return;
            }

            foreach (var item in assets)
            {
                var asset = item as APAsset;
                if (asset == null)
                {
                    break;
                }

                Utility.UpdateJsonInAsset(asset);

                if (!dict.ContainsKey(asset.Id))
                {
                    dict.Add(asset.Id, asset);
                }
                else
                {
                    dict[asset.Id] = asset;
                }
            }

            int key = (int)type;

            if (AssetsCache.ContainsKey(key))
            {
                AssetsCache[key] = dict;
            }
            else
            {
                AssetsCache.Add(key, dict);
            }
        }

        public static void AddIcon(string key, string data)
        {
            if (IconCache.ContainsKey(key))
            {
                IconCache[key] = data;
            }
            else
            {
                IconCache.Add(key, data);
            }
        }

        public static string GetIconCacheJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var keyVal in IconCache)
            {
                sb.Append(string.Format("\"{0}\":\"{1}\",", keyVal.Key, Utility.SafeJson(keyVal.Value)));
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            return sb.ToString();
        }

        public static void LoadBlacklist()
        {
            if (blacklistStorage != null)
            {
                blacklistStorage.Load();
            }
        }

        public static void AddToBlacklist(string path, APAsset asset)
        {
            if (!blacklistStorage.Exists(path))
            {
                blacklistStorage.Add(path);
                SetValue(APAssetType.Blacklist, path, asset);
            }
        }

        public static void RemoveFromBlacklist(string path)
        {
            if (blacklistStorage.Exists(path))
            {
                blacklistStorage.Remove(path);
                Remove(APAssetType.Blacklist, path);
            }
        }

        public static bool ExistsInBlacklist(string path)
        {
            return blacklistStorage.Exists(path);
        }

        public static void CommitBlacklistChange()
        {
            blacklistStorage.Save();
        }

        public static List<string> GetReferences(string selectedAssetPath, string progressInfo, float startProgress, float endProgress, ref bool cancel)
        {
            string title = "Find references";
            EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress);
            var span = endProgress - startProgress;
            List<string> references = new List<string>();

            references.AddRange(GetReferences<APPrefab>(selectedAssetPath, APAssetType.Prefab));
            if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress + 0.2f * span))
            {
                cancel = true;
                return references;
            }

            references.AddRange(GetReferences<APModel>(selectedAssetPath, APAssetType.Model));
            if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress + 0.4f * span))
            {
                cancel = true;
                return references;
            }

            references.AddRange(GetReferences<APMaterial>(selectedAssetPath, APAssetType.Material));
            if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress + 0.6f * span))
            {
                cancel = true;
                return references;
            }

            references.AddRange(GetReferences<APOtherFile>(selectedAssetPath, APAssetType.Others));
            if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress + 0.8f * span))
            {
                cancel = true;
                return references;
            }

            references.AddRange(GetReferencesInScene(selectedAssetPath, ref cancel));
            if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, startProgress + 1f * span))
            {
                cancel = true;
            }

            return references;
        }

        public static List<string> GetReferencesInScene(string selectedAssetPath,
                                                        ref bool cancel,
                                                        bool withProgress = false,
                                                        string progressInfo = "",
                                                        float startProgress = 0,
                                                        float endProcess = 1)
        {
            string title = "Find references";
            var sceneGuids = AssetDatabase.FindAssets("t:scene");
            List<string> references = new List<string>();

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                var dependences = AssetDatabase.GetDependencies(new string[] { scenePath });
                if (dependences.Any(denpend => denpend.Equals(selectedAssetPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    references.Add(scenePath);
                }

                if (withProgress)
                {
                    float progress = startProgress + (endProcess - startProgress) * (i + 1) * 1f / sceneGuids.Length;
                    if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, progress))
                    {
                        cancel = true;
                        break;
                    }
                }
            }

            return references;
        }

        private static List<string> GetReferences<T>(string assetPath, APAssetType type) where T : APAsset
        {
            List<string> references = new List<string>();
            var lookForSet = APCache.GetAssetsListByTypeFromCache<T>(type);

            foreach (var asset in lookForSet)
            {
                var dependences = AssetDatabase.GetDependencies(new string[] { asset.Path });
                if (dependences.Any(denpend => denpend.Equals(assetPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    references.Add(asset.Path);
                }
            }

            return references;
        }

        public static List<string> GetReferencesByType<T>(string assetPath, APAssetType type, string progressInfo, float startProgress, float endProcess, ref bool cancel) where T : APAsset
        {
            List<string> references = new List<string>();
            var lookForSet = APCache.GetAssetsListByTypeFromCache<T>(type);

            string title = "Find references";
            float progress = 0;
            for (int i = 0; i < lookForSet.Count; i++)
            {
                var dependences = AssetDatabase.GetDependencies(new string[] { lookForSet[i].Path });
                if (dependences.Any(denpend => denpend.Equals(assetPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    references.Add(lookForSet[i].Path);
                }

                progress = startProgress + (endProcess - startProgress) * (i + 1) * 1f / lookForSet.Count;
                if (EditorUtility.DisplayCancelableProgressBar(title, progressInfo, progress))
                {
                    cancel = true;
                    break;
                };
            }

            return references;
        }
    }
}
#endif