//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace APlus
{
    public class PreferencesItems
    {
        private const string codeTypeKey = "A+ASSETEXPLORER_CODE_FILE_TYPE";
        private const string DEFAULT_CODE_FILE_TYPE = "*.cs;*.js;*.dll;*.jar;*.java;*.cpp;*.m;*.jsx;*.html;*.txt;*.xml;*.json;*.csv;*.h;*.mm;*.so;*.cginc;*.boo;*.a;*.aar;*.c;*.lua;*.py";
        public static string CodeFileType
        {
            get
            {
                return EditorPrefs.GetString(codeTypeKey, DEFAULT_CODE_FILE_TYPE);
            }
            set
            {
                EditorPrefs.SetString(codeTypeKey, value);
            }
        }

        public static void CodeFileTypeUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Code file extensions: ", GUILayout.Width(115));
#if UNITY_5_3_OR_NEWER
        CodeFileType = EditorGUILayout.DelayedTextField(CodeFileType,  GUILayout.ExpandWidth(true));
#else
            CodeFileType = EditorGUILayout.TextField(CodeFileType, GUILayout.ExpandWidth(true));
#endif
            if (GUILayout.Button("Reset", GUILayout.Height(14), GUILayout.Width(48)))
            {
                CodeFileType = DEFAULT_CODE_FILE_TYPE;
            }

            EditorGUILayout.EndHorizontal();
        }

        #region Window Style Config
        private const string DockableWindowKey = "A+ASSETSEXPLORER_DOCKABLEWINDOWS";
        public static bool IsDockableWindowStyle
        {
            get
            {
                return EditorPrefs.GetBool(DockableWindowKey, true);
            }
            set
            {
                EditorPrefs.SetBool(DockableWindowKey, value);
            }
        }

        private static void DockableStyleWindowUI()
        {
            string itemText = "Use dockable style window";
            string toolTip = "Use dockable style window";
            IsDockableWindowStyle = EditorGUILayout.Toggle(new GUIContent(itemText, toolTip), IsDockableWindowStyle);
        }
        #endregion

        private const string AutoRefreshCacheOnProjectLoadKey = "A+ASSETSEXPLORER_AUTOREFRESHCACHEONLOAD";
        public static bool AutoRefreshCacheOnProjectLoad
        {
            get
            {
                return EditorPrefs.GetBool(AutoRefreshCacheOnProjectLoadKey, false);
            }
            set
            {
                EditorPrefs.SetBool(AutoRefreshCacheOnProjectLoadKey, value);
            }
        }

        private static void AutoRefreshCacheOnProjectLoadUI()
        {
            string itemText = "Creating cache automatically";
            string toolTip = "Creating cache automatically on project launch";
            AutoRefreshCacheOnProjectLoad = EditorGUILayout.Toggle(new GUIContent(itemText, toolTip), AutoRefreshCacheOnProjectLoad);
        }

        private const string THEME_KEY = "APLUS_WINDOW_THEME_STYLE";
        public static string THEME
        {
            get
            {
                return EditorPrefs.GetString(THEME_KEY, "classic");
            }
            set
            {
                EditorPrefs.SetString(THEME_KEY, value);
            }
        }

        public static bool ThemeChanged = false;

        private static string[] options = new string[] {
        "Classic", "Pro", "Personal"
    };

        private static void ThemeSelectionUI()
        {
            EditorGUI.BeginChangeCheck();
            ThemeChanged = false;
            int selected = ArrayUtility.IndexOf(options, THEME); ;
            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup("Color Theme: ", selected, options);

            if (selected >= 0)
            {
                THEME = options[selected];
            }

            if (EditorGUI.EndChangeCheck() && selected >= 0)
            {
                ThemeChanged = true;
            }
        }

        public static void PreferencesItemUI()
        {
            EditorGUILayout.Space();
            ThemeSelectionUI();
            EditorGUILayout.Space();
            AutoRefreshCacheOnProjectLoadUI();
            DockableStyleWindowUI();
            EditorGUILayout.Space();
            PreferencesItems.CodeFileTypeUI();
            EditorGUILayout.Space();
            PrefabTools.PreferencesUI();
        }
    }
}

#endif