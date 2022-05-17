using Model;
using System;
using UnityEditor;
using YooAsset.Editor;

public class OffsetEncrypt : IEncryptionServices
{
    private int offset;

    public OffsetEncrypt()
    {
        var tempSetting = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(
            EditorConfig.ASSETS_BUNDLE_SETTINGS_PATH);
        offset = tempSetting.EncryptOffsetVolume;
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
        var temper = new byte[fileData.Length + offset];
        Buffer.BlockCopy(fileData, 0, temper, offset, fileData.Length);
        return temper;
    }
}