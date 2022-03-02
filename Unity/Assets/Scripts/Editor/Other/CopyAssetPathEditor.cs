using System.Text.RegularExpressions;
using Model;
using UnityEditor;
using UnityEngine;

public class CopyAssetPathEditor
{

    [MenuItem("Assets/工具/复制资源路径", priority = 0)]
    private static void CopyAssetPath()
    {
        GUIUtility.systemCopyBuffer =
            Regex.Replace(
                FileHelper.AbsoluteSwitchRelativelyPath(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0])),
                FileValue.FILE_EXTENSION_PATTERN, "");
    }
}