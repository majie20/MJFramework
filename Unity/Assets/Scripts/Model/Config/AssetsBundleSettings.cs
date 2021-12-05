using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetsBundleSettings", menuName = "ScriptableObjects/AssetsBundleSettings", order = 1)]
public class AssetsBundleSettings : ScriptableObject
{
    public string ABExtension;
    public string ZipExtension;
    public string ZipPassword;
    public string EncryptPassword;
    public string AssetBundleSavePath;
    public string ZipAssetBundleSavePath;
}