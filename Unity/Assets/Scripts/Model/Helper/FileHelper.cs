using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Model
{
    public static class FileHelper
    {
        public enum LoadMode
        {
            UnityWebRequest,
            Stream,
        }

        public enum FilePos
        {
            DataPath,
            StreamingAssetsPath,
            PersistentDataPath
        }

        /// <summary>
        /// 删除文件夹里所有文件和文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                //判断是否文件夹
                if (i is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    //删除子目录和文件
                    subdir.Delete(true);
                }
                else
                {
                    //删除指定文件
                    File.Delete(i.FullName);
                }
            }
        }

        /// <summary>
        /// 判断是否存在此文件夹，没有就逐级创建
        /// </summary>
        /// <param name="dirPath"></param>
        public static void CreateDir(string dirPath)
        {
            for (int i = 0; i < dirPath.Length; i++)
            {
                if (dirPath[i] == '/')
                {
                    var p = dirPath.Substring(0, i + 1);
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                }
            }
        }

        /// <summary>
        /// 加载文件，UnityWebRequest协程
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <param name="call">带结果的回调</param>
        /// <returns></returns>
        public static IEnumerator LoadFileByUnityWebRequest(string path, FilePos pos, Action<byte[]> call)
        {
            path = JoinPath(path, pos, LoadMode.UnityWebRequest);
            using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogWarning($"路径[{path}]的文件获取失败----{uwr.error}");
                    yield break;
                }

                call(uwr.downloadHandler.data);
            }
        }

        ///// <summary>
        ///// 加载文件，UnityWebRequest异步
        ///// </summary>
        ///// <param name="path">文件地址</param>
        public static async Task<byte[]> LoadFileByUnityWebRequestAsync(string path)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                req.SendWebRequest();

                while (!req.isDone)
                {
                    await Task.Delay(50);
                }

                if (req.isHttpError || req.isNetworkError)
                {
                    Debug.LogWarning($"路径[{path}]的文件获取失败----{req.error}");
                    return null;
                }

                return req.downloadHandler.data;
            }
        }

        /// <summary>
        /// 加载文件，流异步（不适用于大文件）
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static async Task<byte[]> LoadFileByStreamAsync(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                var buffer = new byte[fileStream.Length];

                await fileStream.ReadAsync(buffer, 0, buffer.Length);

                return buffer;
            }
        }

        /// <summary>
        /// 加载文件，（不适用于大文件）
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static byte[] LoadFileByStream(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                var buffer = new byte[fileStream.Length];

                fileStream.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }

        /// <summary>
        /// 保存文件(覆盖)，流异步（不适用于大文件）
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static async Task SaveFileByStreamAsync(string path, byte[] buffer)
        {
            using (FileStream fileStream = File.Create(path))
            {
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 保存文件(覆盖),（不适用于大文件）
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static void SaveFileByStream(string path, byte[] buffer)
        {
            using (FileStream fileStream = File.Create(path))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// 绝对路径转换成相对路径,仅适用于编辑器
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static string AbsoluteSwitchRelativelyPath(string path)
        {
            path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
            path = path.Replace('\\', '/');
            return path;
        }

#endif

        public static string JoinPath(string path, FilePos pos, LoadMode mode)
        {
            if (pos == FilePos.DataPath)
            {
#if UNITY_EDITOR_WIN
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.dataPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.dataPath}/{path}";
                }
#elif UNITY_IOS
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.dataPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.dataPath}/{path}";
                }
#elif UNITY_ANDROID
                return null;
#endif
            }
            else if (pos == FilePos.StreamingAssetsPath)
            {
#if UNITY_EDITOR_WIN
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.streamingAssetsPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.streamingAssetsPath}/{path}";
                }
#elif UNITY_IOS
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.streamingAssetsPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.streamingAssetsPath}/{path}";
                }
#elif UNITY_ANDROID
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"{Application.streamingAssetsPath}/{path}";

                    case LoadMode.Stream:
                        return null;
                }
#endif
            }
            else if (pos == FilePos.PersistentDataPath)
            {
#if UNITY_EDITOR_WIN
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.persistentDataPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.persistentDataPath}/{path}";
                }
#elif UNITY_IOS
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"file://{Application.persistentDataPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.persistentDataPath}/{path}";
                }
#elif UNITY_ANDROID
                switch (mode)
                {
                    case LoadMode.UnityWebRequest:
                        return $"{Application.persistentDataPath}/{path}";

                    case LoadMode.Stream:
                        return $"{Application.persistentDataPath}/{path}";
                }
#endif
            }

            return null;
        }
    }
}