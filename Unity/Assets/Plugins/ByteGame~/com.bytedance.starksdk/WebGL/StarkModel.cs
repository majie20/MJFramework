namespace StarkSDKSpace
{
    public class StarkBaseResponse
    {
        public string callbackId; //回调id,调用者不需要关注
        public string errMsg;
        public int errCode;
    }

    public class StarkBaseActionParam<T>
    {
        public System.Action<T> success; //接口调用成功的回调函数
        public System.Action<T> fail; //接口调用失败的回调函数	
    }

    public class StarkReadFileResponse : StarkBaseResponse
    {
        /// <summary>
        /// 如果返回二进制，则数据在这个字段
        /// </summary>
        public byte[] binData;

        /// <summary>
        /// 如果返回的是字符串，则数据在这个字段
        /// </summary>
        public string stringData;
    }

    public class AccessParam : StarkBaseActionParam<StarkBaseResponse>
    {
        public string path;
    }

    public class UnlinkParam : StarkBaseActionParam<StarkBaseResponse>
    {
        public string filePath;
    }

    public class MkdirParam : StarkBaseActionParam<StarkBaseResponse>
    {
        /// <summary>
        /// 创建的目录路径 (本地路径)
        /// </summary>
        public string dirPath;

        /// <summary>
        /// 是否在递归创建该目录的上级目录后再创建该目录。如果对应的上级目录已经存在，则不创建该上级目录。如 dirPath 为 a/b/c/d 且 recursive 为 true，将创建 a 目录，再在 a 目录下创建 b 目录，以此类推直至创建 a/b/c 目录下的 d 目录。
        /// </summary>
        public bool recursive = false;
    }

    public class RmdirParam : StarkBaseActionParam<StarkBaseResponse>
    {
        /// <summary>
        /// 删除的目录路径 (本地路径)
        /// </summary>
        public string dirPath;

        /// <summary>
        /// 是否递归删除目录。如果为 true，则删除该目录和该目录下的所有子目录以及文件。
        /// </summary>
        public bool recursive = false;
    }

    public class CopyFileParam : StarkBaseActionParam<StarkBaseResponse>
    {
        public string srcPath;
        public string destPath;
    }

    public class RenameFileParam : StarkBaseActionParam<StarkBaseResponse>
    {
        public string srcPath;
        public string destPath;
    }

    public class WriteFileParam : StarkBaseActionParam<StarkBaseResponse>
    {
        /// <summary>
        /// 要写入的文件路径 (本地路径)
        /// </summary>
        public string filePath;

        /// <summary>
        /// 要写入的二进制数据
        /// </summary>
        public byte[] data;
    }

    public class WriteFileStringParam : StarkBaseActionParam<StarkBaseResponse>
    {
        /// <summary>
        /// 要写入的文件路径 (本地路径)
        /// </summary>
        public string filePath;

        /// <summary>
        /// 要写入的二进制数据
        /// </summary>
        public string data;

        /// <summary>
        /// 指定写入文件的字符编码
        /// </summary>
        public string encoding = "utf8";
    }

    public class ReadFileParam : StarkBaseActionParam<StarkReadFileResponse>
    {
        /// <summary>
        /// 要读取的文件的路径 (本地路径)
        /// </summary>
        public string filePath;

        /// <summary>
        /// 指定读取文件的字符编码，如果不传 encoding，则以 ArrayBuffer 格式读取文件的二进制内容
        /// </summary>
        public string encoding;
    }

    public class StatParam : StarkBaseActionParam<StarkStatResponse>
    {
        /// <summary>
        /// 文件/目录路径
        /// </summary>
        public string path;
    }

    public class GetSavedFileListParam : StarkBaseActionParam<StarkGetSavedFileListResponse>
    {
    }

    public class StarkReadFileCallback : StarkBaseResponse
    {
        public string data;
        public int byteLength;
    }
    
    public class StarkGetSavedFileListResponse : StarkBaseResponse
    {
        public StarkFileInfo[] fileList;
    }

    public class StarkStatResponse : StarkBaseResponse
    {
        public StarkStatInfo stat;
    }

    public class StarkBaseFileInfo
    {
        /// <summary>
        /// 文件大小，单位：B
        /// </summary>
        public long size;

        /// <summary>
        /// 文件的类型和存取的权限
        /// </summary>
        public int mode;

        /// <summary>
        /// 判断当前文件是否一个普通文件
        /// </summary>
        /// <returns>是普通文件返回true，不是则返回false</returns>
        public bool IsFile()
        {
            return (61440 & mode) == 32768;
        }

        /// <summary>
        /// 判断当前文件是否一个目录
        /// </summary>
        /// <returns>是目录返回true，不是则返回false</returns>
        public bool IsDirectory()
        {
            return (61440 & mode) == 16384;
        }
    }

    public class StarkFileInfo : StarkBaseFileInfo
    {
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public long createTime;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;
    }

    public class StarkStatInfo : StarkBaseFileInfo
    {
        /// <summary>
        /// 文件最近一次被存取或被执行的时间
        /// </summary>
        public long lastAccessedTime;

        /// <summary>
        /// 文件最后一次被修改的时间
        /// </summary>
        public long lastModifiedTime;
    }
}