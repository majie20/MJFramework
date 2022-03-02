using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{

    public enum LoadType 
    {
        None,
        LoadAssetsConfig,
        LoadAssetsConfigComplete,
        CheckAssetsUpdate,
        DownloadHotAssets,
        DownloadHotAssetsComplete,
        UnzipAssets,
        UnzipAssetsComplete,
        LoadAssets,
        LoadAssetsComplete,
    }
}
