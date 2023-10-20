using System;
using System.Collections;
using UnityEngine;

namespace StarkSDKSpace
{
    class StarkCallbackHandler
    {
        private static readonly Hashtable responseHT = new Hashtable();

        private static int htCounter = 0;

        private static int GenarateCallbackId()
        {
            if (htCounter > 1000000)
            {
                htCounter = 0;
            }

            htCounter++;
            return htCounter;
        }

        public static string Add<T>(Action<T> callback) where T : StarkBaseResponse
        {
            if (callback == null)
            {
                return "";
            }
            var key = MakeKey();
            responseHT.Add(key, callback);
            return key;
        }

        public static string MakeKey()
        {
            int id = GenarateCallbackId();
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds);
            var key = timestamp.ToString() + '-' + id;
            return key;
        }

        public static void InvokeResponseCallback<T>(string str) where T : StarkBaseResponse
        {
            if (!string.IsNullOrEmpty(str))
            {
                T res = JsonUtility.FromJson<T>(str);
                var id = res.callbackId;

                Callback(id, res);
            }
        }

        public static void Callback<T>(string id, T res)
        {
            if (responseHT.ContainsKey(id))
            {
                var callback = (Action<T>) responseHT[id];
                callback(res);
                responseHT.Remove(id);
            }
            else
            {
                Debug.LogError($"callback id not found, id: {id}");
            }
        }
    }
}