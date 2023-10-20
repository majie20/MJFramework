using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Text;
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
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录

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
        /// 删除文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDirAll(string srcPath)
        {
            if (System.IO.Directory.Exists(srcPath))
            {
                try
                {
                    var dir = new System.IO.DirectoryInfo(srcPath);
                    dir.Attributes &= ~FileAttributes.ReadOnly;
                    dir.Delete(true);
                }
                catch (Exception ex)
                {
                    NLog.Log.Error($" 文件夹存在 删除文件夹时 出现错误 【{ex.Message}】");
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
        /// 将绝对地址转换成相对地址
        /// </summary>
        /// <param name="absolutePath">相对于</param>
        /// <param name="relativeTo">原始绝对路径</param>
        /// <returns></returns>
        public static string RelativePath(string absolutePath, string relativeTo)
        {
            //from - www.cnphp6.com

            string[] absoluteDirectories = absolutePath.Split('/');
            string[] relativeDirectories = relativeTo.Split('/');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                throw new ArgumentException("Paths do not have a common base");

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append("../");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "/");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }

        /////// <summary>
        /////// 加载文件，UnityWebRequest异步
        /////// </summary>
        /////// <param name="path">文件地址</param>
        //public static async UniTask<byte[]> LoadFileByUnityWebRequestAsync(string path)
        //{
        //    using (UnityWebRequest req = UnityWebRequest.Get(path))
        //    {
        //        await req.SendWebRequest();

        //        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        //        {
        //            NLog.Log.Warn($"路径[{path}]的文件获取失败----{req.error}");

        //            return null;
        //        }

        //        return req.downloadHandler.data;
        //    }
        //}

        /////// <summary>
        /////// 加载文件，UnityWebRequest同步
        /////// </summary>
        /////// <param name="path">文件地址</param>
        //public static byte[] LoadFileByUnityWebRequest(string path)
        //{
        //    using (UnityWebRequest req = UnityWebRequest.Get(path))
        //    {
        //        req.SendWebRequest().GetAwaiter().GetResult();

        //        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        //        {
        //            NLog.Log.Warn($"路径[{path}]的文件获取失败----{req.error}");

        //            return null;
        //        }

        //        return req.downloadHandler.data;
        //    }
        //}

        /////// <summary>
        /////// 加载文件，UnityWebRequest异步,返回进度
        /////// </summary>
        /////// <param name="path">文件地址</param>
        //public static async UniTask<byte[]> LoadFileByUnityWebRequestAsync(string path, Action<float> call)
        //{
        //    using (UnityWebRequest req = UnityWebRequest.Get(path))
        //    {
        //        await req.SendWebRequest().ToUniTask(Progress.CreateOnlyValueChanged(call));
        //        call(1.0f);

        //        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        //        {
        //            NLog.Log.Warn($"路径[{path}]的文件获取失败----{req.error}");

        //            return null;
        //        }

        //        return req.downloadHandler.data;
        //    }
        //}

        /// <summary>
        /// 加载文件，流异步（不适用于大文件）
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns></returns>
        public static async UniTask<byte[]> LoadFileByStreamAsync(string path)
        {
#if WX&&!UNITY_EDITOR
            return await Game.Instance.Scene.GetComponent<WXFileSystemManagerComponent>().ReadFile(path);
#else

            using (FileStream fileStream = File.OpenRead(path))
            {
                var buffer = new byte[fileStream.Length];
#if !UNITY_WEBGL
            await UniTask.SwitchToThreadPool();
#endif
                await fileStream.ReadAsync(buffer, 0, buffer.Length);
#if !UNITY_WEBGL
            await UniTask.SwitchToMainThread();
#endif

                return buffer;
            }
#endif
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
        public static async UniTask SaveFileByStreamAsync(string path, byte[] buffer)
        {
#if WX&&!UNITY_EDITOR
            await Game.Instance.Scene.GetComponent<WXFileSystemManagerComponent>().WriteFile(path, buffer);
#else
            using (FileStream fileStream = File.Create(path))
            {
#if !UNITY_WEBGL
            await UniTask.SwitchToThreadPool();
#endif
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
#if !UNITY_WEBGL
            await UniTask.SwitchToMainThread();
#endif
            }
#endif
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
            switch (pos)
            {
                case FilePos.DataPath:
#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
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
#elif WX
                    return null;
#elif TT
                    return null;
#elif UNITY_WEBGL
                    switch (mode)
                    {
                        case LoadMode.UnityWebRequest:
                            return $"file://{Application.dataPath}/{path}";

                        case LoadMode.Stream:
                            return $"{Application.dataPath}/{path}";
                    }
#endif
                    break;
                case FilePos.StreamingAssetsPath:
#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
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
                            return $"jar:file://{Application.streamingAssetsPath}/{path}";

                        case LoadMode.Stream:
                            return null;
                    }
#elif WX
                    return null;
#elif TT
                    return null;
#elif UNITY_WEBGL
                    switch (mode)
                    {
                        case LoadMode.UnityWebRequest:
                            return $"file://{Application.streamingAssetsPath}/{path}";

                        case LoadMode.Stream:
                            return $"{Application.streamingAssetsPath}/{path}";
                    }
#endif
                    break;
                case FilePos.PersistentDataPath:
#if UNITY_STANDALONE_WIN||UNITY_EDITOR
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
                            return $"jar:file://{Application.persistentDataPath}/{path}";

                        case LoadMode.Stream:
                            return $"{Application.persistentDataPath}/{path}";
                    }
#elif WX
                    return $"{WeChatWASM.WXBase.env.USER_DATA_PATH}/{path}";
#elif TT
                    return $"{StarkSDKSpace.StarkFileSystemManagerWebGL.USER_DATA_PATH}/{path}";
#elif UNITY_WEBGL
                    switch (mode)
                    {
                        case LoadMode.UnityWebRequest:
                            return $"file://{Application.persistentDataPath}/{path}";

                        case LoadMode.Stream:
                            return $"{Application.persistentDataPath}/{path}";
                    }
#endif
                    break;
            }

            return null;
        }
    }
}