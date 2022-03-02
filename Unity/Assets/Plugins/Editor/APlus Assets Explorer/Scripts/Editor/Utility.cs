//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEditor;

    public class Utility
    {
        /// <summary>
        /// Get file name (with extensions)
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns>File name</returns>
        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetFileName(path);
        }

        public static long GetFileSize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }

            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                return 0;
            }

            return fi.Length;
        }

        public static bool IsFileExtensionMatch(string path, params string[] fileExtension)
        {
            FileInfo fi = new FileInfo(path);
            return fileExtension.Any(fs => fs.Equals(fi.Extension, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Get description of size in B or KB or MB
        /// </summary>
        /// <param name="size">Size in int</param>
        /// <returns></returns>
        public static string GetSizeDescription(int size)
        {
            if (size < 1024)
            {
                return string.Format("{0} B", size);
            }
            else if (size < 1048576) // 1024 * 1024
            {
                return string.Format("{0:f1} KB", size * 1.0f / 1024);
            }
            else
            {
                return string.Format("{0:f1} MB", size * 1.0 / 1048576);
            }
        }

        public static string GetTimeDurationString(TimeSpan duration)
        {
            return duration.TotalMilliseconds.ToString();
        }

        public static string GetFileExtension(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return string.Empty;
            }

            return Path.GetExtension(file);
        }

        public static string[] GetFilesInRelativePath(string folder, string searchPattern = "*.*")
        {
            if (string.IsNullOrEmpty(folder))
            {
                return null;
            }

            if (!Directory.Exists(folder))
            {
                return null;
            }

            string[] files = Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories);
            List<string> lists = new List<string>();
            foreach (var item in files)
            {
                string lowerItem = item.ToLower().Trim();
                if (lowerItem.EndsWith(".meta") || lowerItem.EndsWith(".ds_store"))
                {
                    continue;
                }

                lists.Add(item.Trim().Substring(folder.Length));
            }

            return lists.ToArray();
        }

        public static bool IsStreamingAssetsFile(string assetPath)
        {
            string lowerCasePath = assetPath.ToLower();
            if (lowerCasePath.StartsWith("assets/streamingassets")
                || lowerCasePath.StartsWith(@"assets\streamingassets"))
            {
                return true;
            }

            return false;
        }

        public static bool IsCodeFile(string assetPath)
        {
            string[] extensions = GetCodeFileExtensions();
            return IsFileExtensionMatch(assetPath, extensions);
        }

        public static string[] GetCodeFileExtensions()
        {
            return PreferencesItems.CodeFileType.ToLower().Split(new char[] { ';', '*' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsModelPath(string assetPath)
        {
            string[] modelFileExtensions = new string[] { ".fbx", ".obj", ".dae", ".3DS", ".dxf", ".MAX", ".MB", ".MA" };
            return Utility.IsFileExtensionMatch(assetPath, modelFileExtensions);
        }

        public static bool IsModels(UnityEngine.Object obj, string assetPath)
        {
            return obj is GameObject && IsModelPath(assetPath);
        }

        public static bool IsPrefab(string assetPath)
        {
            return assetPath.ToLower().EndsWith(".prefab");
        }

        public static bool IsMaterial(string path)
        {
            var extensions = new string[] { ".physicmaterial", ".mat", ".physicsmaterial2d" };
            return extensions.Any(ext => path.ToLower().EndsWith(ext));
        }

        public static bool IsUntyNewAnimation(string path)
        {
            return path.ToLower().EndsWith(".anim");
        }

        public static bool IsScriptAsset(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            return assetPath.EndsWith(".cs") || assetPath.EndsWith(".js");
        }

        public static string SafeJson(string json)
        {
            StringBuilder sb = new System.Text.StringBuilder(json);
            sb = sb.Replace("\t", "");
            sb = sb.Replace("\r", "");
            sb = sb.Replace("\n", "");
            sb = sb.Replace("\f", "");
            sb = sb.Replace(@"\", @"\\");
            return sb.ToString();
        }

        public static void UpdateJsonInAsset(APAsset asset)
        {
            string json = APJsonSerializer.ToJson(asset);
            asset.Json = SafeJson(json);
        }

        public static string GetShaderRenderQueueText(int renderQueue)
        {
            var dq = renderQueue;
            if (dq < 1000)
            {
                return "Background-" + (1000 - dq);
            }
            else if (dq == 1000)
            {
                return "Background";
            }
            else if (dq <= 1500)
            {
                return "Background+" + (dq - 1000);
            }
            else if (dq < 2000)
            {
                return "Geometry-" + (2000 - dq);
            }
            else if (dq == 2000)
            {
                return "Geometry";
            }
            else if (dq <= 2225)
            {
                return "Geometry+" + (dq - 2000);
            }
            else if (dq < 2450)
            {
                return "AlphaTest-" + (2450 - dq);
            }
            else if (dq == 2450)
            {
                return "AlphaTest";
            }
            else if (dq <= 2725)
            {
                return "AlphaTest+" + (dq - 2450);
            }
            else if (dq < 3000)
            {
                return "Transparent-" + (3000 - dq);
            }
            else if (dq == 3000)
            {
                return "Transparent";
            }
            else if (dq <= 3500)
            {
                return "Transparent+" + (dq - 3000);
            }
            else if (dq < 4000)
            {
                return "Overlay-" + (4000 - dq);
            }
            else if (dq == 4000)
            {
                return "Overlay";
            }
            else
            {
                return "Overlay+" + (dq - 4000);
            }
        }

        public static string MD5(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().Substring(8, 16);
        }

        public static string GetFileMd5(string filePath)
        {
            String hashMD5 = String.Empty;

            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create())
                    {

                        Byte[] buffer = calculator.ComputeHash(fs);
                        calculator.Clear();
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            stringBuilder.Append(buffer[i].ToString("x2"));
                        }

                        hashMD5 = stringBuilder.ToString();
                    }
                }
            }

            return hashMD5;
        }

        public static void DebugLog(string s)
        {
#if APLUS_DEV
            UnityEngine.Debug.Log(s);
#endif
        }

        public static long GetLocalIndentifierOfObject(UnityEngine.Object unityobject)
        {
            var inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            SerializedObject serializedObject = new SerializedObject(unityobject);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
            var serializedProperty = serializedObject.FindProperty("m_LocalIdentfierInFile");
            DebugLog("localId: " + serializedProperty.longValue);
            return serializedProperty.longValue;
        }

        public static string GetInstanceIdFromAssetId(string assetId)
        {
            var guid = GetGuidFromAssetId(assetId);
            var fileId = GetFileIdFromAssetId(assetId);

            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(guid))
            {
                return string.Empty;
            }

            var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid));
            foreach (var item in objects)
            {
                if (fileId.Equals(GetLocalIndentifierOfObject(item).ToString()))
                {
                    return item.GetInstanceID().ToString();
                }
            }

            return string.Empty;
        }

        public static bool IsSubAsset(string assetid)
        {
            return !string.IsNullOrEmpty(assetid) && assetid.Length == 39;
        }

        public static string GetAssetId(string guid, string fileid)
        {
            return string.Format("{0}{1}", guid, fileid);
        }

        public static string GetGuidFromAssetId(string assetid)
        {
            if (string.IsNullOrEmpty(assetid) || assetid.Length < 32 || assetid.Length > 39)
            {
                return null;
            }

            return assetid.Substring(0, 32);
        }

        public static string GetFileIdFromAssetId(string assetid)
        {
            if (string.IsNullOrEmpty(assetid) || assetid.Length != 39)
            {
                return null;
            }

            return assetid.Substring(32, 7);
        }

        public static string GetContainerFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            FileInfo file = new FileInfo(path);
            return file.Directory.FullName;
        }

        public static bool IsInResources(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var editorIndex = path.IndexOf("/Editor/");
            var resoucesIndex = path.IndexOf("/Resources/");
            if (resoucesIndex != -1)
            {
                if (editorIndex != -1 && editorIndex < resoucesIndex)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static string BuildTargetToPlatform(BuildTarget target)
        {
            //  "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", 
            //  "Tizen", "PSP2", "PS4", "XboxOne", "Samsung TV", "Nintendo 3DS", "WiiU" and "tvOS"
            //
            switch (target)
            {
                case BuildTarget.PS4:
                    return "PS4";
#if !UNITY_5_5_OR_NEWER
                case BuildTarget.PS3:
                    return "PS3";
#endif
                case BuildTarget.WSAPlayer:
                    return "Windows Store Apps";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
#else   
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
#endif
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Standalone";
                case BuildTarget.iOS:
                    return "iPhone";
#if !UNITY_5_4_OR_NEWER
                case BuildTarget.WebPlayer:
                case BuildTarget.WebPlayerStreamed:
                    return "Web";
#endif
                default:
                    return target.ToString();
            }
        }
    }
}
#endif