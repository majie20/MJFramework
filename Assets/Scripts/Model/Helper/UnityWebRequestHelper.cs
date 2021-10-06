using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace MGame.Model
{
    public class UnityWebRequestHelper
    {
        public static IEnumerator LoadFile(string path, Action<DownloadHandler> call)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(path);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning($"路径[{path}]的文件获取失败----{uwr.error}");
                yield break;
            }

            call(uwr.downloadHandler);
        }
    }
}