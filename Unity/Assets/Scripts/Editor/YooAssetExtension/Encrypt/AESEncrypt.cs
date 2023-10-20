using Model;
using UnityEditor;
using YooAsset;

public class AESEncrypt : IEncryptionServices
{
    private string password;

    public AESEncrypt()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);
        password = settings.EncryptPassword;
    }

    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        return new EncryptResult() { EncryptedData = AESHelper.Encrypt(FileHelper.LoadFileByStream(fileInfo.FilePath), password), LoadMethod = EBundleLoadMethod.LoadFromMemory };
    }
}