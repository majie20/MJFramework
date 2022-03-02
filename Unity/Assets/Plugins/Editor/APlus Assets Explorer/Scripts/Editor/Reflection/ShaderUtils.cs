//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

namespace APlus
{
    public sealed class ShaderUtils
    {
        private static Func<object, bool> _HasShadowCasterPass;
        private static Func<object, int> _GetRenderQueue;
        private static Func<object, int> _GetLOD;
        private static Func<object, bool> _DoesIgnoreProjector;
        private static Func<object, int> _get_disableBatching;
        private static Func<object, bool> _HasSurfaceShaders;
#if UNITY_5
        private static Func<object, bool, int> _GetVariantsCount;
#else
        private static Func<object, bool, ulong> _GetVariantsCount;
#endif 

        static ShaderUtils()
        {
            var editorAssembly = typeof(Editor).Assembly;
            var shaderUtilType = editorAssembly.GetType("UnityEditor.ShaderUtil");

            ReflectionUtils.RegisterMethod(typeof(Shader), "get_disableBatching", ref _get_disableBatching);
            ReflectionUtils.RegisterStaticMethod(shaderUtilType, "HasShadowCasterPass", ref _HasShadowCasterPass);
            ReflectionUtils.RegisterStaticMethod(shaderUtilType, "GetRenderQueue", ref _GetRenderQueue);
            ReflectionUtils.RegisterStaticMethod(shaderUtilType, "GetLOD", ref _GetLOD);
            ReflectionUtils.RegisterStaticMethod(shaderUtilType, "DoesIgnoreProjector", ref _DoesIgnoreProjector);
            ReflectionUtils.RegisterStaticMethod(shaderUtilType, "HasSurfaceShaders", ref _HasSurfaceShaders);
#if !UNITY_5
            ReflectionUtils.RegisterStaticMethod<bool, ulong>(shaderUtilType, "GetVariantCount", ref _GetVariantsCount);
#else
            ReflectionUtils.RegisterStaticMethod<bool, int>(shaderUtilType, "GetComboCount", ref _GetVariantsCount);
#endif
        }

#if UNITY_5
        public static int GetVariantsCount(Shader shader, bool included)
        {
            return _GetVariantsCount(shader, included);
        }
#else
        public static ulong GetVariantsCount(Shader shader, bool sceneOnly)
        {
            return _GetVariantsCount(shader, sceneOnly);
        }
#endif

        public static bool HasShadowCasterPass(Shader shader)
        {
            return _HasShadowCasterPass(shader);
        }

        public static int GetRenderQueue(Shader shader)
        {
            return _GetRenderQueue(shader);
        }

        public static int GetLOD(Shader shader)
        {
            return _GetLOD(shader);
        }

        public static bool DoesIgnoreProjector(Shader shader)
        {
            return _DoesIgnoreProjector(shader);
        }

        public static bool HasSurfaceShaders(Shader shader)
        {
            return _HasSurfaceShaders(shader);
        }

        public static string GetDisableBatching(Shader shader)
        {
            string label = string.Empty;
            switch (_get_disableBatching(shader))
            {
                case 0:
                    label = "no";
                    break;
                case 1:
                    label = "yes";
                    break;
                case 2:
                    label = "when LOD fading is on";
                    break;
                default:
                    label = "unknown";
                    break;
            }

            return label;
        }
    }
}
#endif