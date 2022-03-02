//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using System;
using UnityEditor;

namespace APlus
{
    public class PatchImportSettingRecycleIDUtility
    {
        private static Action<object, int, string, string> _Patch;

        static PatchImportSettingRecycleIDUtility()
        {
            var editorAssembly = typeof(Editor).Assembly;
            var typec = editorAssembly.GetType("PatchImportSettingRecycleID");
            var method = ReflectionUtils.GetMethodFromType(typec, "Patch");
            _Patch = (o, classid, oldname, newname) =>
            {
                method.Invoke(null, new object[] { o, classid, oldname, newname });
            };
        }

        public static void Path(SerializedObject obj, int classid, string oldname, string newname)
        {
            _Patch(obj, classid, oldname, newname);
        }

    }
}
#endif