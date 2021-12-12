using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.U2D;
using UnityEditor.U2D;

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
                SpriteAtlas spriteAtlas = new SpriteAtlas();
                string name = files[i].Name.Replace(".meta", "");
                AssetDatabase.CreateAsset(spriteAtlas, folderPath + "/" + name + ".spriteatlas");
                Object obj = AssetDatabase.LoadAssetAtPath(imagePath + "/" + name, typeof(Object));
                spriteAtlas.Add(new[] { obj });
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
}