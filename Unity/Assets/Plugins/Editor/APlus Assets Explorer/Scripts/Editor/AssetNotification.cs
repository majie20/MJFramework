//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_5_6_OR_NEWER
    using UnityEngine.Video;
    using MovieTextureType = UnityEngine.Video.VideoClip;
#else
    using MovieTextureType =  UnityEngine.MovieTexture;
#endif

namespace APlus
{
    public class AssetNotification : AssetPostprocessor
    {
        private static void UpdateModelInCache(string modelAssetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(modelAssetPath);
            webCommunicationService.UpdateObjectsIntoCache(APAssetType.Model, guid, SyncManager.ModifiedAssets);

            var animationsInCache = APCache.GetAssetsListByTypeFromCache<APAnimation>(APAssetType.AnimationClip);
            HashSet<string> animationsAssetIdInCache = new HashSet<string>();

            foreach (var animation in animationsInCache)
            {
                if (animation.Id.ToLower().Contains(guid.ToLower()))
                {
                    animationsAssetIdInCache.Add(animation.Id);
                }
            }

            var clipIds = APResources.GetAnimationClipAssetIdInModel(modelAssetPath);
            foreach (var id in clipIds)
            {
                if (animationsAssetIdInCache.Contains(id))
                {
                    webCommunicationService.UpdateObjectsIntoCache(APAssetType.AnimationClip, id, SyncManager.ModifiedAssets);
                }
                else
                {
                    var clip = APResources.GetAPAssetByPath(APAssetType.AnimationClip, id);
                    APCache.SetValue(APAssetType.AnimationClip, id, clip);
                    SyncManager.AddedAssets.Enqueue(clip);
                }
            }

            foreach (var id in animationsAssetIdInCache)
            {
                if (!clipIds.Contains(id))
                {
                    var clip = APCache.GetValue(APAssetType.AnimationClip, id);
                    APCache.Remove(id);
                    SyncManager.DeleteAssets.Enqueue(clip);
                }
            }
        }

        private static void AddModelInCache(string modelAssetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(modelAssetPath);
            var asset = APResources.GetAPAssetByPath(APAssetType.Model, guid);
            APCache.SetValue(APAssetType.Model, guid, asset);
            SyncManager.AddedAssets.Enqueue(asset);

            var animationClipAssetids = APResources.GetAnimationClipAssetIdInModel(modelAssetPath);
            foreach (var id in animationClipAssetids)
            {
                var clip = APResources.GetAPAssetByPath(APAssetType.AnimationClip, id);
                APCache.SetValue(APAssetType.AnimationClip, id, clip);
                SyncManager.AddedAssets.Enqueue(clip);
            }
        }

        private static void HandleScriptsChange(string[] importedAssets, string[] deletedAssets, string[] movedAssets)
        {
            bool hasScriptAssets = false;

            string[] assets = new string[0];
            ArrayUtility.AddRange(ref assets, importedAssets);
            ArrayUtility.AddRange(ref assets, deletedAssets);
            ArrayUtility.AddRange(ref assets, movedAssets);

            if (assets != null)
            {
                foreach (var item in assets)
                {
                    if (Utility.IsScriptAsset(item))
                    {
                        hasScriptAssets = true;
                        break;
                    }
                }
            }

            if (hasScriptAssets)
            {
                APCache.SaveToLocalAsync();
            }
        }

        private static void HandleImportedAssets(string[] importedAssets)
        {
            foreach (var assetPath in importedAssets)
            {
                var id = AssetDatabase.AssetPathToGUID(assetPath);

                if (!APCache.HasAsset(id))
                {
                    AddNewImportAssets(assetPath);
                }
                else
                {
                    UpdateReimportExistAssets(assetPath);
                }
            }

            webCommunicationService?.RefreshIconCache();
        }

        private static void HandleDeletedAssets(string[] deletedAssets)
        {
            var animationClips = APCache.GetAssetsListByTypeFromCache<APAnimation>(APAssetType.AnimationClip);

            foreach (var assetPath in deletedAssets)
            {
                Utility.DebugLog(string.Format("Deleted: {0}", assetPath));

                if (Utility.IsModelPath(assetPath))
                {
                    foreach (var clip in animationClips)
                    {
                        if (clip.Path.Contains(assetPath))
                        {
                            APCache.Remove(APAssetType.AnimationClip, clip.Id);
                            SyncManager.DeleteAssets.Enqueue(clip);
                            SyncManager.DeleteAssets.Enqueue(APResources.GetBlackListAPAsset(assetPath));
                        }
                    }
                }

                var id = AssetDatabase.AssetPathToGUID(assetPath);
                var asset = APCache.GetValue(AssetDatabase.AssetPathToGUID(assetPath));
                if (asset != null)
                {
                    APCache.Remove(id);
                    APCache.RemoveFromBlacklist(assetPath);
                    SyncManager.DeleteAssets.Enqueue(asset);
                    SyncManager.DeleteAssets.Enqueue(APResources.GetBlackListAPAsset(assetPath));
                }
            }
        }

