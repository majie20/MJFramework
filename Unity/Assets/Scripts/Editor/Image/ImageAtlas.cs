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
    private static string folderPath = "Assets/ImageAtlas";
    private static string imagePath = "Assets/Image";

    [MenuItem("Tools/ImageAtlas/自动生成图集")]
    public static void UpdateAtlas()
    {
        CreateFloder();

        if (Directory.Exists(imagePath))
        {
            DirectoryInfo directory = new DirectoryInfo(imagePath);
            FileInfo[] files = directory.GetFiles();
            for(int i = 0; i < files.Length; i++)
            {
                string path = FileHelper.AbsoluteSwitchRelativelyPath(files[i].FullName).Replace(".meta", "");

                if (files[i].Name.Contains("Common"))
                {
                    DirectoryInfo childDirectory = new DirectoryInfo(path);
                    FileInfo[] childFiles = childDirectory.GetFiles();
                    for (int j = 0; j < childFiles.Length; j++)
                    {
                        string commonName = childFiles[j].Name.Replace(".meta", "");
                        string commonAtlasPath = folderPath + "/" + commonName + ".spriteatlas";
                        CreateAtlas(commonAtlasPath, FileHelper.AbsoluteSwitchRelativelyPath(childFiles[j].FullName).Replace(".meta", ""));             
                    }
                    continue;
                }
                string name = files[i].Name.Replace(".meta", "");
                string atlasPath = folderPath + "/" + name + ".spriteatlas";
                CreateAtlas(atlasPath, imagePath + "/" + name);
            }
        }
        AssetDatabase.SaveAssets();
    }

    public static void CreateFloder()
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
        AssetDatabase.CreateFolder("Assets", "ImageAtlas");
    }

    public static void CreateAtlas(string atlasPath, string imagePath)
    {
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
        Object obj = AssetDatabase.LoadAssetAtPath(imagePath, typeof(Object));
        spriteAtlas.Add(new[] { obj });      
    }
}