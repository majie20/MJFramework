using Model;
using System;
using UnityEditor;
using YooAsset;

public class OffsetEncrypt : IEncryptionServices
{
    private ulong offset;

    public OffsetEncrypt()
    {
        var settings = AssetDatabase.LoadAssetAtPath<AssetsBundleSettings>(EditorConst.ASSETS_BUNDLE_SETTINGS_PATH);
        offset = settings.EncryptOffsetVolume;
    }

    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        var bytes = FileHelper.LoadFileByStream(fileInfo.FilePath);
        var temper = new byte[(ulong)bytes.Length + offset];
        Buffer.BlockCopy(bytes, 0, temper, (int)offset, bytes.Length);

        return new EncryptResult() { EncryptedData = temper, LoadMethod = EBundleLoadMethod.LoadFromFileOffset };
    }
}