        private static void HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (var i = 0; i < movedAssets.Length; i++)
            {
                Utility.DebugLog(string.Format("moved {0} to {1}", movedFromAssetPaths[i], movedAssets[i]));
                var sid = AssetDatabase.AssetPathToGUID(movedAssets[i]);
                if (Utility.IsModelPath(movedAssets[i]))
                {
                    var clipIds = APResources.GetAnimationClipAssetIdInModel(movedAssets[i]);
                    foreach (var id in clipIds)
                    {
                        APCache.MoveTo(sid, movedAssets[i]);
                        var clip = APCache.GetValue(APAssetType.AnimationClip, id);
                        SyncManager.MovedFromAssets.Enqueue(id);
                        SyncManager.MovedToAssets.Enqueue(clip);
                    }
                }

                APCache.MoveTo(sid, movedAssets[i]);
                var asset = APCache.GetValue(sid);
                SyncManager.MovedFromAssets.Enqueue(sid);
                SyncManager.MovedToAssets.Enqueue(asset);
            }
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            HandleImportedAssets(importedAssets);
            HandleDeletedAssets(deletedAssets);
            HandleMovedAssets(movedAssets, movedFromAssetPaths);
            HandleScriptsChange(importedAssets, deletedAssets, movedAssets);
            SyncManager.Process(webCommunicationService, true);
        }

