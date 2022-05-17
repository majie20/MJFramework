using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{

    public enum LoadProgressType 
    {
        None,
        UpdateStaticVersion,
        UpdatePatchManifest,
        DownloadHotAssets,
        DownloadHotAssetsSuccess,
    }
}
