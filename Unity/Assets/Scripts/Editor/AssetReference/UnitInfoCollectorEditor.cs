using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

public class UnitInfoCollectorEditor
{
    [MenuItem("Tools/资源收集/收集Unit信息", priority = 2)]
    public static void Run()
    {
        List<string> pathList = new List<string>();
        Dictionary<string, UnitInfo> infos = new Dictionary<string, UnitInfo>();
        EditorHelper.GetAssetPath(pathList, EditorConst.UNIT_PREFAB_PATH);

        for (int i = pathList.Count - 1; i >= 0; i--)
        {
            string directoryName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(pathList[i])));
            Enum.TryParse(directoryName, out UnitType type);

            if (type == UnitType.None || Path.GetExtension(pathList[i]) != ".prefab")
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(pathList[i]);

            infos.Add(fileName,
                new UnitInfo() { Type = type, AssetReferenceSettingsPath = $"{Regex.Replace(Path.GetDirectoryName(pathList[i]), @"\\", "/")}/{fileName}_ARS.asset" });
        }

        UnitInfoSettings settings = AssetDatabase.LoadAssetAtPath<UnitInfoSettings>(EditorConst.UNIT_INFO_SETTINGS);
        settings.UnitInfoMap = infos;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}