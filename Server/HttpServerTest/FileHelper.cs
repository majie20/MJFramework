using System.IO;
using System.Threading.Tasks;

public static class FileHelper
{
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
}