using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageRepetitionName 
{
    public static string commonPath = "Assets/Image/Common";

    [MenuItem("Assets/判断是否有重复命名", false, 21)]
    public static void FindRepetitionName()
    {
        if (!Directory.Exists(commonPath))
        {
            Debug.LogError("目前没有公共图片文件夹" + commonPath);
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        //文件
        if (File.Exists(path))
        {
            if (path.EndsWith(".png") || path.EndsWith(".jpg"))
            {
                IsRepetitionName(path, new DirectoryInfo(path).Name);
            }
            else
            {
                Debug.LogError("选择的不是图片");
            }
        }
        //文件夹
        else if (Directory.Exists(path))
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] fileInfos = directory.GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                if (fileInfos[i].Name.EndsWith(".png") || fileInfos[i].Name.EndsWith(".jpg"))
                {
                    string childPath = path + "/" + fileInfos[i].Name;
                    IsRepetitionName(childPath, fileInfos[i].Name);
                }
            }
        }
    }


    public static void IsRepetitionName(string path, string name)
    {
        DirectoryInfo directory = new DirectoryInfo(commonPath);
        FileInfo[] fileInfos = directory.GetFiles();
        for(int  i = 0; i < fileInfos.Length; i++)
        {
            string childPath = commonPath + "/" + fileInfos[i].Name.Replace(".meta", "");
            if (Directory.Exists(childPath)){
                DirectoryInfo childDir = new DirectoryInfo(childPath);
                FileInfo[] childFiles = childDir.GetFiles(); 
                for (int j = 0; j < childFiles.Length; j++)
                {
                    if (childFiles[j].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    string filePath = childPath + "/" + childFiles[j].Name;
                    if (filePath == path)
                    {
                        return;
                    }
                    else if(childFiles[j].Name == name)
                    {
                        Debug.Log(path + "替换名字");
                        AssetDatabase.RenameAsset(path, name.Replace(".png", "").Replace(".jpg", "") + "0");
                    }
                }
            }
        }
    }
}
