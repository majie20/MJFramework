//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

namespace APlus
{
    public sealed class AudioUtil
    {
        private static Func<object, int> _GetFrequency;

        static AudioUtil()
        {
            var assembly = typeof(Editor).Assembly;
            var audioUtilType = assembly.GetType("UnityEditor.AudioUtil");

            ReflectionUtils.RegisterStaticMethod(audioUtilType, "GetFrequency", ref _GetFrequency);
        }

        public static int GetFrequency(AudioClip clip)
        {
            return _GetFrequency(clip);
        }
    }
}
#endif