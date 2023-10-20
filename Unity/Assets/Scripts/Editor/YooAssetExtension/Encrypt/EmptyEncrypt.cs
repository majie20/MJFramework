using Model;
using YooAsset;

public class EmptyEncrypt : IEncryptionServices
{
    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        return new EncryptResult() { EncryptedData = FileHelper.LoadFileByStream(fileInfo.FilePath), LoadMethod = EBundleLoadMethod.Normal };
    }
}