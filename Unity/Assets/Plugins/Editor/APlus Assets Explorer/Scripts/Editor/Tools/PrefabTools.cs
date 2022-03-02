//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace APlus
{
    #pragma warning disable
    public class PrefabTools
    {
        public static string PREFAB_FOLDER = "Assets/Prefabs";
        public static string PREFAB_STORAGE_FOLDER_KEY = "PREFAB_STORAGE_FOLDER_KEY";
        public static string PrefabsStorageFolder
        {
            get
            {
                return EditorPrefs.GetString(PREFAB_STORAGE_FOLDER_KEY, PREFAB_FOLDER);
            }
            set
            {
                EditorPrefs.SetString(PREFAB_STORAGE_FOLDER_KEY, value);
            }
        }

        public static void PreferencesUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefabs Save folder: ", GUILayout.Width(115));

#if UNITY_5_3_OR_NEWER
            PrefabsStorageFolder = EditorGUILayout.DelayedTextField(PrefabsStorageFolder,  GUILayout.ExpandWidth(true));
#else
            PrefabsStorageFolder = EditorGUILayout.TextField(PrefabsStorageFolder, GUILayout.ExpandWidth(true));
#endif
            if (GUILayout.Button("Reset", GUILayout.Height(14), GUILayout.Width(48)))
            {
                PrefabsStorageFolder = PREFAB_FOLDER;
            }

            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("GameObject/A+ Prefab Tools/New If Needed with Connection %#d", false, 0)]
        public static void CreateWithConnection(MenuCommand command)
        {
            if (command.context == null)
            {
                CreatePrefabWithOptionsAndCheck(ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                CreatePrefabWithOptionsAndCheck(command.context as GameObject, ReplacePrefabOptions.ConnectToPrefab);
            }
        }

        [MenuItem("GameObject/A+ Prefab Tools/New If Needed Without Connection %#g", false, 0)]
        public static void CreateWihtoutConnection(MenuCommand command)
        {
            if (command.context == null)
            {
                CreatePrefabWithOptionsAndCheck(ReplacePrefabOptions.Default);
            }
            else
            {
                CreatePrefabWithOptionsAndCheck(command.context as GameObject, ReplacePrefabOptions.Default);
            }
        }

        [MenuItem("GameObject/A+ Prefab Tools/New Prefab With Connection &%#d", false, 0)]
        public static void CreateWithConnectionForceNew(MenuCommand command)
        {
            if (command.context == null)
            {
                CreatePrefabWithOptionsWithoutCheck(ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                CreateNewPrefab(command.context as GameObject, ReplacePrefabOptions.ConnectToPrefab);
            }
        }

        [MenuItem("GameObject/A+ Prefab Tools/New Prefab Without Connection &%#g", false, 0)]
        public static void CreateWithoutConnectionForceNew(MenuCommand command)
        {
            if (command.context == null)
            {
                CreatePrefabWithOptionsWithoutCheck(ReplacePrefabOptions.Default);
            }
            else
            {
                CreateNewPrefab(command.context as GameObject, ReplacePrefabOptions.Default);
            }
        }

        [MenuItem("GameObject/A+ Prefab Tools/Find All Prefabs Instances", false, 22)]
        public static void FindAllPrefabs()
        {
            FindPrefabsAndThenSelectInScene(go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType != PrefabType.None;
            });
        }

        [MenuItem("GameObject/A+ Prefab Tools/Find Instances With Connection", false, 22)]
        public static void FindAllConnectedPrefabs()
        {
            FindPrefabsAndThenSelectInScene(go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.ModelPrefabInstance
                        || goPrefabType == PrefabType.PrefabInstance;
            });
        }

        [MenuItem("GameObject/A+ Prefab Tools/Find Disconnected Prefab Instances", false, 22)]
        public static void FindAllDisconectedPrefabs()
        {
            FindPrefabsAndThenSelectInScene(go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.DisconnectedPrefabInstance || goPrefabType == PrefabType.DisconnectedModelPrefabInstance;
            });
        }

        [MenuItem("GameObject/A+ Prefab Tools/Find Missing Prefab Instances", false, 22)]
        public static void FindAllMissingPrefabObjects()
        {
            FindPrefabsAndThenSelectInScene(go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.MissingPrefabInstance;
            });
        }

        [MenuItem("GameObject/A+ Prefab Tools/Find Model Prefab Instances", false, 22)]
        public static void FindAllModelPrefabInstances()
        {
            FindPrefabsAndThenSelectInScene(go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.ModelPrefabInstance;
            });
        }

        [MenuItem("GameObject/A+ Prefab Tools/Apply All Selected Prefabs", false, 33)]
        public static void ApplaySelectedPrefabs(MenuCommand command)
        {
            Predicate<GameObject> predicate = go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.DisconnectedPrefabInstance ||
                       goPrefabType == PrefabType.PrefabInstance;
            };

            Action<GameObject> action = go => {
                var prefab = PrefabUtility.GetPrefabParent(go);
                PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
            };

            FindPrefabsAndDoSomething(command, predicate, action);
        }

        [MenuItem("GameObject/A+ Prefab Tools/Revert All Selected Prefabs", false, 33)]
        public static void RevertSelectedPrefabs(MenuCommand command)
        {
            Predicate<GameObject> predicate = go =>
            {
                PrefabType goPrefabType = PrefabUtility.GetPrefabType(go);
                return goPrefabType == PrefabType.DisconnectedPrefabInstance ||
                       goPrefabType == PrefabType.DisconnectedModelPrefabInstance ||
                       goPrefabType == PrefabType.ModelPrefabInstance ||
                       goPrefabType == PrefabType.PrefabInstance;
            };

            Action<GameObject> action = go => {
                PrefabUtility.ReconnectToLastPrefab(go);
                PrefabUtility.RevertPrefabInstance(go);
            };

            FindPrefabsAndDoSomething(command, predicate, action);
        }

        [MenuItem("GameObject/A+ Prefab Tools/Break Prefab Instances", false, 33)]
        public static void BreakPrefabInstances(MenuCommand command)
        {
            EditorApplication.ExecuteMenuItem("GameObject/Break Prefab Instance");
        }

        private static void FindPrefabsAndDoSomething(MenuCommand command, Predicate<GameObject> predicate, Action<GameObject> action)
        {
            if (command.context == null)
            {
                FindPrefabsAndDoSomething(predicate, action);
            }
            else
            {
                var go = command.context as GameObject;
                if(go == null)
                {
                    return;
                }

                if (predicate.Invoke(go))
                {
                    var root = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
                    action(root);
                }
            }
        }

        private static void FindPrefabsAndDoSomething(Predicate<GameObject> predicate, Action<GameObject> action)
        {
            var prefabObjects = FindRootGameObjects(predicate, Selection.gameObjects);
            foreach (var prefab in prefabObjects)
            {
                if (action != null)
                {
                    action(prefab);
                }
            }
        }

        private static void FindPrefabsAndThenSelectInScene(Predicate<GameObject> predicate)
        {
            var objects = GameObject.FindObjectsOfType(typeof(GameObject));
            Selection.objects = FindRootGameObjects(predicate, objects);
        }

        private static GameObject[] FindRootGameObjects(Predicate<GameObject> predicate, UnityEngine.Object[] objects)
        {
            HashSet<GameObject> prefabObjects = new HashSet<GameObject>();

            foreach (var obj in objects)
            {
                var go = obj as GameObject;
                if (go != null)
                {
                    if (predicate(go))
                    {
                        prefabObjects.Add(PrefabUtility.FindRootGameObjectWithSameParentPrefab(go));
                    }
                }
            }

            return prefabObjects.ToArray();
        }

        private static void CreatePrefabWithOptionsWithoutCheck(ReplacePrefabOptions option)
        {
            foreach (var go in Selection.gameObjects)
            {
                CreateNewPrefab(go, option);
            }
        }

        private static void CreatePrefabWithOptionsAndCheck(ReplacePrefabOptions option)
        {
            foreach (var go in Selection.gameObjects)
            {
                CreatePrefabWithOptionsAndCheck(go, option);
            }

            AssetDatabase.SaveAssets();
        }

        private static void CreatePrefabWithOptionsAndCheck(GameObject go, ReplacePrefabOptions option)
        {
            if (go == null)
            {
                return;
            }

            PrefabType type = PrefabUtility.GetPrefabType(go);

            switch (type)
            {
                case PrefabType.None:
                case PrefabType.MissingPrefabInstance:
                case PrefabType.ModelPrefabInstance:
                    // Only create Prefab for non or missing Prefab
                    //
                    CreateNewPrefab(go, option);
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                case PrefabType.DisconnectedModelPrefabInstance:
                    // If it's a disconnected prefab, reconnect it
                    //
                    PrefabUtility.ReconnectToLastPrefab(go);
                    break;
                default:
                    break;
            }
        }

        private static void CreateNewPrefab(GameObject go, ReplacePrefabOptions option)
        {
            if (go == null)
            {
                return;
            }

            // Ensure folder is exists
            //
            Directory.CreateDirectory(PrefabsStorageFolder);
            string path = GetGeneratedPath(go);
            var prefab = PrefabUtility.CreateEmptyPrefab(path);
            PrefabUtility.ReplacePrefab(go, prefab, option);
            Debug.Log(string.Format("New Prefab: {0}", path));
        }

        private static string GetGeneratedPath(GameObject go)
        {
            string fileName = go.name + ".prefab";
            string path = Path.Combine(PrefabsStorageFolder, fileName);

            if (File.Exists(path))
            {
                path = AddSuffixOfPath(path);
            }

            return path.Replace("\\", "/");
        }

        private static string AddSuffixOfPath(string path)
        {
            var filename = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var fileExt = Path.GetExtension(path);

            bool hasSuffix = Regex.IsMatch(filename, string.Format(@"[\s\S]+_\d+{0}", fileExt));
            if (hasSuffix)
            {
                int indexOf_ = filename.LastIndexOf("_");
                string filenameWithoutExtensions = filename.Substring(0, indexOf_);
                string filenameSuffix = filename.Substring(indexOf_);
                int Length = filenameSuffix.Length - fileExt.Length - 1;
                string number = filenameSuffix.Substring(1, Length);
                int num = 0;
                string newPath = string.Empty;

                if (int.TryParse(number, out num))
                {
                    num++;
                    newPath = string.Format(@"{0}/{1}_{2}{3}", folder, filenameWithoutExtensions, num, fileExt);
                }

                if (File.Exists(newPath))
                {
                    newPath = AddSuffixOfPath(newPath);
                    return newPath;
                }
                else if (!string.IsNullOrEmpty(newPath))
                {
                    return newPath;
                }
            }

            var fileNameWithoutExt = filename.Substring(0, filename.Length - fileExt.Length);
            string returnPath = string.Format(@"{0}/{1}_1{2}", folder, fileNameWithoutExt, fileExt);
            if (File.Exists(returnPath))
            {
                returnPath = AddSuffixOfPath(returnPath);
            }

            return returnPath;
        }
    }
    #pragma warning restore
}
#endif