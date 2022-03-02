//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

using UnityEditor;
using System;
using UnityEngine;
using System.Reflection;

namespace APlus
{
    public sealed class TextureUtillity
    {
        private static Func<object, int> _GetStorageMemorySize;
        private static Func<object, int> _GetRuntimeMemorySize;
        private static Func<object, bool> _IsNonPowerOfTwo;
        private static MethodInfo _GetWidthAndHeight;

        static TextureUtillity()
        {
            var editorAssembly = typeof(Editor).Assembly;
            var tu = editorAssembly.GetType("UnityEditor.TextureUtil");
            ReflectionUtils.RegisterStaticMethod(tu, "GetStorageMemorySize", ref _GetStorageMemorySize);
            ReflectionUtils.RegisterStaticMethod(tu, "GetRuntimeMemorySize", ref _GetRuntimeMemorySize);
            ReflectionUtils.RegisterStaticMethod(tu, "IsNonPowerOfTwo", ref _IsNonPowerOfTwo);

            _GetWidthAndHeight = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);

        }

        public static bool GetImageSize(Texture2D asset, out int width, out int height)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer != null)
                {
                    object[] args = new object[2] { 0, 0 };
                    _GetWidthAndHeight.Invoke(importer, args);

                    width = (int)args[0];
                    height = (int)args[1];

                    return true;
                }
            }

            height = width = 0;
            return false;
        }

        public static int GetRuntimeMemorySize(Texture texture)
        {
            return _GetRuntimeMemorySize(texture);
        }

        /// <summary>
        /// Get texture tize in storage
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <returns>Size of texture in storage</returns>
        public static int GetStorageMemorySize(Texture texture)
        {
            return _GetStorageMemorySize(texture);
        }

        /// <summary>
        /// Is texture is power of two
        /// </summary>
        /// <param name="texture">texture</param>
        /// <returns>Is power of two</returns>
        public static bool IsPowerOfTwo(Texture texture)
        {
            return !_IsNonPowerOfTwo(texture as Texture2D);
        }
    }
}
#endif