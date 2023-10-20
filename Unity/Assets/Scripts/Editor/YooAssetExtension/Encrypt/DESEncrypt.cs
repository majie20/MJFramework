using Model;
using UnityEditor;
using YooAsset;

public class DESEncrypt : IEncryptionServices
{
    private string password;

    public DESEncrypt()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);
        password = settings.EncryptPassword;
    }

    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        return new EncryptResult() { EncryptedData = DESHelper.Encrypt(FileHelper.LoadFileByStream(fileInfo.FilePath), password), LoadMethod = EBundleLoadMethod.LoadFromMemory };
    }
}