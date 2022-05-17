using Model;
using UnityEditor;
using YooAsset.Editor;

public class AESEncrypt : IEncryptionServices
{
    private string password;

    public AESEncrypt()
    {
        var tempSetting = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(
            EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        password = tempSetting.EncryptPassword;
    }

    /// <summary>
    /// 检测资源包是否需要加密
    /// </summary>
    bool IEncryptionServices.Check(string bundleName)
    {
        return true;
    }

    /// <summary>
    /// 对数据进行加密，并返回加密后的数据
    /// </summary>
    byte[] IEncryptionServices.Encrypt(byte[] fileData)
    {
        return AESHelper.Encrypt(fileData, password);
    }
}