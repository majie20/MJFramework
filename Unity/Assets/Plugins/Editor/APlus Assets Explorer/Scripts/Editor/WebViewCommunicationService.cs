//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace APlus
{
    public class WebViewCommunicationService : ScriptableObject
    {
        private const string SCRIPTOBJECTNAME = "AnyPerf";
        private CallbackWrapper wrap;
        private Webview webView;

        public WebViewCommunicationService()
        {
        }

        public void Init(Webview webView)
        {
            if (webView == null)
            {
                return;
            }

            webView.DefineScriptObject(SCRIPTOBJECTNAME, this);
            webView.SetDelegateObject(this);
            this.webView = webView;
        }

        public void DoAssetsChange(
            List<APAsset> addedAssets,
            List<APAsset> deleteAssets,
            List<APAsset> modifedAssets,
            List<string> movedFrom,
            List<APAsset> moveToAssets)
        {

            if (IsListNullOrEmpty(addedAssets)
                && IsListNullOrEmpty(deleteAssets)
                && IsListNullOrEmpty(modifedAssets)
                && IsListNullOrEmpty(movedFrom)
                && IsListNullOrEmpty(moveToAssets))
            {
                return;
            }

            string moveFromIds = movedFrom == null || movedFrom.Count == 0 ? "[]" : GetJsonFromStringList(movedFrom);
            string addedJson = EncodeJsonFromList(addedAssets);
            string deleteJson = EncodeJsonFromList(deleteAssets);
            string modifedJson = EncodeJsonFromList(modifedAssets);
            string movetoJson = EncodeJsonFromList(moveToAssets);
            if (webView != null)
            {
                string js = string.Format("window.doAssetsChange('{0}', '{1}', '{2}', '{3}', '{4}')",
                                            addedJson,
                                            deleteJson,
                                            modifedJson,
                                            CLZF2_Base64(Utility.SafeJson(moveFromIds)),
                                            movetoJson);
                webView.ExecuteJavascript(js);
                RefreshIconCache();
                Utility.DebugLog("DoAssetsChange");
            }
        }

        private string GetJsonFromStringList(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            foreach (var item in list)
            {
                sb.Append(string.Format("\"{0}\",", item));
            }

            if (sb.Length > 1)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            Utility.DebugLog(Utility.SafeJson(sb.ToString()));
            return Utility.SafeJson(sb.ToString());
        }

        private bool IsListNullOrEmpty<T>(IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        private string EncodeJsonFromList(List<APAsset> assets)
        {
            string json = APCache.GetJsonFromList(assets);
            return CLZF2_Base64(json);
        }

        public void AddAssets(List<APAsset> assets)
        {
            DoAssetsChange(assets, null, null, null, null);
        }

        public void DeleteAssets(List<APAsset> assets)
        {
            DoAssetsChange(null, assets, null, null, null);
        }

        public void SetCurrentURL(string url, string data)
        {
            if (webView == null)
            {
                return;
            }

            webView.ExecuteJavascript(string.Format("window.setCurrentURL('{0}', '{1}');", url, CLZF2_Base64(data)));
            webView.ExecuteJavascript("window.refreshSearch();");
        }

        public void Refresh()
        {
            if (webView != null)
            {
                webView.ExecuteJavascript("window.Refresh();");
                return;
            }
        }

        public void UpdateTheme()
        {
            if (webView != null)
            {
                webView.ExecuteJavascript("window.applyTheme();");
                return;
            }
        }

        public void RefreshAll()
        {
            if (webView != null)
            {
                Utility.DebugLog("RefreshAll");
                webView.ExecuteJavascript("window.RefreshAllData();");
                webView.ExecuteJavascript("window.refreshIconCache();");
            }
        }

        public void UpdateObjectsIntoCache(APAssetType type, string assetid, Queue<APAsset> modifedAssets = null)
        {
            APAsset asset = APResources.GetAPAssetByPath(type, assetid);

            if (APCache.HasAsset(type, assetid))
            {
                var previousAssets = APCache.GetValue(type, assetid);
                if (asset != null && previousAssets != null)
                {
                    asset.Used = previousAssets.Used;
                    APCache.SetValue(type, assetid, asset);
                    if (modifedAssets != null)
                    {
                        modifedAssets.Enqueue(asset);
                    }
                }
            }
        }

        public void UpdateObjectsIntoCache(APAssetType type, APAsset asset, Queue<APAsset> modifedAssets = null)
        {
            if (APCache.HasAsset(type, asset.Id))
            {
                APCache.SetValue(type, asset.Id, asset);
                if (modifedAssets != null)
                {
                    modifedAssets.Enqueue(asset);
                }
            }
        }

        public void ExecuteJSinWebView(string jsCode)
        {
            this.webView.ExecuteJavascript(jsCode);
        }

        /// <summary>
        /// Get the texture and return the DESC sort order
        /// </summary>
        private void GetResByType(string message, object callback)
        {
#if APLUS_DEV
            var now = DateTime.Now;
#endif
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("no message provide");
            }

            string json = "[]";

            switch (message.ToLower())
            {
                case AssetType.TEXTURES:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Texture);
                    break;
                case AssetType.MODELS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Model);
                    break;
                case AssetType.AUDIOS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.AudioClip);
                    break;
                case AssetType.MOVIES:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.MovieTexture);
                    break;
                case AssetType.FONTS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Font);
                    break;
                case AssetType.MATERIALS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Material);
                    break;
                case AssetType.SHADERS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Shader);
                    break;
                case AssetType.ANIMATIONS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.AnimationClip);
                    break;
                case AssetType.PREFABS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Prefab);
                    break;
                case AssetType.STREAMING_ASSETS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.StreamingAssets);
                    break;
                case AssetType.CODE:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Script);
                    break;
                case AssetType.BLACKLIST:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Blacklist);
                    break;
                case AssetType.OTHERS:
                    json = APCache.GetAssetsLitJsonByTypeFromCache(APAssetType.Others);
                    break;
                case "hierarchy":
                    json = GetHierarchyAssetsListJson();
                    break;
                default:
                    break;
            }

            json = CLZF2_Base64(json);
            wrap = new CallbackWrapper(callback);
            wrap.Send(json);

