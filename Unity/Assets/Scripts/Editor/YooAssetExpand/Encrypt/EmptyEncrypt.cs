using YooAsset.Editor;

public class EmptyEncrypt : IEncryptionServices
{
    /// <summary>
    /// 检测资源包是否需要加密
    /// </summary>
    bool IEncryptionServices.Check(string bundleName)
    {
        return false;
    }

    /// <summary>
    /// 对数据进行加密，并返回加密后的数据
    /// </summary>
    byte[] IEncryptionServices.Encrypt(byte[] fileData)
    {
        return fileData;
    }
}