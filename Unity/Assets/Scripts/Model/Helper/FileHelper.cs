using System.IO;

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
}