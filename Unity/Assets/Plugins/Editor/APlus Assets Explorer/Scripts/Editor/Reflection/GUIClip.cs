//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using System;

namespace APlus
{
    public sealed class GUIClip
    {
        private static Func<Rect, Rect> _Unclip;

        static GUIClip()
        {
            var assembly = typeof(GUI).Assembly;
            var type = assembly.GetType("UnityEngine.GUIClip");


			var method = type.GetMethod("Unclip", new Type[] { typeof(Rect) });
            _Unclip = arg1 =>
            {
                var retunVal = method.Invoke(null, new object[] { arg1 });
                return (Rect)retunVal;
            };
        }

        public static Rect Unclip(Rect clip)
        {
            return _Unclip(clip);
        }
    }
}
#endif
