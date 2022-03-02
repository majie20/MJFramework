//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace APlus
{
    public class SyncManager : MonoBehaviour
    {
        public static bool Enable = true;

        public static Queue<APAsset> ModifiedAssets = new Queue<APAsset>();
        public static Queue<APAsset> DeleteAssets = new Queue<APAsset>();
        public static Queue<string> MovedFromAssets = new Queue<string>();
        public static Queue<APAsset> MovedToAssets = new Queue<APAsset>();
        public static Queue<APAsset> AddedAssets = new Queue<APAsset>();

        public static void Process(WebViewCommunicationService service, bool saveToLocal = false)
        {
            if (!Enable)
            {
                return;
            }

            if (!IfNeedToProcess())
            {
                return;
            }

            var modifed = LoopDequeueToList(ModifiedAssets);
            var deleted = LoopDequeueToList(DeleteAssets);
            var movedFrom = LoopDequeueToList(MovedFromAssets);
            var movedTo = LoopDequeueToList(MovedToAssets);
            var added = LoopDequeueToList(AddedAssets);

            service?.DoAssetsChange(added, deleted, modifed, movedFrom, movedTo);

            if (saveToLocal)
            {
                APCache.SaveToLocalAsync();
            }

            Resources.UnloadUnusedAssets();
        }

        private static bool IfNeedToProcess()
        {
            return ModifiedAssets.Count > 0 || DeleteAssets.Count > 0 || MovedFromAssets.Count > 0 || MovedToAssets.Count > 0 || AddedAssets.Count > 0;
        }

        private static List<T> LoopDequeueToList<T>(Queue<T> queue)
        {
            List<T> list =  new List<T>();
            while (queue.Count > 0)
            {
                list.Add(queue.Dequeue());
            }

            return list;
        }
    }
}
#endif