#if APLUS_DEV
            Utility.DebugLog(String.Format("Load Res {0} cost {1} ms", message, (DateTime.Now - now).TotalMilliseconds));
#endif
        }

        private string GetHierarchyAssetsListJson()
        {
            var assets = EditorUtility.CollectDependencies(GameObject.FindObjectsOfType(typeof(GameObject)));
            Dictionary<string, APHierarchyAsset> realAssets = new Dictionary<string, APHierarchyAsset>();

            foreach (var item in assets)
            {
                var assetPath = AssetDatabase.GetAssetPath(item);

                if (string.IsNullOrEmpty(assetPath)
                    || assetPath.Contains("unity_builtin_extra")
                    || assetPath.Contains("unity default resources")
                    || assetPath.Contains("unity editor resources"))
                {
                    continue;
                }

                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (realAssets.ContainsKey(guid))
                {
                    Utility.UpdateJsonInAsset(realAssets[guid]);
                }
                else
                {
                    APHierarchyAsset aphAsset = new APHierarchyAsset();

                    if (Utility.IsModelPath(assetPath) && (item is AnimationClip))
                    {
                        aphAsset.Id = Utility.GetAssetId(guid, Utility.GetLocalIndentifierOfObject(item).ToString());
                        aphAsset.Name = item.name;
                    }
                    else
                    {
                        aphAsset.Id = guid;
                        aphAsset.Name = Utility.GetFileName(assetPath);
                    }

                    aphAsset.FileType = Utility.GetFileExtension(assetPath);
                    aphAsset.Icon = APResources.GetIconID(assetPath, item is AnimationClip);
                    Utility.UpdateJsonInAsset(aphAsset);
                    realAssets.Add(guid, aphAsset);
                }
            }

            return APCache.GetJsonFromList(realAssets.Values.ToArray());
        }

        private void SelectInHierarchy(string message, object callback)
        {
            var decompressedMessage = Encoding.UTF8.GetString(CLZF2.Decompress(Convert.FromBase64String(message)));
            Utility.DebugLog("Find References in Hierarchy: " + decompressedMessage);

            var assets = decompressedMessage.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (assets.Length == 0)
            {
                return;
            }

            SearchableEditorWindow hierarchyWindow = null;
            SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));
            foreach (SearchableEditorWindow window in windows)
            {
                if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
                {
                    hierarchyWindow = window;
                    break;
                }
            }

            if (hierarchyWindow == null)
            {
                return;
            }

            var guid = Utility.GetGuidFromAssetId(assets[0]);
            var instanceId = Utility.GetInstanceIdFromAssetId(assets[0]);
            if (!string.IsNullOrEmpty(instanceId))
            {
                instanceId += ":";
            }
            
            string filter = string.Format("ref:{0}\"{1}\"", instanceId, AssetDatabase.GUIDToAssetPath(guid).Substring(7));
            
            var setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            
            try
            {
                setSearchType.Invoke(hierarchyWindow, new object[] { filter, 0, false});
            }
            catch
            {
                setSearchType.Invoke(hierarchyWindow, new object[] { filter, 0, false, false});
            }
        }

        private string CLZF2_Base64(string s)
        {
            return Convert.ToBase64String(CLZF2.Compress(Encoding.UTF8.GetBytes(s)));
        }

        private void GetIconCache(string message, object callback)
        {
            var json = APCache.GetIconCacheJSON();
            json = Convert.ToBase64String(CLZF2.Compress(Encoding.UTF8.GetBytes(json)));
            wrap = new CallbackWrapper(callback);
            wrap.Send(json);
        }

        public void RefreshIconCache()
        {
            if (webView != null)
            {
                webView.ExecuteJavascript("window.refreshIconCache()");
            }
        }

        private void PingAssets(string id, object callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                string message = string.Format("Asset not found: {0}", id);
                EditorUtility.DisplayDialog("404: Not found", message, "OK");
                return;
            }

            Utility.DebugLog(string.Format("Ping {0}", id));

            UnityEngine.Object obj;

            if (Utility.IsSubAsset(id))
            {
                Utility.DebugLog("Ping: Is SubAsset");
                obj = GetAnimationObjectFromModel(id);
            }
            else
            {
                Utility.DebugLog("Ping: Is Not SubAsset");
                obj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(id));
            }

            Utility.DebugLog(string.Format("Trying to ping {0}", obj));

            if (obj == null)
            {
                return;
            }

            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        private void MultiSelect(string paths, object callback)
        {
            if (string.IsNullOrEmpty(paths))
            {
                return;
            }

            var decompressedIds = Encoding.UTF8.GetString(CLZF2.Decompress(Convert.FromBase64String(paths)));

            Utility.DebugLog(string.Format("MultiSelect: {0}", decompressedIds));

            var assetIds = decompressedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var objects = new UnityEngine.Object[assetIds.Length];
            for (int i = 0; i < assetIds.Length; i++)
            {
                string id = assetIds[i];
                if (Utility.IsSubAsset(id))
                {
                    Utility.DebugLog("MultiSelect: Is SubAsset");
                    var obj = GetAnimationObjectFromModel(id);
                    if (obj != null)
                    {
                        objects[i] = obj;
                    }
                }
                else
                {
                    Utility.DebugLog("MultiSelect: Is Not SubAsset");
                    objects[i] = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(id));
                }
            }

            if (objects.Length > 0)
            {
                Selection.objects = objects;
                if (Event.current != null)
                {
                    Event.current.Use();
                };
            }
        }

        private UnityEngine.Object GetAnimationObjectFromModel(string assetid)
        {
            string guid = Utility.GetGuidFromAssetId(assetid);
            string fileId = Utility.GetFileIdFromAssetId(assetid);

            Utility.DebugLog(string.Format("Find Animation in {0} with id {1}", guid, fileId));

            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(fileId))
            {
                return null;
            }

            var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid));
            Utility.DebugLog(string.Format("Get {0} items in {1}", objects.Length, assetid));

            foreach (var obj in objects)
            {
                if (obj is AnimationClip && Utility.GetLocalIndentifierOfObject(obj).ToString() == fileId)
                {
                    objects = null;
                    return obj;
                }
            }

            return null;
        }

        private void GetEditorPerfsValue(string key, object callback)
        {
            var value = EditorPrefs.GetString(key);
            wrap = new CallbackWrapper(callback);
            wrap.Send(value);
        }

        private void SetEditorPerfsValue(string KeyValue, object callback)
        {
            var keyAndValue = KeyValue.Split(new char[] { '$' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (keyAndValue.Length == 2)
            {
                EditorPrefs.SetString(keyAndValue[0], keyAndValue[1]);
            }
        }

        private void DeleteAssets(string delIds, object callback)
        {
            if (string.IsNullOrEmpty(delIds))
            {
                return;
            }

            var ids = delIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (ids.Length == 0)
            {
                return;
            }

            string title = "Delete selected file?";
            string message = @"- 'Delete' will delete assets permanently.
- 'Backup And Delete' is slow but will backup assets to Application.persistentDataPath."; 
            string alt = "Backup And Delete";
            var seletedIndex = EditorUtility.DisplayDialogComplex(title, message, "Delete", "Cancel", alt);
            SyncManager.Enable = false;
            
            if (seletedIndex == 0)
            {
                EditorUtility.DisplayProgressBar("Deleting Assets", "Might be slow, be patient :)", 0f);
                for (int i = 0; i < ids.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Deleting Assets", "Might be slow, be patient :)", i * 1f / ids.Length);
                    var path = AssetDatabase.GUIDToAssetPath(ids[i]);

                    if (File.Exists(path))
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
            else if (seletedIndex == 2)
            {
                BackupAssets(ids);
            }

            SyncManager.Enable = true;
        }

        private void BackupAssets(string[] ids)
        {
            if (ids == null)
            {
                return;
            }

            List<string> backupfiles = new List<string>();
            for(int i = 0; i < ids.Length; i ++)
            {
                var path = AssetDatabase.GUIDToAssetPath(ids[i]);
                if (File.Exists(path))
                {
                    backupfiles.Add(path);
                }
            }
            
            string exportFileName = string.Format("AE_Backup_{0}.unitypackage", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
            var info = Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "AEBackup"));
            exportFileName = Path.Combine(info.FullName, exportFileName);
            EditorUtility.DisplayProgressBar("Creating backup package", "Might be slow, be patient :)", 0.2f);
            AssetDatabase.ExportPackage(backupfiles.ToArray(), exportFileName, ExportPackageOptions.Default);

            for (int i = 0; i < backupfiles.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Creating backup package", "Might be slow, be patient :)", 0.2f + (0.8f * i / backupfiles.Count));
                AssetDatabase.DeleteAsset(backupfiles[i]);
            }

            AssetDatabase.Refresh();
            QuickOpener.Reveal(info.FullName);
            EditorUtility.ClearProgressBar();
        }

        private void DeleteFromBlacklist(string paths, object callback)
        {
            var assetPaths = paths.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (assetPaths.Length == 0)
            {
                return;
            }

            foreach (string path in assetPaths)
            {
                APCache.RemoveFromBlacklist(path);
            }

            APCache.CommitBlacklistChange();
        }

        private void TriggerBuild(string args, object callback)
        {
            APlusWindow.FindUnusedAssets();
        }

        private void RenameAssets(string assets, object callback)
        {
            Utility.DebugLog(string.Format("Rename {0}", assets));

            if (string.IsNullOrEmpty(assets))
            {
                return;
            }

            string[] temp = assets.Split(new char[] { ']', '[' }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length != 3)
            {
                return;
            }

            var assetIds = temp[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var newNames = temp[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string title = "Renaming";
            float progress = 0;
            if (assetIds.Length == newNames.Length)
            {
                int changedCount = 0;
                int passCount = 0;
                Dictionary<string, List<long>> fileIdMaps = new Dictionary<string, List<long>>();
                Dictionary<string, List<string>> newNamesMap = new Dictionary<string, List<string>>();

                for (int i = 0; i < assetIds.Length; i++)
                {
                    if (Utility.IsSubAsset(assetIds[i]))
                    {
                        var guid = Utility.GetGuidFromAssetId(assetIds[i]);
                        var fileId = Utility.GetFileIdFromAssetId(assetIds[i]);
                        long fileId_long = -1;

                        if (long.TryParse(fileId, out fileId_long))
                        {
                            if (fileIdMaps.ContainsKey(guid))
                            {
                                fileIdMaps[guid].Add(fileId_long);
                                newNamesMap[guid].Add(newNames[i]);
                            }
                            else
                            {
                                fileIdMaps.Add(guid, new List<long>() { fileId_long });
                                newNamesMap.Add(guid, new List<string>() { newNames[i] });
                            }
                        }
                        else
                        {
                            passCount++;
                        }
                        continue;
                    }

                    var assetPath = AssetDatabase.GUIDToAssetPath(assetIds[i]);
                    AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                    if (importer.ToString().Contains("UnityEngine.NativeFormatImporter"))
                    {
                        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
                        serializedObject.targetObject.name = newNames[i];
                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    }

                    var errorMessage = AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(assetIds[i]), newNames[i]);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        changedCount++;
                    }
                    else
                    {
                        Debug.LogWarning(errorMessage);
                    }

                    progress = (changedCount + passCount) * 1.0f / assetIds.Length;
                    if (EditorUtility.DisplayCancelableProgressBar(title, assetIds[i], progress))
                    {
                        wrap = new CallbackWrapper(callback);
                        wrap.Send("done");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                }

                foreach (var keyVal in fileIdMaps)
                {
                    var modelPath = AssetDatabase.GUIDToAssetPath(keyVal.Key);
                    APResources.RenameAnimationClipInFbx(modelPath, keyVal.Value.ToArray(), newNamesMap[keyVal.Key].ToArray(), ref changedCount);
                    progress = (changedCount + keyVal.Value.Count + passCount) * 1.0f / assetIds.Length;
                    if (EditorUtility.DisplayCancelableProgressBar(title, keyVal.Key, progress))
                    {
                        wrap = new CallbackWrapper(callback);
                        wrap.Send("done");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                }

                AssetDatabase.Refresh();

                int errorCount = assetIds.Length - passCount - changedCount;
                string msg = string.Format("{0} Success, {1} Failed, {2} Passed", changedCount, errorCount, passCount);
                EditorUtility.DisplayDialog("Bulk Rename Result", msg, "OK");

                wrap = new CallbackWrapper(callback);
                wrap.Send("done");
                EditorUtility.ClearProgressBar();
            }
        }

        private void RefreshCache(string msg, object callback)
        {
            APlusWindow.RefreshCache();
        }

        private void GetAssetsInActiveScene(string msg, object callback)
        {
            var list = EditorUtility.CollectDependencies(GameObject.FindObjectsOfType(typeof(GameObject)));
            HashSet<string> assetsPath = new HashSet<string>();
            foreach (var item in list)
            {
                var path = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(path))
                {
                    assetsPath.Add(path);
                }
            }

            var queryString = "Id:(null)";

            var guids = assetsPath.Select(d => AssetDatabase.AssetPathToGUID(d)).ToArray();
            if (guids.Length > 0)
            {
                queryString = string.Format("Id:{0}", string.Join("|", guids));
            }

            SetCurrentURL(string.Format("res/{0}", msg), queryString);
        }

        private void OpenContainerFolder(string msg, object callback)
        {
            Utility.DebugLog(string.Format("OpenFolder Receive: {0}", msg));

            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            
            var decompressedMessage = Encoding.UTF8.GetString(CLZF2.Decompress(Convert.FromBase64String(msg)));
            var assets = decompressedMessage.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> folders = new HashSet<string>();
            foreach (var item in assets)
            {
                var guid = Utility.GetGuidFromAssetId(item);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var folder = Utility.GetContainerFolder(path);

                Utility.DebugLog(string.Format("OpenFolder: {0}", folder));

                if (!string.IsNullOrEmpty(folder))
                {
                    folders.Add(folder);
                }
            }

            foreach (var folder in folders)
            {
                EditorUtility.RevealInFinder(folder);
            }
        }

        private void CopyNameToClipboard(string msg, object callback)
        {
            CopyAssetPropertyToClipboard(msg, asset => asset.Name);
        }

        private void CopyPathToClipboard(string msg, object callback)
        {
            CopyAssetPropertyToClipboard(msg, asset => asset.Path);
        }

        private void CopyAssetPropertyToClipboard(string msg, Func<APAsset, string> selector)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            var decompressedMessage = Encoding.UTF8.GetString(CLZF2.Decompress(Convert.FromBase64String(msg)));
            var assets = decompressedMessage.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var names = new List<string>();
            foreach (var assetId in assets)
            {
                var asset = APCache.GetValue(assetId);
                if (asset != null)
                {
                    names.Add(selector(asset));
                }
            }

            TextEditor textEditor = new TextEditor();
            textEditor.ReplaceSelection(string.Join("\n", names.ToArray()));
            textEditor.OnFocus();
            textEditor.Copy();
        }

        private void StarAndReview(string message, object callback)
        {
            UnityEditorInternal.AssetStore.Open("content/57335");
        }
    }
}
#endif