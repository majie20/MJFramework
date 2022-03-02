//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APlus
{
    [SerializableAttribute]
    public class APlusWindow : EditorWindow, IHasCustomMenu
    {
        static PropertyInfo getDocked;
        static APlusWindow()
        {
            getDocked = typeof(EditorWindow).GetProperty("docked", ReflectionUtils.BIND_FLAGS);
        }

        public static bool GetDocked(EditorWindow window)
        {
            var obj = getDocked.GetValue(window, null);
            return bool.Parse(obj.ToString());
        }

        public bool InDockableWindowStyle;
        private const float titleHeight = 0;
        public Webview webview;
        private const string EditorHTMLFile = "index.html";
        private const string TITLE = "A+ Assets Explorer";

        public static APlusWindow Instance = null;

#if APLUS_DEV
        private static bool useServerMode = false;
#endif

        [MenuItem("Tools/A+ Assets Explorer/Assets Explorer %#w", false, 11)]
        [MenuItem("Assets/A+ Assets Explorer", false, 531)]
        public static void LoadWindow()
        {
            if (Instance != null)
            {
                Instance.Show();
                Instance.Focus();
                return;
            }

            ClearPrevoiusURL();

            if (!PreferencesItems.IsDockableWindowStyle)
            {
                Instance = GetWindow<APlusWindow>(true, TITLE, true);
                Instance.InitWebView(GUIClip.Unclip(new Rect(0, 0, Instance.position.x, Instance.position.y)));
            }
            else
            {
                Type[] desiredDockNextTo = new Type[] { typeof(SceneView) };
                Instance = EditorWindow.GetWindow<APlusWindow>(TITLE, desiredDockNextTo);
                Instance.SetMinMaxSizes();
                Instance.Show();
            }

            Instance.InDockableWindowStyle = PreferencesItems.IsDockableWindowStyle;
            Instance.Show();
            Instance.Focus();
        }

        [MenuItem("Assets/Show In Assets Explorer", true)]
        public static bool ShowInAPlusEnable()
        {
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                return false;
            }

            if (Instance == null)
            {
                return false;
            }

            HashSet<string> types = new HashSet<string>();
            foreach (var item in Selection.objects)
            {
                if (item == null)
                {
                    continue;
                }

                types.Add(APResources.GetAssetTypeByObject(item));
            }

            return types.Count() == 1 && !string.IsNullOrEmpty(types.ElementAt(0));
        }

        [MenuItem("Assets/Show In Assets Explorer", false, 534)]
        public static void ShowInAPlus()
        {
            HashSet<string> types = new HashSet<string>();
            var ids = Selection.objects.Select(o =>
            {
                var path = AssetDatabase.GetAssetPath(o);
                var assetid = AssetDatabase.AssetPathToGUID(path);
                if (o is AnimationClip)
                {
                    if (!path.EndsWith(".anim"))
                    {
                        assetid = Utility.GetAssetId(assetid, Utility.GetLocalIndentifierOfObject(o).ToString());
                    }
                }

                types.Add(APResources.GetAssetTypeByObject(o));
                return assetid;
            }).ToArray();

            if (types.Count > 1)
            {
                string message = "This actions does not support multiple assets type. Please use 'Select in Selection' in Assets menu to select one type.";
                EditorUtility.DisplayDialog("Not Support Action", message, "OK");
                return;
            }

            if (types.Count == 1)
            {
                var assetType = types.ElementAt(0);
                if (string.IsNullOrEmpty(assetType))
                {
                    string message = "The assets selected are not supported in currect version.";
                    EditorUtility.DisplayDialog("Not Support Assets", message, "OK");
                    return;
                }

                AssetNotification.webCommunicationService.SetCurrentURL(string.Format("res/{0}", types.ElementAt(0)), string.Format("Id:{0}", string.Join("|", ids)));
            }
        }

        [MenuItem("Assets/Select Assets in Selection/Animation", false, 533)]
        public static void SelectAnimationsInSelection()
        {
            SelectAssetTypeInSelection(AssetType.ANIMATIONS);
        }

        [MenuItem("Assets/Select Assets in Selection/Audio", false, 533)]
        public static void SelectAudioInSelections()
        {
            SelectAssetTypeInSelection(AssetType.AUDIOS);
        }

        [MenuItem("Assets/Select Assets in Selection/Code", false, 533)]
        public static void SelectCodeInSelection()
        {
            SelectAssetTypeInSelection(AssetType.CODE);
        }

        [MenuItem("Assets/Select Assets in Selection/Font", false, 533)]
        public static void SelectFontsInSelection()
        {
            SelectAssetTypeInSelection(AssetType.FONTS);
        }

        [MenuItem("Assets/Select Assets in Selection/Movie", false, 533)]
        public static void SelectMoviesInSelection()
        {
            SelectAssetTypeInSelection(AssetType.MOVIES);
        }

        [MenuItem("Assets/Select Assets in Selection/Model", false, 533)]
        public static void SelectModelsInSelection()
        {
            SelectAssetTypeInSelection(AssetType.MODELS);
        }

        [MenuItem("Assets/Select Assets in Selection/Material", false, 533)]
        public static void SelectMaterialInSelctions()
        {
            SelectAssetTypeInSelection(AssetType.MATERIALS);
        }

        [MenuItem("Assets/Select Assets in Selection/Prefab", false, 533)]
        public static void SelectPerfabsInSelections()
        {
            SelectAssetTypeInSelection(AssetType.PREFABS);
        }

        [MenuItem("Assets/Select Assets in Selection/Shader", false, 533)]
        public static void SelectShaderInSelection()
        {
            SelectAssetTypeInSelection(AssetType.SHADERS);
        }

        [MenuItem("Assets/Select Assets in Selection/Texture", false, 533)]
        public static void SelectTextuersInSelection()
        {
            SelectAssetTypeInSelection(AssetType.TEXTURES);
        }

        private static void SelectAssetTypeInSelection(string assetType)
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            foreach (var obj in Selection.objects)
            {
                if (assetType.Equals(APResources.GetAssetTypeByObject(obj), StringComparison.CurrentCultureIgnoreCase))
                {
                    objects.Add(obj);
                }
            }

            Selection.objects = objects.ToArray();
            if (Event.current != null)
            {
                Event.current.Use();
            }
        }

        [MenuItem("Assets/A+ Assets Blacklist/Add", false, 532)]
        public static void AddToBlacklist()
        {
            List<APAsset> newBlacklist = new List<APAsset>();
            foreach (var item in Selection.objects)
            {
                var path = GetPathOfSelectedAssets(item);
                if (!APCache.ExistsInBlacklist(path))
                {
                    var asset = APResources.GetBlackListAPAsset(path);
                    APCache.AddToBlacklist(path, asset);
                    newBlacklist.Add(asset);
                }
            }

            AssetNotification.webCommunicationService.AddAssets(newBlacklist);
            APCache.CommitBlacklistChange();
        }

        [MenuItem("Assets/A+ Assets Blacklist/Remove", false, 532)]
        public static void RemoveBlacklist()
        {
            List<APAsset> deleteAssets = new List<APAsset>();
            foreach (var item in Selection.objects)
            {
                var path = GetPathOfSelectedAssets(item);
                if (APCache.ExistsInBlacklist(path))
                {
                    var asset = APResources.GetBlackListAPAsset(path);
                    APCache.RemoveFromBlacklist(path);
                    Utility.UpdateJsonInAsset(asset);
                    deleteAssets.Add(asset);
                }
            }

            AssetNotification.webCommunicationService.DeleteAssets(deleteAssets.ToArray().ToList()); ;
            APCache.CommitBlacklistChange();
        }

        private static string GetPathOfSelectedAssets(UnityEngine.Object asset)
        {
#if UNITY_2018
            var path = string.Empty;
            // NULL means the Packages
            if (asset == null)
            {
                path = "Packages/";
            }
            else
            {
                path = AssetDatabase.GetAssetPath(asset);
            }
#else
                var path = AssetDatabase.GetAssetPath(asset);
#endif
            return path;
        }

        [MenuItem("Assets/Find References In Project", false, 535)]
        public static void FindReferences()
        {
            HashSet<string> references = new HashSet<string>();
            var objects = Selection.objects;
            var scriptsMap = APResources.GetMonoScriptsMap();

            bool Cancel = false;
            for (int i = 0; i < objects.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(objects[i]);
                string message = "Find references of " + path;
                var referencesAssets = APCache.GetReferences(path, message, i * 1f / objects.Length, (i + 1) * 1f / objects.Length, ref Cancel);

                foreach (var item in referencesAssets)
                {
                    if (!item.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                    {
                        references.Add(item);
                    }
                }

                if (objects[i] is ScriptableObject || objects[i] is MonoScript)
                {
                    Type msType = null;
                    if (objects[i] is ScriptableObject)
                    {
                        msType = (objects[i] as ScriptableObject).GetType();
                    }

                    if (objects[i] is MonoScript)
                    {
                        msType = (objects[i] as MonoScript).GetClass();
                    }

                    if (msType != null)
                    {
                        msType
                            .GetFields(ReflectionUtils.BIND_FLAGS)
                            .ToList()
                            .ForEach(f =>
                            {
                                if (scriptsMap.ContainsKey(f.FieldType.ToString()))
                                {
                                    references.Add(scriptsMap[f.FieldType.ToString()]);
                                }
                            });
                    }
                }

                if (Cancel)
                {
                    break;
                }

            }

            if (!Cancel)
            {
                SelectReferences(references.ToList());
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Assets/Find References By Type/By Model", false, 536)]
        public static void FindReferencesByModels()
        {
            SelectReferencesByType(AssetType.MODELS);
        }

        [MenuItem("Assets/Find References By Type/By Material", false, 536)]
        public static void FindReferencesByMaterials()
        {
            SelectReferencesByType(AssetType.MATERIALS);
        }

        [MenuItem("Assets/Find References By Type/By Prefab", false, 536)]
        public static void FindReferencesByPrefabs()
        {
            SelectReferencesByType(AssetType.PREFABS);
        }

        [MenuItem("Assets/Find References By Type/By Scene", false, 536)]
        public static void FindReferencesByScenes()
        {
            SelectReferencesByType(AssetType.SCENE);
        }

        private static void SelectReferencesByType(string assetType)
        {
            HashSet<string> references = new HashSet<string>();
            var objects = Selection.objects;
            bool Cancel = false;

            for (int i = 0; i < objects.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(objects[i]);
                string message = "Find references of " + path;
                float startProgress = i * 1f / objects.Length;
                float endProgress = (i + 1) * 1f / objects.Length;

                List<string> referencesAssets = new List<string>();
                switch (assetType)
                {
                    case AssetType.MODELS:
                        referencesAssets = APCache.GetReferencesByType<APModel>(path, APAssetType.Model, message, startProgress, endProgress, ref Cancel);
                        break;
                    case AssetType.MATERIALS:
                        referencesAssets = APCache.GetReferencesByType<APMaterial>(path, APAssetType.Material, message, startProgress, endProgress, ref Cancel);
                        break;
                    case AssetType.PREFABS:
                        referencesAssets = APCache.GetReferencesByType<APPrefab>(path, APAssetType.Prefab, message, startProgress, endProgress, ref Cancel);
                        break;
                    case AssetType.SCENE:
                        referencesAssets = APCache.GetReferencesInScene(path, ref Cancel, true, message, startProgress, endProgress);
                        break;
                    default:
                        break;
                }

                foreach (var item in referencesAssets)
                {
                    if (!item.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                    {
                        references.Add(item);
                    }
                }

                if (Cancel)
                {
                    break;
                }

            }

            if (!Cancel)
            {
                SelectReferences(references.ToList());
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void SelectReferences(List<string> references)
        {
            List<UnityEngine.Object> referencesObjects = new List<UnityEngine.Object>();
            foreach (var item in references)
            {
                referencesObjects.Add(AssetDatabase.LoadAssetAtPath(item, typeof(UnityEngine.Object)));
            }

            if (references.Count == 0)
            {
                EditorUtility.DisplayDialog("No References Found!", "The assets have no references in Project!", "OK");
                EditorUtility.ClearProgressBar();
            }
            else
            {
                Selection.objects = referencesObjects.ToArray();
                EditorUtility.ClearProgressBar();
                if (Event.current != null)
                {
                    Event.current.Use();
                }
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
        }

        private void Reload()
        {
            if (webview != null)
            {
                webview.Reload();
            }
        }

        [MenuItem("Tools/A+ Assets Explorer/Find unused assets", false, 22)]
        public static void FindUnusedAssets()
        {
            string title = "Find unused files?";
            string message = "Press 'OK' to launch a build setting dialog to start a build.\r\n\r\n";
            if (EditorUtility.DisplayDialog(title, message, "OK", "Cancel"))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            APCache.SaveToLocal();
            EditorPrefs.SetString(APCache.LOAD_FROM_LOCAL_KEY, APCache.LOAD_FROM_LOCAL_KEY);
        }

        [MenuItem("Tools/A+ Assets Explorer/Refresh cache", false, 22)]
        public static void RefreshCache()
        {
            EditorPrefs.DeleteKey(APCache.LOAD_FROM_LOCAL_KEY);
            APCache.ReloadCache(() =>
            {
                AssetNotification.webCommunicationService.RefreshAll();
                AssetNotification.webCommunicationService.Refresh();
            });
        }

#if APLUS_DEV
        [MenuItem("Tools/A+ Assets Explorer/Toggle Server mode", false, 1024)]
        public static bool ToggleServerMode()
        {
            useServerMode = !useServerMode;
            Menu.SetChecked("Tools/A+ Assets Explorer/Toggle Server mode", useServerMode);
            return true;
        }
#endif

        public void InitWebView(Rect webviewRect)
        {
            if (webview == null)
            {
                this.webview = ScriptableObject.CreateInstance<Webview>();
                this.webview.hideFlags = HideFlags.HideAndDontSave;
            }

            this.webview.InitWebView(Webview.GetView(this), webviewRect, false);
            AssetNotification.webCommunicationService.Init(this.webview);
            docked = GetDocked(this);
            SetMinMaxSizes();
            LoadEditor();
            SetFocus(true);
            Instance = this;
        }

        #region Load Editor

        private const string ASSETEXPLORER_PREVOIUS_URL = "A+ASSETEXPLORER_PREVOIUS_URL";
        void LoadEditor()
        {
            string path = GetIndexHTMLPath();
#if APLUS_DEV
            if (useServerMode)
            {
                path = "http://127.0.0.1:8080";
            }
#endif

            if (EditorPrefs.HasKey(ASSETEXPLORER_PREVOIUS_URL))
            {
                path = EditorPrefs.GetString(ASSETEXPLORER_PREVOIUS_URL);
            }

            LoadURL(path);
        }

        private string GetIndexHTMLPath()
        {
            var guids = AssetDatabase.FindAssets("index");
            string checkMark = "bb14e2ed-a70f-4bca-95f7-d4463e335936";
            string htmlPath = string.Format(@"file://{0}/APlus Assets Explorer/Window/{1}", Application.dataPath, EditorHTMLFile); ;
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.ToLower().EndsWith("index.html"))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    if (obj != null && !string.IsNullOrEmpty(obj.text) && obj.text.Contains(checkMark))
                    {
                        htmlPath = string.Format("file://{0}", System.IO.Path.Combine(Application.dataPath, path.Substring("Assets/".Length)));
                    }
                }
            }

            Utility.DebugLog(string.Format("Loading from {0}", htmlPath));
            return htmlPath;
        }

        public static void ClearPrevoiusURL()
        {
            EditorPrefs.DeleteKey(ASSETEXPLORER_PREVOIUS_URL);
        }

        public void LoadURL(string path)
        {
#if APLUS_DEV
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
            this.webview.AllowRightClickMenu(true);
#endif
            if (useServerMode)
            {
                this.webview.LoadURL(path);
            }
            else
            {
                this.webview.LoadURL(path);
            }
#else
            this.webview.LoadURL(path);
#endif
        }

        #endregion

        #region Dockable Window Style

        public void OnAddedAsTab()
        {
            SetFocus(true);
        }

        public void OnBeforeRemovedAsTab()
        {
            OnBecameInvisible();
        }

        public void OnBecameInvisible()
        {
            if (!this.InDockableWindowStyle)
            {
                return;
            }

            if (this.webview != null)
            {
                this.webview.SetHostView(null);
                this.webview.Hide();
                this.webview.SetFocus(false);
            }
        }

        private int repeatedShow;
        private bool syncingFocus;
        private void SetFocus(bool value)
        {
            if (!this.syncingFocus)
            {
                this.syncingFocus = true;
                if (this.webview != null)
                {
                    if (value)
                    {
                        this.webview.SetHostView(Webview.GetView(this));
                        this.webview.Show();
                        this.repeatedShow = 5;
                    }

                    this.webview.SetFocus(value);
                }
                this.syncingFocus = false;
            }
        }

        public void OnEnable()
        {
            // re-binding the webview Instance
            //
            Rebinding();
            SetMinMaxSizes();
        }

        private void Rebinding()
        {
            if (Instance == null)
            {
                Instance = this;
                if (AssetNotification.webCommunicationService != null)
                {
                    AssetNotification.webCommunicationService.Init(this.webview);
                }
            }
        }

        public void OnLostFocus()
        {
            this.SetFocus(false);
        }

        public void OnFocus()
        {
            SetFocus(true);
            Rebinding();
        }

        bool docked = false;

        private void SetMinMaxSizes()
        {
            if (InDockableWindowStyle)
            {
                base.minSize = new Vector2(860f, 300f);
                base.maxSize = new Vector2(2048f, 2048f);
            }
            else
            {
                base.minSize = new Vector2(1024, 300);
            }
        }

        void TipUI(float windowWith, float windowHeight)
        {
            string text = "Assets Explorer is Loading...";
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.background = null;
            textStyle.fontSize = 18;
            float width = 280;
            float height = 36;
            float x = (windowWith - width) / 2;
            float y = windowHeight / 2 - height;
            GUI.Label(new Rect(x, y, width, height), text, textStyle);
        }

        void OnGUI()
        {
            if (docked != GetDocked(this))
            {
                SetMinMaxSizes();
            }

            Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0, base.position.width, base.position.height));

            TipUI(webViewRect.width, webViewRect.height);

            if (this.webview == null)
            {
                this.InitWebView(webViewRect);
            }

            if (Instance == null)
            {
                Rebinding();
            }

            if (this.repeatedShow-- > 0)
            {
                this.Refresh();
            }

            if (Event.current.type == EventType.Repaint && webview != null)
            {
                this.webview.SetHostView(Webview.GetView(this));
                this.webview.SetSizeAndPosition(webViewRect);
            }

            CheckRefresh();
        }

        public void CheckRefresh()
        {
            if (System.IO.File.Exists(AssetNotification.PrepareOnLoad.AFTERBUILD_A_PLUS)
                && webview != null)
            {
                AssetNotification.webCommunicationService.RefreshAll();
                AssetNotification.webCommunicationService.Refresh();
                System.IO.File.Delete(AssetNotification.PrepareOnLoad.AFTERBUILD_A_PLUS);
            }
        }

        public void Refresh()
        {
            this.webview.Hide();
            this.webview.Show();
        }

        public void Destroy()
        {
            if (webview != null)
            {
                webview.OnDestory();
            }

            Instance = null;
        }

        void OnDestroy()
        {
            Destroy();
        }

        #endregion

        [PreferenceItem("Assets Explorer")]
        private static void PreferencesItem()
        {
            PreferencesItems.PreferencesItemUI();
            if (PreferencesItems.ThemeChanged && AssetNotification.webCommunicationService != null)
            {
                AssetNotification.webCommunicationService.UpdateTheme();
                PreferencesItems.ThemeChanged = false;
            }
        }
    }
}
#endif