        private static void AddNewImportAssets(string assetPath)
        {
            Utility.DebugLog(string.Format("New: {0}", assetPath));

            if (!File.Exists(assetPath) && Directory.Exists(assetPath))
            {
                return;
            }

            if (Utility.IsStreamingAssetsFile(assetPath))
            {
                APStreamingAssetsFile file = APResources.GetStreamingAssetFile(assetPath);
                APCache.Add(APAssetType.StreamingAssets, file.Id, file);
                SyncManager.AddedAssets.Enqueue(file);
                return;
            }
            else if (Utility.IsCodeFile(assetPath))
            {
                APCodeFile codeFile = APResources.GetCodeFile(assetPath);
                APCache.Add(APAssetType.Script, codeFile.Id, codeFile);
                SyncManager.AddedAssets.Enqueue(codeFile);
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            APAsset asset = null;

            // if new path
            //
            if (obj is Texture)
            {
                if (obj is MovieTextureType)
                {
                    var movie = APResources.GetAPAssetByPath(APAssetType.MovieTexture, guid);
                    if (movie != null)
                    {
                        APCache.SetValue(APAssetType.MovieTexture, movie.Id, movie);
                    }
                    SyncManager.AddedAssets.Enqueue(movie);
                    return;
                }

                asset = APResources.GetAPAssetByPath(APAssetType.Texture, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.Texture, asset.Id, asset);
                }
            }
            else if (Utility.IsModels(obj, assetPath))
            {
                AddModelInCache(assetPath);
                return;
            }
            else if (obj is AnimationClip)
            {
                asset = APResources.GetAPAssetByPath(APAssetType.AnimationClip, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.AnimationClip, asset.Id, asset);
                }
            }
            else if (obj is AudioClip)
            {
                asset = APResources.GetAPAssetByPath(APAssetType.AudioClip, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.AudioClip, asset.Id, asset);
                }
            }
            else if (obj is Font)
            {
                asset = APResources.GetAPAssetByPath(APAssetType.Font, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.Font, asset.Id, asset);
                }
            }
            else if (obj is Shader)
            {
                asset = APResources.GetAPAssetByPath(APAssetType.Shader, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.Shader, asset.Id, asset);
                }
            }
            else if (obj is Material || obj is PhysicMaterial || obj is PhysicsMaterial2D)
            {
                asset = APResources.GetAPAssetByPath(APAssetType.Material, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.Material, asset.Id, asset);
                }
            }
            else if (Utility.IsPrefab(assetPath))
            {
                asset = APResources.GetAPAssetByPath(APAssetType.Prefab, guid);
                if (asset != null)
                {
                    APCache.SetValue(APAssetType.Prefab, asset.Id, asset);
                }
            }
            else
            {
                if (assetPath.ToLower().StartsWith("assets"))
                {
                    asset = APResources.GetOtherFile(assetPath);
                    if (asset != null)
                    {
                        APCache.Add(APAssetType.Others, asset.Id, asset);
                    }
                }
            }

            if (asset != null)
            {
                SyncManager.AddedAssets.Enqueue(asset);
            }

            Utility.DebugLog("New object type = " + obj);
        }

        private static void UpdateReimportExistAssets(string assetPath)
        {
            Utility.DebugLog(string.Format("Update: {0}", assetPath));

            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (Utility.IsStreamingAssetsFile(assetPath))
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.StreamingAssets, guid, SyncManager.ModifiedAssets);
                return;
            }
            else if (Utility.IsCodeFile(assetPath))
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Script, guid, SyncManager.ModifiedAssets);
                return;
            }

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            if (obj is MovieTextureType)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.MovieTexture, guid, SyncManager.ModifiedAssets);
            }
            else if (obj is Font)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Font, guid, SyncManager.ModifiedAssets);
            }
            else if (obj is Texture)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Texture, guid, SyncManager.ModifiedAssets);
            }
            else if (obj is Shader)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Shader, guid, SyncManager.ModifiedAssets);
            }
            else if (obj is Material || obj is PhysicMaterial)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Material, guid, SyncManager.ModifiedAssets);
            }
            else if (obj is AudioClip)
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.AudioClip, guid, SyncManager.ModifiedAssets);
            }
            else if (Utility.IsModels(obj, assetPath))
            {
                UpdateModelInCache(assetPath);
            }
            else if (Utility.IsPrefab(assetPath))
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Prefab, guid, SyncManager.ModifiedAssets);
            }
            else if (Utility.IsUntyNewAnimation(assetPath))
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.AnimationClip, guid, SyncManager.ModifiedAssets);
            }
            else
            {
                webCommunicationService.UpdateObjectsIntoCache(APAssetType.Others, guid, SyncManager.ModifiedAssets);
            }
        }

        public static WebViewCommunicationService webCommunicationService;

        [InitializeOnLoadAttribute]
        public static class PrepareOnLoad
        {
            static PrepareOnLoad()
            {
                Utility.DebugLog("Executing PrepareOnLoad");
                webCommunicationService = ScriptableObject.CreateInstance<WebViewCommunicationService>();
                webCommunicationService.hideFlags = HideFlags.HideAndDontSave;

                EditorApplication.update -= BackgroundUpdate;
                EditorApplication.update -= CheckUsedUpdate;

                EditorApplication.update += BackgroundUpdate;
                EditorApplication.update += CheckUsedUpdate;

#if UNITY_2017_1_OR_NEWER
                EditorApplication.playModeStateChanged += e => { PlaymodeStateChanged(); };
#else
                EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
#endif

                time = EditorApplication.timeSinceStartup;

                // Load black list data form local file
                APCache.LoadBlacklist();

                if (PreferencesItems.AutoRefreshCacheOnProjectLoad)
                {
                    APCache.LoadDataIntoCache(CheckUnusedState);
                }
                else
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode
                    || EditorApplication.isCompiling)
                    {
                        if (!APCache.LoadFromLocal())
                        {
                            APCache.LoadDataIntoCache(CheckUnusedState);
                        }
                        else
                        {
                            CheckUnusedState();
                        }
                    }
                    else
                    {
                        if (!APCache.LoadFromLocal())
                        {
                            APCache.LoadDataIntoCache(CheckUnusedState);
                        }
                    }
                }
            }

            private static void CheckUsedUpdate()
            {
                var markFile = AssetsUsageChecker.GetMarkFile();
                if (File.Exists(markFile))
                {
                    if (!APCache.LoadFromLocal())
                    {
                        APCache.LoadDataIntoCache(CheckUnusedState);
                    }
                    else
                    {
                        CheckUnusedState();
                    }
                }


                if (File.Exists(AFTERBUILD_A_PLUS))
                {
                    if (APlusWindow.Instance != null 
                        && APlusWindow.Instance.webview != null
                        && AssetNotification.webCommunicationService != null)
                    {
                        AssetNotification.webCommunicationService.RefreshAll();
                        AssetNotification.webCommunicationService.Refresh();
                        File.Delete(AFTERBUILD_A_PLUS);
                    }
                }
            }

            public const string AFTERBUILD_A_PLUS = "Library/AFTERBUILD_A_PLUS";

            private static void CheckUnusedState()
            {
                Utility.DebugLog("CheckUnusedState");
                var markFile = AssetsUsageChecker.GetMarkFile();

                if (File.Exists(markFile))
                {
                    List<string> assets = AssetsUsageChecker.Check();
                    if (assets != null && assets.Count > 0)
                    {
                        HashSet<string> usedFiles = new HashSet<string>();
                        foreach (var item in assets)
                        {
                            usedFiles.Add(item);
                        }

                        APCache.UpdateUsedStatus(usedFiles);
                        APCache.SaveToLocal();
                        File.WriteAllText(AFTERBUILD_A_PLUS, string.Empty);
                    }

                    File.Delete(markFile);
                }
            }

            private static void PlaymodeStateChanged()
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    Utility.DebugLog("APCache.SaveToLocalAsync()");
                    APCache.SaveToLocalAsync();
                }
            }

            private static double time;
            private static void BackgroundUpdate()
            {
                TrackingWorkaround();
            }

            public const double INTERVAL = 1.5;
            private static void TrackingWorkaround()
            {
                if (EditorApplication.timeSinceStartup - time > INTERVAL)
                {
                    bool hasModifiedAsset = false;

                    foreach (var obj in Selection.objects)
                    {
                        if (obj is UnityEngine.Material)
                        {
                            hasModifiedAsset = hasModifiedAsset || HandleMaterialOnSelection(obj);
                        }

                        if (obj is GameObject)
                        {
                            hasModifiedAsset = hasModifiedAsset || HandlePrefabOnSelection(obj);
                        }

                        if (obj is AnimationClip)
                        {
                            hasModifiedAsset = hasModifiedAsset || HandleNewAnimationClipOnSelection(obj);
                        }

                        if (obj is RenderTexture)
                        {
                            hasModifiedAsset = hasModifiedAsset || HandleRenderTextureOnSelection(obj);
                        }
                    }

                    if (hasModifiedAsset)
                    {
                        SyncManager.Process(webCommunicationService, true);
                    }

                    time = EditorApplication.timeSinceStartup;
                }
            }

            private static bool HandleRenderTextureOnSelection(UnityEngine.Object obj)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var rtInCache = APCache.GetValue(guid) as APTexture;
                if (rtInCache == null)
                {
                    return false;
                }

                var rtCurrent = APResources.GetAPTextureFromAssetGuid(guid);
                if (rtInCache.Width != rtCurrent.Width
                    || rtInCache.Height != rtCurrent.Height
                    || rtInCache.MipMap != rtCurrent.MipMap)
                {

                    webCommunicationService.UpdateObjectsIntoCache(APAssetType.Texture, AssetDatabase.AssetPathToGUID(assetPath), SyncManager.ModifiedAssets);
                    return true;
                }

                return false;
            }

            private static bool HandleNewAnimationClipOnSelection(UnityEngine.Object obj)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (Utility.IsUntyNewAnimation(assetPath))
                {
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    var animationInCache = APCache.GetValue(guid) as APAnimation;

                    if (animationInCache == null)
                    {
                        return false;
                    }

                    var currentAnimation = APResources.GetAPAnimationFromClip(obj as AnimationClip);
                    currentAnimation.Id = guid;

                    if (animationInCache.CycleOffset - currentAnimation.CycleOffset > 0.001f
                        || animationInCache.LoopPose != currentAnimation.LoopPose
                        || animationInCache.LoopTime != currentAnimation.LoopTime
                        || animationInCache.FPS != currentAnimation.FPS
                        || animationInCache.InAssetBundle != currentAnimation.InAssetBundle
                        || animationInCache.Length - currentAnimation.Length > 0.001f)
                    {
                        webCommunicationService.UpdateObjectsIntoCache(APAssetType.AnimationClip, currentAnimation, SyncManager.ModifiedAssets);
                        return true;
                    }
                }

                return false;
            }

            private static bool HandlePrefabOnSelection(UnityEngine.Object obj)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (Utility.IsPrefab(assetPath))
                {
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    var prefabInCache = APCache.GetValue(guid) as APPrefab;

                    if (prefabInCache == null)
                    {
                        return false;
                    }

                    var currentPrefab = APResources.GetAPPrefabFromAssetGuid(guid);

                    if (prefabInCache.InLayers != currentPrefab.InLayers
                        || prefabInCache.InAssetBundle != currentPrefab.InAssetBundle
                        || prefabInCache.ContainTags != currentPrefab.ContainTags)
                    {

                        webCommunicationService.UpdateObjectsIntoCache(APAssetType.Prefab, currentPrefab, SyncManager.ModifiedAssets);
                        return true;
                    }
                }

                return false;
            }

            private static bool HandleMaterialOnSelection(UnityEngine.Object obj)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                var currentMaterial = (obj as Material);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                var materialInCache = APCache.GetValue(APAssetType.Material, guid) as APMaterial;
                if (materialInCache != null && !currentMaterial.shader.name.Equals(materialInCache.Shader, StringComparison.CurrentCultureIgnoreCase))
                {
                    webCommunicationService.UpdateObjectsIntoCache(APAssetType.Material, guid, SyncManager.ModifiedAssets);
                    return true;
                }

                return false;
            }
        }
    }
}

#endif