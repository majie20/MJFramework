using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.U2D;
using UnityEditor.U2D;
using Model;

public class ImageAtlas
{
    private static string folderABPath = "Assets/Res/BuildAB/Atlas";
    private static string folderNoABPath = "Assets/Res/NoBuildAB/Atlas";

    private static string atlasPath;

    [MenuItem("Assets/图片设置/自动生成图集", false, 1)]
    public static void UpdateAtlas()
    {
        string selectPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

        if (!Directory.Exists(selectPath))
        {
            Debug.LogError("该文件不是文件夹");
            return;
        }

        GetAtlasPath(selectPath);
        CreateFloder();
        ScanFloader(selectPath);

//         CreateFloder();
// 
//         if (Directory.Exists(imagePath))
//         {
//             DirectoryInfo directory = new DirectoryInfo(imagePath);
//             FileInfo[] files = directory.GetFiles();
//             for (int i = 0; i < files.Length; i++)
//             {
//                 string path = FileHelper.AbsoluteSwitchRelativelyPath(files[i].FullName).Replace(".meta", "");
// 
//                 if (files[i].Name.Contains("Common"))
//                 {
//                     DirectoryInfo childDirectory = new DirectoryInfo(path);
//                     FileInfo[] childFiles = childDirectory.GetFiles();
//                     for (int j = 0; j < childFiles.Length; j++)
//                     {
//                         string commonName = childFiles[j].Name.Replace(".meta", "");
//                         string commonAtlasPath = folderPath + "/" + commonName + ".spriteatlas";
//                         CreateAtlas(commonAtlasPath, FileHelper.AbsoluteSwitchRelativelyPath(childFiles[j].FullName).Replace(".meta", ""));
//                     }
//                     continue;
//                 }
//                 string name = files[i].Name.Replace(".meta", "");
//                 string atlasPath = folderPath + "/" + name + ".spriteatlas";
//                 CreateAtlas(atlasPath, imagePath + "/" + name);
//             }
//         }
//         AssetDatabase.SaveAssets();
    }

    //获取图集文件夹的路径
    public static void GetAtlasPath(string path)
    {
        if (path.Contains("NoBuildAB"))
        {
            atlasPath = folderNoABPath;
        }
        else
        {
            atlasPath = folderABPath;
        }
    }

    //递归判断是否含有子文件夹
    public static void ScanFloader(string path)
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles();
        if (files.Length > 0)
        {
            if (IsChildHaveFloder(files))
            {      
                for(int i = 0; i < files.Length; i++)
                {
                    ScanFloader(FileHelper.AbsoluteSwitchRelativelyPath(files[i].FullName).Replace(".meta", ""));
                }
            }
            else
            {
                string childPath = files[0].Name;
                if (childPath.EndsWith(".png") || childPath.EndsWith(".jpg"))
                {
                     Debug.Log(directory.Name);
                     string curPath = path.Replace(".meta", "");
                     string curAtlasPath = atlasPath + "/" + directory.Name + ".spriteatlas";                  
                     CreateAtlas(curAtlasPath, curPath);
                }
                else
                {
                    Debug.LogError(path + "该文件夹不是图片文件夹");
                }
            }
        }
    }


    public static bool IsChildHaveFloder(FileInfo[] files)
    {
        for (int i = 0; i < files.Length; i++) {
            string filePath = FileHelper.AbsoluteSwitchRelativelyPath(files[i].FullName).Replace(".meta", "");
            if (Directory.Exists(filePath)) {
                return true;
            }
        }
        return false;
    }

    public static void CreateFloder()
    {
        if (Directory.Exists(atlasPath))
        {
            return;
        }
        AssetDatabase.CreateFolder(atlasPath.Replace("/Atlas", ""), "Atlas");
    }

    public static void CreateAtlas(string atlasPath, string imagePath)
    {
        if (File.Exists(atlasPath))
        {
            return;   
        }

        SpriteAtlas spriteAtlas = new SpriteAtlas();
        AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
        Object obj = AssetDatabase.LoadAssetAtPath(imagePath, typeof(Object));
        spriteAtlas.Add(new[] { obj });      
    }
}