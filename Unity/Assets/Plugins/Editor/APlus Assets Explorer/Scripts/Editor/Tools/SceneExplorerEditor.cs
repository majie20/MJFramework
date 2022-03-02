//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace SceneExplorer
{
    [SerializableAttribute]
    public class SceneExplorerEditor : EditorWindow, IHasCustomMenu
    {
        private const string WINDOW_TITLE = "Build Manager";
        private const string DATA_FILE_PATH = "ProjectSettings/ScenesExplorer.asset";
        private const string BUILD_SETTING_FILE_EXTENSION = "unitybuildsettings";

        [MenuItem("Tools/A+ Assets Explorer/Build Manager", false, 44)]
        public static void OpenWindow()
        {
            GetWindow<SceneExplorerEditor>(WINDOW_TITLE);
        }

        [MenuItem("Edit/Build Manager/Active in BuildSettings &a", false, 11)]
        public static void ActiveScene()
        {
            if (Instacne == null)
            {
                return;
            }

            Instacne.SetSceneActive(true);
        }

        [MenuItem("Edit/Build Manager/Deactive in BuildSettings &d", false, 11)]
        public static void DeactiveScene()
        {
            if (Instacne == null)
            {
                return;
            }

            Instacne.SetSceneActive(false);
        }

        [MenuItem("Edit/Build Manager/Ping in Project %l", false, 22)]
        public static void Ping()
        {
            if (Instacne == null)
            {
                return;
            }

            Instacne.PingSelectedScene();
        }

        [MenuItem("Edit/Build Manager/Delete Scene %.", false, 22)]
        public static void DeleteScene()
        {
            if (Instacne == null)
            {
                return;
            }

            Instacne.DeleteSelectedScene();
        }

        [MenuItem("Edit/Build Manager/Open Container Folder", false, 22)]
        public static void OpenFolder()
        {
            if (Instacne == null)
            {
                return;
            }

            Instacne.RevealFolder();
        }

        public static SceneExplorerEditor Instacne;
        public static List<SceneData> sceneList;
        public ListViewState listView;
        public bool isCheckAll;
        public ListViewActionOptions listViewActionOptions = ListViewActionOptions.Reordering;

        public bool isInAdvanceLayout = false;
        public bool isInBasicLayout = true;
        private bool isFocus;
        protected static Styles styles;
        private bool[] selectedItems;
        private int initialSelectedItem = -1;

        static SceneExplorerEditor()
        {
            InitSceneList();
        }

        protected class Styles
        {
            public readonly GUIStyle listItem = new GUIStyle("PR Label");
            public readonly GUIStyle background = new GUIStyle();

            public Styles()
            {
                listItem.richText = true;
                Texture2D background = this.listItem.onFocused.background;
                this.listItem.onActive.background = background;
                this.listItem.onFocused.background = background;
            }
        }

        private void DrawToobarButton(string text, float width, Action action)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), EditorStyles.toolbarButton);
            rect.width = width;
            if (GUI.Button(rect, text, EditorStyles.toolbarButton))
            {
                if (action != null)
                {
                    action.Invoke();
                }
            }
        }

        private void DrawCheckAllCheckBox()
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent("Checked"), EditorStyles.toolbarButton);
            rect.width = 36;
            EditorGUI.BeginChangeCheck();
            isCheckAll = GUI.Toggle(rect, isCheckAll, "", EditorStyles.toggle);
            if (EditorGUI.EndChangeCheck())
            {
                sceneList.ForEach(data => data.Active = isCheckAll);
                SaveDataToLocal(DATA_FILE_PATH);
            }
        }

        public SceneExplorerEditor()
        {
            listView = new ListViewState();
            InitSceneList();
        }

        public static void InitSceneList()
        {
            sceneList = new List<SceneData>();
        }

        private static List<SceneData> GetCurrentSceneList()
        {
            List<SceneData> scenes = new List<SceneData>();
            var sceneGuids = AssetDatabase.FindAssets("t:scene");
            foreach (var guid in sceneGuids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                scenes.Add(Utility.GetSceneDataFromPath(scenePath));
            }

            return scenes;
        }

        private void TopToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);

            if (isInAdvanceLayout)
            {
                DrawCheckAllCheckBox();
            }

            GUILayout.FlexibleSpace();
            DrawToobarButton("Sync to Build Settings", 120, OpenBuildSettingsWithCurrentSceneSettings);

            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            if (styles == null)
            {
                styles = new Styles();
            }

            if (isInBasicLayout)
            {
                RenderBasicLayout();
                return;
            }

            if (isInAdvanceLayout)
            {
                RenderAdvancedLayout();
                return;
            }
        }

        private void InitSelectedItemIfNeeded()
        {
            if (selectedItems == null || selectedItems.Length != sceneList.Count)
            {
                selectedItems = new bool[sceneList.Count];
            }
        }

        private void HandleMultiSelection(ListViewElement element, int prevSelected)
        {
            // Need this line otherwise the Reordering will break;
            //
            if (Event.current.type == EventType.MouseDown
                && element.position.Contains(Event.current.mousePosition)
                && (Event.current.shift || EditorGUI.actionKey))
            {
                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseDown
                && element.position.Contains(Event.current.mousePosition)
                && !Event.current.shift && !EditorGUI.actionKey)
            {
                for (int i = 0; i < selectedItems.Length; i++)
                {
                    selectedItems[i] = false;
                }

                selectedItems[element.row] = true;
            }

            if (ListViewGUILayout.HasMouseUp(element.position))
            {
                ListViewGUILayout.MultiSelection(prevSelected, element.row, ref initialSelectedItem, ref selectedItems);
            }
        }

        private void RenderBasicLayout()
        {
            listView.totalRows = sceneList.Count;
            EditorGUILayout.BeginVertical();

            TopToolbar();
            GUILayout.Space(4);

            InitSelectedItemIfNeeded();
            int row = listView.row;
            foreach (ListViewElement element in ListViewGUILayout.ListView(listView, listViewActionOptions, styles.background))
            {
                if (IsDoubleClickOnRow(element))
                {
                    var data = GetRowData(element);
                    Utility.LoadScene(data.Path);
                }

                if (IsRightClick(element))
                {
                    DisplayEditorContextMenu(element);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    DrawListViewElement(element, selectedItems[element.row]);
                }

                HandleMultiSelection(element, row);
            }

            EditorGUILayout.EndVertical();
            UpdateReOrdering();
        }

        private void RenderAdvancedLayout()
        {
            listView.totalRows = sceneList.Count;
            EditorGUILayout.BeginVertical();

            TopToolbar();
            GUILayout.Space(4);

            int[] cloumnsWidth = new int[2];
            int firstClomnsWith = 21;
            cloumnsWidth[0] = firstClomnsWith;
            cloumnsWidth[1] = (int)this.position.width - firstClomnsWith;

            InitSelectedItemIfNeeded();
            int row = listView.row;

            foreach (ListViewElement element in ListViewGUILayout.ListView(listView, listViewActionOptions, cloumnsWidth, string.Empty, styles.background))
            {
                if (IsDoubleClickOnRow(element))
                {
                    var data = GetRowData(element);
                    Utility.LoadScene(data.Path);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    DrawListViewElement(element, selectedItems[element.row]);
                }

                if (element.column == 0)
                {
                    DrawRowCheckBox(element);
                }

                if (IsRightClick(element))
                {
                    DisplayEditorContextMenu(element);
                }

                HandleMultiSelection(element, row);
            }

            EditorGUILayout.EndVertical();
            UpdateReOrdering();
        }

        public void DeleteSelectedScene()
        {
            InitSelectedItemIfNeeded();
            bool hasChange = false;

            if (EditorUtility.DisplayDialog("Delete selected asset?", "You cannot undo this action!", "OK", "Cancel"))
            {
                for (int i = 0; i < selectedItems.Length; i++)
                {
                    if (selectedItems[i])
                    {
                        if (!File.Exists(sceneList[i].Path))
                        {
                            sceneList.RemoveAt(i);
                        }
                        else
                        {
                            AssetDatabase.DeleteAsset(sceneList[i].Path);
                        }

                        hasChange = true;
                    }
                }
            }

            if (hasChange)
            {
                SaveDataToLocal(DATA_FILE_PATH);
            }

        }

        public void PingSelectedScene()
        {
            InitSelectedItemIfNeeded();

            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();

            for (int i = 0; i < selectedItems.Length; i++)
            {
                var selected = selectedItems[i];
                if (selected)
                {
                    var sceneObject = AssetDatabase.LoadAssetAtPath(sceneList[i].Path, typeof(UnityEngine.Object));
                    if (sceneObject)
                    {
                        objects.Add(sceneObject);
                    }
                }
            }

            if (sceneList.Count > 0)
            {
                Selection.objects = objects.ToArray();
            }
        }

        public void RevealFolder()
        {
            InitSelectedItemIfNeeded();

            for (int i = 0; i < selectedItems.Length; i++)
            {
                if (selectedItems[i])
                {
                    EditorUtility.RevealInFinder(sceneList[i].Path);
                }
            }
        }

        public void SetSceneActive(bool active)
        {
            InitSelectedItemIfNeeded();
            bool hasChange = false;

            for (int i = 0; i < selectedItems.Length; i++)
            {
                if (selectedItems[i])
                {
                    hasChange = true;
                    sceneList[i].Active = active;
                }
            }

            if (hasChange)
            {
                SaveDataToLocal(DATA_FILE_PATH);
            }

            this.Repaint();
        }

        private void DisplayEditorContextMenu(ListViewElement element)
        {
            Rect rect = element.position;
            rect.width = 168;
            rect.x += 80;

            EditorUtility.DisplayPopupMenu(rect, "Edit/Build Manager", null);
            listView.row = element.row;
        }

        private void UpdateReOrdering()
        {
            if (listView.totalRows > 0 && listView.selectionChanged)
            {
                if (listView.draggedFrom != -1
                    && listView.draggedTo != -1
                    && listView.draggedFrom != listView.draggedTo)
                {
                    var temp = sceneList[listView.draggedFrom];
                    var temp_select = selectedItems[listView.draggedFrom];

                    if (listView.draggedFrom > listView.draggedTo)
                    {
                        for (int i = listView.draggedFrom; i > listView.draggedTo; i--)
                        {
                            sceneList[i] = sceneList[i - 1];
                            selectedItems[i] = selectedItems[i - 1];
                        }

                        sceneList[listView.draggedTo] = temp;
                        selectedItems[listView.draggedTo] = temp_select;
                    }
                    else
                    {
                        for (int i = listView.draggedFrom; i < listView.draggedTo - 1; i++)
                        {
                            sceneList[i] = sceneList[i + 1];
                            selectedItems[i] = selectedItems[i + 1];
                        }

                        sceneList[listView.draggedTo - 1] = temp;
                        selectedItems[listView.draggedTo - 1] = temp_select;
                    }
                }
            }
        }

        private void DrawListViewElement(ListViewElement element, bool selected)
        {
            // background
            //
            GUIStyle style = styles.background;

            Rect rect = new Rect(element.position.x - 2,
                                 element.position.y,
                                 element.position.width + 2,
                                 element.position.height);

            style.Draw(rect, false, false, listView.row == element.row, false);

            var data = GetRowData(element);

            var shouldDisabled = (!data.Active || (!File.Exists(data.Path) && !selected));
            EditorGUI.BeginDisabledGroup(shouldDisabled);
            styles.listItem.Draw(rect,
                                GetRowContent(element),
                                false,
                                false,
                                selected,
                                isFocus);

            EditorGUI.EndDisabledGroup();
        }

        private void DrawRowCheckBox(ListViewElement element)
        {
            var data = GetRowData(element);
            Rect rect = new Rect(element.position.x + 6,
                                 element.position.y,
                                 element.position.height,
                                 element.position.height);

            EditorGUI.BeginChangeCheck();
            data.Active = GUI.Toggle(rect, data.Active, string.Empty);
            if (EditorGUI.EndChangeCheck())
            {
                SaveDataToLocal(DATA_FILE_PATH);
            }
        }

        private SceneData GetRowData(ListViewElement element)
        {
            return sceneList[element.row];
        }

        private GUIContent GetRowContent(ListViewElement element)
        {
            if (element.column == 0 && isInAdvanceLayout)
            {
                return new GUIContent("");
            }

            var data = sceneList[element.row];

            var currentSceneColor = "#ff0000ff";
            var text = IsCurrentScene(element) ? string.Format("<color={0}>{1}</color>", currentSceneColor, data.Name) : data.Name;

            GUIContent content = new GUIContent();
            content.text = text;
            content.tooltip = data.Path;
            content.image = AssetDatabase.GetCachedIcon(data.Path);

            return content;
        }

        private bool IsCurrentScene(ListViewElement element)
        {
#if UNITY_5_3_OR_NEWER
            Scene scene = SceneManager.GetActiveScene();
            if(string.IsNullOrEmpty(scene.path)) 
            {
                return false;
            }

            var scenePath = scene.path;
#else
            var scenePath = EditorApplication.currentScene;
#endif

            var elementPath = sceneList[element.row].Path;

            if (string.IsNullOrEmpty(elementPath) || string.IsNullOrEmpty(scenePath))
            {
                return false;
            }

            if (scenePath.Equals(elementPath, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        void OnEnable()
        {
            if (Instacne == null)
            {
                Instacne = this;
            }

            LoadDataFromLocal(DATA_FILE_PATH);
            this.minSize = new Vector2(180, 100);
            InitSelectedItemIfNeeded();
        }

        void OnDisable()
        {
            SaveDataToLocal(DATA_FILE_PATH);
        }

        void OnFocus()
        {
            isFocus = true;
            Instacne = this;
        }

        void OnLostFocus()
        {
            Instacne = null;
            isFocus = false;
        }

        private static void SaveDataToLocal(string filePath)
        {
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            StringBuilder sb = new StringBuilder();
            sceneList.ForEach(s => sb.AppendLine(s.ToString()));
            File.WriteAllText(filePath, sb.ToString());
            AssetDatabase.SaveAssets();
        }

        private static void LoadDataFromLocal(string filePath)
        {
            if (sceneList == null)
            {
                InitSceneList();
            }

            if (!File.Exists(filePath))
            {
                InitDataWithEditorBuildSetting();
                SaveDataToLocal(filePath);
                return;
            }

            sceneList.Clear();
            var dataItems = File.ReadAllLines(filePath);
            foreach (var item in dataItems)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                string[] temp = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length != 3)
                {
                    continue;
                }

                SceneData data = new SceneData();
                data.Name = Utility.FormatName(temp[0]);
                data.Active = temp[1].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                data.Path = temp[2];
                sceneList.Add(data);
            }

            UpdateStoreDataWithCurrentScenesInProject();
        }

        private static void InitDataWithEditorBuildSetting()
        {
            var currentScenes = GetCurrentSceneList();
            var scenesInBuildSettings = EditorBuildSettings.scenes;
            foreach (var item in currentScenes)
            {
                var sceneInBuildSetting = scenesInBuildSettings.FirstOrDefault(s => s.path.Equals(item.Path, StringComparison.CurrentCultureIgnoreCase));
                if (sceneInBuildSetting != null)
                {
                    item.Active = sceneInBuildSetting.enabled;
                }
                else
                {
                    item.Active = false;
                }
            }

            sceneList = currentScenes;
        }

        private static void UpdateStoreDataWithCurrentScenesInProject()
        {
            var currentScenesInProject = GetCurrentSceneList();
            bool haveSceneNotInScene = false;

            foreach (var scene in currentScenesInProject)
            {
                if (sceneList.Any(s => s.Path.Equals(scene.Path, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                sceneList.Add(scene);
                haveSceneNotInScene = true;
            }

            if (haveSceneNotInScene)
            {
                SaveDataToLocal(DATA_FILE_PATH);
            }
        }

        private bool IsDoubleClickOnRow(ListViewElement element)
        {
            var rect = element.position;

            if (isInAdvanceLayout)
            {
                rect.x += 20;
            }

            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && rect.Contains(Event.current.mousePosition)
                && Event.current.clickCount == 2)
            {
                Event.current.Use();
                return true;
            }

            return false;
        }

        private bool IsRightClick(ListViewElement element)
        {
            if (Event.current.type == EventType.MouseDown
                    && Event.current.button == 1
                    && element.position.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return true;
            }
            return false;

        }

        private void OpenBuildSettingsWithCurrentSceneSettings()
        {
            string title = "Sync to Editor Build Settings?";
            string message = "This action will rewrite Editor Build Settings.";
            if (EditorUtility.DisplayDialog(title, message, "OK", "Cancel"))
            {
                UpdateBuildSettingsWithCurrentSceneSettings();
                Utility.OpenBuildSettings();
            }
        }

        public void UpdateBuildSettingsWithCurrentSceneSettings()
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            List<SceneData> notInBuildSettingsScenes = new List<SceneData>();
            foreach (var scene in sceneList)
            {
                bool inEditorBuildSettings = false;
                foreach (var sceneInEditorBuildSetting in scenes)
                {
                    if (scene.Path.Equals(sceneInEditorBuildSetting.path, StringComparison.CurrentCultureIgnoreCase))
                    {
                        inEditorBuildSettings = true;
                        sceneInEditorBuildSetting.enabled = scene.Active;
                    }
                }

                if (!inEditorBuildSettings)
                {
                    notInBuildSettingsScenes.Add(scene);
                }
            }

            foreach (var scene in notInBuildSettingsScenes)
            {
                var newScene = new EditorBuildSettingsScene();
                newScene.path = scene.Path;
                newScene.enabled = scene.Active;
                scenes.Add(newScene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            GUIContent basicLayout = new GUIContent("Basic Layout");
            GUIContent AdvanceLayout = new GUIContent("Advanced Layout");

            menu.AddItem(basicLayout, isInBasicLayout, () =>
            {
                isInAdvanceLayout = false;
                isInBasicLayout = true;
            });

            menu.AddItem(AdvanceLayout, isInAdvanceLayout, () =>
            {
                isInAdvanceLayout = true;
                isInBasicLayout = false;
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Save Build Setting As..."), false, () =>
            {
                string title = "Save Build Setting";
                var filePath = EditorUtility.SaveFilePanel(
                            title,
                            "",
                            EditorUserBuildSettings.activeBuildTarget.ToString(),
                            BUILD_SETTING_FILE_EXTENSION);

                if (!string.IsNullOrEmpty(filePath))
                {
                    SaveDataToLocal(filePath);
                }
            });

            menu.AddItem(new GUIContent("Load Build Setting From..."), false, () =>
            {
                string title = "Load Build Setting";
                var filePath = EditorUtility.OpenFilePanel(title, "", BUILD_SETTING_FILE_EXTENSION);
                if (!string.IsNullOrEmpty(filePath))
                {
                    LoadDataFromLocal(filePath);
                    EditorUtility.DisplayDialog("Done!", "Load build setting completed!", "OK");
                }
            });

            menu.AddItem(new GUIContent("Reload Scenes In Project"), false, () =>
            {
                string title = "Reload Scenes In Project?";
                string message = "Load all scenes in project and overwite your current scene in Scene Explorer";
                if (EditorUtility.DisplayDialog(title, message, "OK", "Cancel"))
                {
                    sceneList = GetCurrentSceneList();
                }
            });
        }
    }

    public class AssetsChangeNotification : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            HandleImportedAssets(importedAssets);
            HandleDeleteAssets(deletedAssets);
            HandleMovedAssets(movedAssets, movedFromAssetPaths);
        }

        private static void HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (movedAssets == null ||
                movedFromAssetPaths == null ||
                movedAssets.Length != movedFromAssetPaths.Length)
            {
                return;
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (string.IsNullOrEmpty(movedAssets[i]))
                {
                    continue;
                }

                if (movedAssets[i].ToLower().EndsWith(".unity"))
                {
                    var data = SceneExplorerEditor.sceneList.FirstOrDefault(s => s.Path.Equals(movedFromAssetPaths[i], StringComparison.CurrentCultureIgnoreCase));
                    if (data != null)
                    {
                        data.Path = movedAssets[i];
                        data.Name = Utility.FormatName(Utility.GetFileNameWithoutExtension(movedAssets[i]));
                    }
                }
            }
        }

        private static void HandleDeleteAssets(string[] deletedAssets)
        {
            if (deletedAssets == null)
            {
                return;
            }

            foreach (string assetPath in deletedAssets)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                if (assetPath.ToLower().EndsWith(".unity"))
                {
                    var data = SceneExplorerEditor.sceneList.FirstOrDefault(s => s.Path.Equals(assetPath, StringComparison.CurrentCultureIgnoreCase));
                    if (data != null)
                    {
                        SceneExplorerEditor.sceneList.Remove(data);
                    }
                }
            }
        }

        private static void HandleImportedAssets(string[] importedAssets)
        {
            if (importedAssets == null)
            {
                return;
            }

            foreach (string assetPath in importedAssets)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                if (assetPath.ToLower().EndsWith(".unity") &&
                    !SceneExplorerEditor.sceneList.Any(s => s != null && s.Path.Equals(assetPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var data = Utility.GetSceneDataFromPath(assetPath);
                    SceneExplorerEditor.sceneList.Add(data);
                }
            }
        }
    }

    // ------------------ Utility -----------------------//
    public class Utility
    {
        public static string GetFileNameWithoutExtension(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        public static SceneData GetSceneDataFromPath(string scenePath)
        {
            var sceneName = Utility.GetFileNameWithoutExtension(scenePath);
            SceneData data = new SceneData();
            data.Name = Utility.FormatName(sceneName);
            data.Path = scenePath;
            data.Active = true;
            return data;
        }

        public static string FormatName(string name)
        {
            return "  " + name;
        }

        public static void LoadScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                return;
            }

#if UNITY_5_3_OR_NEWER
            var currentScene = EditorSceneManager.GetActiveScene();
            if(currentScene.isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
#else
            if (EditorApplication.isSceneDirty)
            {
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
            }

            EditorApplication.OpenScene(scenePath);
#endif
        }

        public static void OpenBuildSettings()
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }

    // ------------------ Editor Data class  ------------------- //

    [SerializableAttribute]
    public class SceneData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", Name.Trim(), Active, Path);
        }
    }

    // ------------------- List view ------------------- //

    internal static class GUIClip
    {
        private static Func<Rect> VisibleRect;

        public static void InitType()
        {
            var tyGUIClip = Type.GetType("UnityEngine.GUIClip, UnityEngine");
            if (tyGUIClip != null)
            {
                var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
                if (piVisibleRect != null)
                    VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), piVisibleRect.GetGetMethod());
            }
        }

        public static Rect visibleRect
        {
            get
            {
                InitType();
                return VisibleRect();
            }
        }
    }

    internal class DragAndDropDelay
    {
        public Vector2 mouseDownPosition;

        public bool CanStartDrag()
        {
            return Vector2.Distance(this.mouseDownPosition, Event.current.mousePosition) > 6f;
        }
    }

    public struct ListViewElement
    {
        public int row;
        public int column;
        public Rect position;
    }

    public enum ListViewActionOptions
    {
        Reordering = 1,
        ExternalFiles = 2,
        ToStartCustomDrag = 4,
        ToAcceptCustomDrag = 8
    }

    [Serializable]
    public class ListViewState
    {
        private const int defaultRowHeight = 16;
        public int row;
        public int column;
        public Vector2 scrollPos;
        public int totalRows;
        public int rowHeight;
        public int ID;
        public bool selectionChanged;
        public int draggedFrom;
        public int draggedTo;
        public bool drawDropHere;
        public Rect dropHereRect = new Rect(0f, 0f, 0f, 0f);
        public string[] fileNames;
        public int customDraggedFromID;

        public ListViewState()
        {
            this.Init(0, defaultRowHeight);
        }
        public ListViewState(int totalRows)
        {
            this.Init(totalRows, defaultRowHeight);
        }
        public ListViewState(int totalRows, int rowHeight)
        {
            this.Init(totalRows, rowHeight);
        }
        private void Init(int totalRows, int rowHeight)
        {
            this.row = -1;
            this.column = 0;
            this.scrollPos = Vector2.zero;
            this.totalRows = totalRows;
            this.rowHeight = rowHeight;
            this.selectionChanged = false;
        }
    }

    internal class ListViewCommon
    {
        internal class InternalState
        {
            public int id = -1;
            public int invisibleRows;
            public int endRow;
            public int rectHeight;
            public ListViewState state;
            public bool beganHorizontal;
            public Rect rect;
            public bool wantsReordering;
            public bool wantsExternalFiles;
            public bool wantsToStartCustomDrag;
            public bool wantsToAcceptCustomDrag;
            public int dragItem;
        }

        internal class Constants
        {
            public static string insertion = "PR Insertion";
        }

        internal class ListViewElementsEnumerator : IDisposable, IEnumerator, IEnumerator<ListViewElement>
        {
            private int[] colWidths;
            private int xTo;
            private int yFrom;
            private int yTo;
            private Rect firstRect;
            private Rect rect;
            private int xPos = -1;
            private int yPos = -1;
            private ListViewElement element;
            private ListViewCommon.InternalState internalState;
            private bool quiting;
            private bool isLayouted = false;
            private string dragTitle;

            ListViewElement IEnumerator<ListViewElement>.Current
            {
                get
                {
                    return this.element;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.element;
                }
            }

            internal ListViewElementsEnumerator(ListViewCommon.InternalState internalState, int[] colWidths, int yFrom, int yTo, string dragTitle, Rect firstRect)
            {
                this.colWidths = colWidths;
                this.xTo = colWidths.Length - 1;
                this.yFrom = yFrom;
                this.yTo = yTo;
                this.firstRect = firstRect;
                this.rect = firstRect;
                this.quiting = (internalState.state.totalRows == 0);
                this.internalState = internalState;
                this.dragTitle = dragTitle;
                internalState.state.customDraggedFromID = 0;
                this.Reset();
            }

            public bool MoveNext()
            {
                if (this.xPos > -1)
                {
                    if (ListViewCommon.HasMouseDown(this.internalState, this.rect))
                    {
                        this.internalState.state.selectionChanged = true;
                        this.internalState.state.row = this.yPos;
                        this.internalState.state.column = this.xPos;
                        this.internalState.state.scrollPos = ListViewCommon.ListViewScrollToRow(this.internalState, this.yPos);
                        if ((this.internalState.wantsReordering || this.internalState.wantsToStartCustomDrag) && GUIUtility.hotControl == this.internalState.state.ID)
                        {
                            DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.internalState.state.ID);
                            dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
                            this.internalState.dragItem = this.yPos;
                            ListViewCommon.dragControlID = this.internalState.state.ID;
                        }
                    }

                    if ((this.internalState.wantsReordering || this.internalState.wantsToStartCustomDrag) && GUIUtility.hotControl == this.internalState.state.ID && Event.current.type == EventType.MouseDrag && GUIClip.visibleRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.internalState.state.ID);
                        if (dragAndDropDelay2.CanStartDrag())
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new UnityEngine.Object[0];
                            DragAndDrop.paths = null;
                            if (this.internalState.wantsReordering)
                            {
                                this.internalState.state.dropHereRect = new Rect(this.internalState.rect.x, 0f, this.internalState.rect.width, (float)(this.internalState.state.rowHeight * 2));
                                DragAndDrop.StartDrag(this.dragTitle);
                            }
                            else
                            {
                                if (this.internalState.wantsToStartCustomDrag)
                                {
                                    DragAndDrop.SetGenericData("CustomDragID", this.internalState.state.ID);
                                    DragAndDrop.StartDrag(this.dragTitle);
                                }
                            }
                        }
                        Event.current.Use();
                    }
                }

                this.xPos++;

                if (this.xPos > this.xTo)
                {
                    this.xPos = 0;
                    this.yPos++;
                    this.rect.x = this.firstRect.x;
                    this.rect.width = (float)this.colWidths[0];
                    if (this.yPos > this.yTo)
                    {
                        this.quiting = true;
                    }
                    else
                    {
                        this.rect.y = this.rect.y + this.rect.height;
                    }
                }
                else
                {
                    if (this.xPos >= 1)
                    {
                        this.rect.x = this.rect.x + (float)this.colWidths[this.xPos - 1];
                    }
                    this.rect.width = (float)this.colWidths[this.xPos];
                }

                this.element.row = this.yPos;
                this.element.column = this.xPos;
                this.element.position = this.rect;

                if (this.element.row >= this.internalState.state.totalRows)
                {
                    this.quiting = true;
                }

                if (this.isLayouted && Event.current.type == EventType.Layout && this.yFrom + 1 == this.yPos)
                {
                    this.quiting = true;
                }

                if (this.isLayouted && this.yPos != this.yFrom)
                {
                    GUILayout.EndHorizontal();
                }

                if (this.quiting)
                {
                    if (this.internalState.state.drawDropHere && Event.current.GetTypeForControl(this.internalState.state.ID) == EventType.Repaint)
                    {
                        GUIStyle gUIStyle = ListViewCommon.Constants.insertion;
                        gUIStyle.Draw(gUIStyle.margin.Remove(this.internalState.state.dropHereRect), false, false, false, false);
                    }

                    if (ListViewCommon.ListViewKeyboard(this.internalState, this.colWidths.Length))
                    {
                        this.internalState.state.selectionChanged = true;
                    }

                    if (Event.current.GetTypeForControl(this.internalState.state.ID) == EventType.MouseUp)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    if (this.internalState.wantsReordering && GUIUtility.hotControl == this.internalState.state.ID)
                    {
                        ListViewState state = this.internalState.state;
                        EventType type = Event.current.type;
                        if (type != EventType.DragUpdated)
                        {
                            if (type != EventType.DragPerform)
                            {
                                if (type == EventType.DragExited)
                                {
                                    this.internalState.wantsReordering = false;
                                    this.internalState.state.drawDropHere = false;
                                    GUIUtility.hotControl = 0;
                                }
                            }
                            else
                            {
                                if (GUIClip.visibleRect.Contains(Event.current.mousePosition))
                                {
                                    this.internalState.state.draggedFrom = this.internalState.dragItem;
                                    this.internalState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / (float)state.rowHeight);
                                    if (this.internalState.state.draggedTo > this.internalState.state.totalRows)
                                    {
                                        this.internalState.state.draggedTo = this.internalState.state.totalRows;
                                    }
                                    if (this.internalState.state.draggedTo > this.internalState.state.draggedFrom)
                                    {
                                        this.internalState.state.row = this.internalState.state.draggedTo - 1;
                                    }
                                    else
                                    {
                                        this.internalState.state.row = this.internalState.state.draggedTo;
                                    }
                                    this.internalState.state.selectionChanged = true;
                                    DragAndDrop.AcceptDrag();
                                    Event.current.Use();
                                    this.internalState.wantsReordering = false;
                                    this.internalState.state.drawDropHere = false;
                                }
                                GUIUtility.hotControl = 0;
                            }
                        }
                        else
                        {
                            DragAndDrop.visualMode = ((!this.internalState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
                            Event.current.Use();
                            if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
                            {
                                state.dropHereRect.y = (float)((Mathf.RoundToInt(Event.current.mousePosition.y / (float)state.rowHeight) - 1) * state.rowHeight);
                                if (state.dropHereRect.y >= (float)(state.rowHeight * state.totalRows))
                                {
                                    state.dropHereRect.y = (float)(state.rowHeight * (state.totalRows - 1));
                                }
                                state.drawDropHere = true;
                            }
                        }
                    }
                    else
                    {
                        if (this.internalState.wantsExternalFiles)
                        {
                            EventType type = Event.current.type;
                            if (type != EventType.DragUpdated)
                            {
                                if (type != EventType.DragPerform)
                                {
                                    if (type == EventType.DragExited)
                                    {
                                        this.internalState.wantsExternalFiles = false;
                                        this.internalState.state.drawDropHere = false;
                                        GUIUtility.hotControl = 0;
                                    }
                                }
                                else
                                {
                                    if (GUIClip.visibleRect.Contains(Event.current.mousePosition))
                                    {
                                        this.internalState.state.fileNames = DragAndDrop.paths;
                                        DragAndDrop.AcceptDrag();
                                        Event.current.Use();
                                        this.internalState.wantsExternalFiles = false;
                                        this.internalState.state.drawDropHere = false;
                                        this.internalState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / (float)this.internalState.state.rowHeight);
                                        if (this.internalState.state.draggedTo > this.internalState.state.totalRows)
                                        {
                                            this.internalState.state.draggedTo = this.internalState.state.totalRows;
                                        }
                                        this.internalState.state.row = this.internalState.state.draggedTo;
                                    }
                                    GUIUtility.hotControl = 0;
                                }
                            }
                            else
                            {
                                if (GUIClip.visibleRect.Contains(Event.current.mousePosition) && DragAndDrop.paths != null && DragAndDrop.paths.Length != 0)
                                {
                                    DragAndDrop.visualMode = ((!this.internalState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy);
                                    Event.current.Use();
                                    if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
                                    {
                                        this.internalState.state.dropHereRect = new Rect(this.internalState.rect.x, (float)((Mathf.RoundToInt(Event.current.mousePosition.y / (float)this.internalState.state.rowHeight) - 1) * this.internalState.state.rowHeight), this.internalState.rect.width, (float)this.internalState.state.rowHeight);
                                        if (this.internalState.state.dropHereRect.y >= (float)(this.internalState.state.rowHeight * this.internalState.state.totalRows))
                                        {
                                            this.internalState.state.dropHereRect.y = (float)(this.internalState.state.rowHeight * (this.internalState.state.totalRows - 1));
                                        }
                                        this.internalState.state.drawDropHere = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.internalState.wantsToAcceptCustomDrag && ListViewCommon.dragControlID != this.internalState.state.ID)
                            {
                                EventType type = Event.current.type;
                                if (type != EventType.DragUpdated)
                                {
                                    if (type != EventType.DragPerform)
                                    {
                                        if (type == EventType.DragExited)
                                        {
                                            GUIUtility.hotControl = 0;
                                        }
                                    }
                                    else
                                    {
                                        object genericData = DragAndDrop.GetGenericData("CustomDragID");
                                        if (GUIClip.visibleRect.Contains(Event.current.mousePosition) && genericData != null)
                                        {
                                            this.internalState.state.customDraggedFromID = (int)genericData;
                                            DragAndDrop.AcceptDrag();
                                            Event.current.Use();
                                        }
                                        GUIUtility.hotControl = 0;
                                    }
                                }
                                else
                                {
                                    var data = DragAndDrop.GetGenericData("CustomDragID");
                                    if (GUIClip.visibleRect.Contains(Event.current.mousePosition) && data != null)
                                    {
                                        DragAndDrop.visualMode = ((!this.internalState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
                                        Event.current.Use();
                                    }
                                }
                            }
                        }
                    }

                    if (this.internalState.beganHorizontal)
                    {
                        EditorGUILayout.EndScrollView();
                        GUILayout.EndHorizontal();
                        this.internalState.beganHorizontal = false;
                    }

                    if (this.isLayouted)
                    {

                    }

                    this.internalState.wantsReordering = false;
                    this.internalState.wantsExternalFiles = false;
                }
                else
                {
                    if (this.isLayouted)
                    {

                    }
                }

                if (this.isLayouted)
                {
                    if (!this.quiting)
                    {
                        GUILayout.BeginHorizontal(GUIStyle.none, new GUILayoutOption[0]);
                    }
                    else
                    {
                        GUILayout.EndHorizontal();
                    }
                }

                return !this.quiting;
            }

            public void Reset()
            {
                this.xPos = -1;
                this.yPos = this.yFrom;
            }

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            public void Dispose()
            {

            }
        }

        public static bool OSX = Application.platform == RuntimePlatform.OSXEditor;
        internal static int dragControlID = -1;

        private static bool DoListViewPageUpDown(ListViewCommon.InternalState internalState, ref int selectedRow, ref Vector2 scrollPos, bool up)
        {
            int num = internalState.endRow - internalState.invisibleRows;
            if (up)
            {
                if (!ListViewCommon.OSX)
                {
                    selectedRow -= num;
                    if (selectedRow < 0)
                    {
                        selectedRow = 0;
                    }
                    return true;
                }

                scrollPos.y -= (float)(internalState.state.rowHeight * num);
                if (scrollPos.y < 0f)
                {
                    scrollPos.y = 0f;
                }
            }
            else
            {
                if (!ListViewCommon.OSX)
                {
                    selectedRow += num;
                    if (selectedRow >= internalState.state.totalRows)
                    {
                        selectedRow = internalState.state.totalRows - 1;
                    }
                    return true;
                }
                scrollPos.y += (float)(internalState.state.rowHeight * num);
            }

            return false;
        }

        internal static bool ListViewKeyboard(ListViewCommon.InternalState internalState, int totalCols)
        {
            int totalRows = internalState.state.totalRows;
            return Event.current.type == EventType.KeyDown
                    && totalRows != 0
                    && GUIUtility.keyboardControl == internalState.state.ID
                    && Event.current.GetTypeForControl(internalState.state.ID) == EventType.KeyDown
                    && ListViewCommon.SendKey(internalState, Event.current.keyCode, totalCols);
        }

        internal static bool SendKey(ListViewCommon.InternalState internalState, KeyCode keyCode, int totalCols)
        {
            ListViewState state = internalState.state;

            switch (keyCode)
            {
                case KeyCode.UpArrow:
                    if (state.row > 0)
                    {
                        state.row--;
                    }
                    goto SCROLL_TO_ROW;
                case KeyCode.DownArrow:
                    if (state.row < state.totalRows - 1)
                    {
                        state.row++;
                    }
                    goto SCROLL_TO_ROW;
                case KeyCode.RightArrow:
                    if (state.column < totalCols - 1)
                    {
                        state.column++;
                    }
                    goto SCROLL_TO_ROW;
                case KeyCode.LeftArrow:
                    if (state.column > 0)
                    {
                        state.column--;
                    }
                    goto SCROLL_TO_ROW;
                case KeyCode.Home:
                    state.row = 0;
                    goto SCROLL_TO_ROW;
                case KeyCode.End:
                    state.row = state.totalRows - 1;
                    goto SCROLL_TO_ROW;
                case KeyCode.PageUp:
                    if (!ListViewCommon.DoListViewPageUpDown(internalState, ref state.row, ref state.scrollPos, true))
                    {
                        Event.current.Use();
                        return false;
                    }
                    goto SCROLL_TO_ROW;
                case KeyCode.PageDown:
                    if (!ListViewCommon.DoListViewPageUpDown(internalState, ref state.row, ref state.scrollPos, false))
                    {
                        Event.current.Use();
                        return false;
                    }
                    goto SCROLL_TO_ROW;
            }

            return false;

        SCROLL_TO_ROW:
            state.scrollPos = ListViewCommon.ListViewScrollToRow(internalState, state.scrollPos, state.row);
            Event.current.Use();
            return true;
        }

        internal static bool HasMouseDown(ListViewCommon.InternalState internalState, Rect rect)
        {
            return ListViewCommon.HasMouseDown(internalState, rect, 0);
        }

        internal static bool HasMouseDown(ListViewCommon.InternalState internalState, Rect rect, int button)
        {
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == button
                && rect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = internalState.state.ID;
                GUIUtility.keyboardControl = internalState.state.ID;
                Event.current.Use();
                return true;
            }

            return false;
        }

        internal static bool HasMouseUp(ListViewCommon.InternalState internalState, Rect rect)
        {
            return ListViewCommon.HasMouseUp(internalState, rect, 0);
        }

        internal static bool HasMouseUp(ListViewCommon.InternalState internalState, Rect rect, int button)
        {
            if (Event.current.type == EventType.MouseUp
                && Event.current.button == button
                && rect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
                return true;
            }
            return false;
        }

        internal static bool MultiSelection(ListViewCommon.InternalState internalState, int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
        {
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            bool result = false;

            if ((shift || actionKey) && initialSelected == -1)
            {
                initialSelected = prevSelected;
            }

            if (shift)
            {
                int startIndex = Math.Min(initialSelected, currSelected);
                int endIndex = Math.Max(initialSelected, currSelected);

                for (int j = 0; j < selectedItems.Length; j++)
                {
                    if (selectedItems[j])
                    {
                        result = true;
                    }
                    selectedItems[j] = false;
                }

                if (startIndex < 0)
                {
                    startIndex = endIndex;
                }

                for (int k = startIndex; k <= endIndex; k++)
                {
                    if (!selectedItems[k])
                    {
                        result = true;
                    }

                    selectedItems[k] = true;
                }
            }
            else
            {
                if (actionKey)
                {
                    selectedItems[currSelected] = !selectedItems[currSelected];
                    initialSelected = currSelected;
                    result = true;
                }
                else
                {
                    if (!selectedItems[currSelected])
                    {
                        result = true;
                    }

                    for (int l = 0; l < selectedItems.Length; l++)
                    {
                        if (selectedItems[l] && currSelected != l)
                        {
                            result = true;
                        }

                        selectedItems[l] = false;
                    }

                    initialSelected = -1;
                    selectedItems[currSelected] = true;
                }
            }

            if (internalState != null)
            {
                internalState.state.scrollPos = ListViewCommon.ListViewScrollToRow(internalState, currSelected);
            }

            return result;
        }

        internal static Vector2 ListViewScrollToRow(ListViewCommon.InternalState internalState, int row)
        {
            return ListViewCommon.ListViewScrollToRow(internalState, internalState.state.scrollPos, row);
        }

        internal static int ListViewScrollToRow(ListViewCommon.InternalState internalState, int currPosY, int row)
        {
            return (int)ListViewCommon.ListViewScrollToRow(internalState, new Vector2(0f, (float)currPosY), row).y;
        }

        internal static Vector2 ListViewScrollToRow(ListViewCommon.InternalState internalState, Vector2 currPos, int row)
        {
            if (internalState.invisibleRows < row && internalState.endRow > row)
            {
                return currPos;
            }

            if (row <= internalState.invisibleRows)
            {
                currPos.y = (float)(internalState.state.rowHeight * row);
            }
            else
            {
                currPos.y = (float)(internalState.state.rowHeight * (row + 1) - internalState.rectHeight);
            }

            if (currPos.y < 0f)
            {
                currPos.y = 0f;
            }
            else
            {
                if (currPos.y > (float)(internalState.state.totalRows * internalState.state.rowHeight - internalState.rectHeight))
                {
                    currPos.y = (float)(internalState.state.totalRows * internalState.state.rowHeight - internalState.rectHeight);
                }
            }

            return currPos;
        }
    }

    internal class ListViewGUILayout
    {
        private static int[] dummyWidths = new int[1];
        internal static ListViewCommon.InternalState internalState = new ListViewCommon.InternalState();
        private static int listViewHash = "ListView".GetHashCode();

        public static ListViewCommon.ListViewElementsEnumerator ListView(Rect pos, ListViewState state)
        {
            return ListViewGUILayout.DoListView(pos, state, null, string.Empty);
        }

        public static ListViewCommon.ListViewElementsEnumerator ListView(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListViewGUILayout.ListView(state, (ListViewActionOptions)0, null, string.Empty, style, options);
        }

        public static ListViewCommon.ListViewElementsEnumerator ListView(ListViewState state, int[] colWidths, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListViewGUILayout.ListView(state, (ListViewActionOptions)0, colWidths, string.Empty, style, options);
        }

        public static ListViewCommon.ListViewElementsEnumerator ListView(ListViewState state, ListViewActionOptions lvOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListViewGUILayout.ListView(state, lvOptions, null, string.Empty, style, options);
        }

        public static ListViewCommon.ListViewElementsEnumerator ListView(ListViewState state, ListViewActionOptions lvOptions, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            return ListViewGUILayout.ListView(state, lvOptions, null, dragTitle, style, options);
        }

        public static ListViewCommon.ListViewElementsEnumerator ListView(ListViewState state, ListViewActionOptions lvOptions, int[] colWidths, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
            state.scrollPos = EditorGUILayout.BeginScrollView(state.scrollPos, options);
            ListViewGUILayout.internalState.beganHorizontal = true;
            state.draggedFrom = -1;
            state.draggedTo = -1;
            state.fileNames = null;

            if ((lvOptions & ListViewActionOptions.Reordering) != (ListViewActionOptions)0)
            {
                ListViewGUILayout.internalState.wantsReordering = true;
            }

            if ((lvOptions & ListViewActionOptions.ExternalFiles) != (ListViewActionOptions)0)
            {
                ListViewGUILayout.internalState.wantsExternalFiles = true;
            }

            if ((lvOptions & ListViewActionOptions.ToStartCustomDrag) != (ListViewActionOptions)0)
            {
                ListViewGUILayout.internalState.wantsToStartCustomDrag = true;
            }

            if ((lvOptions & ListViewActionOptions.ToAcceptCustomDrag) != (ListViewActionOptions)0)
            {
                ListViewGUILayout.internalState.wantsToAcceptCustomDrag = true;
            }

            return ListViewGUILayout.DoListView(GUILayoutUtility.GetRect(1f, (float)(state.totalRows * state.rowHeight + 3)), state, colWidths, string.Empty);
        }

        public static ListViewCommon.ListViewElementsEnumerator DoListView(Rect pos, ListViewState state, int[] colWidths, string dragTitle)
        {
            int controlID = GUIUtility.GetControlID(ListViewGUILayout.listViewHash, FocusType.Passive);
            state.ID = controlID;
            state.selectionChanged = false;
            Rect rect;
            if (GUIClip.visibleRect.x < 0f || GUIClip.visibleRect.y < 0f)
            {
                rect = pos;
            }
            else
            {
                rect = ((pos.y >= 0f) ? new Rect(0f, state.scrollPos.y, GUIClip.visibleRect.width, GUIClip.visibleRect.height) : new Rect(0f, 0f, GUIClip.visibleRect.width, GUIClip.visibleRect.height));
            }
            if (rect.width <= 0f)
            {
                rect.width = 1f;
            }
            if (rect.height <= 0f)
            {
                rect.height = 1f;
            }
            ListViewGUILayout.internalState.rect = rect;
            int num = (int)((-pos.y + rect.yMin) / (float)state.rowHeight);
            int num2 = num + (int)Math.Ceiling((double)(((rect.yMin - pos.y) % (float)state.rowHeight + rect.height) / (float)state.rowHeight)) - 1;
            if (colWidths == null)
            {
                ListViewGUILayout.dummyWidths[0] = (int)rect.width;
                colWidths = ListViewGUILayout.dummyWidths;
            }
            ListViewGUILayout.internalState.invisibleRows = num;
            ListViewGUILayout.internalState.endRow = num2;
            ListViewGUILayout.internalState.rectHeight = (int)rect.height;
            ListViewGUILayout.internalState.state = state;

            if (num < 0)
            {
                num = 0;
            }

            if (num2 >= state.totalRows)
            {
                num2 = state.totalRows - 1;
            }

            return new ListViewCommon.ListViewElementsEnumerator(ListViewGUILayout.internalState, colWidths, num, num2, dragTitle, new Rect(0f, (float)(num * state.rowHeight), pos.width, (float)state.rowHeight));
        }

        public static bool MultiSelection(int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
        {
            return ListViewCommon.MultiSelection(ListViewGUILayout.internalState, prevSelected, currSelected, ref initialSelected, ref selectedItems);
        }

        public static bool HasMouseUp(Rect rect)
        {
            return ListViewCommon.HasMouseUp(ListViewGUILayout.internalState, rect, 0);
        }

        public static bool HasMouseDown(Rect rect)
        {
            return ListViewCommon.HasMouseDown(ListViewGUILayout.internalState, rect, 0);
        }

        public static bool HasMouseDown(Rect rect, int button)
        {
            return ListViewCommon.HasMouseDown(ListViewGUILayout.internalState, rect, button);
        }
    }
}

#endif
