using UnityEditor;
using UnityEngine;

public class ProtobufExportEditor : EditorWindow
{
    [MenuItem("Tools/Protobuf/协议导出")]
    public static void ProtobufExport()
    {
        EditorHelper.RunMyBat("protogen.bat", "../proto2cs/Proto2CS.Google/");
        //EditorHelper.RunMyBat("protogen.bat", "../proto2cs/Proto2CS.Net/");
        AssetDatabase.Refresh();
        Debug.Log("proto2cs succeed!");
    }
}