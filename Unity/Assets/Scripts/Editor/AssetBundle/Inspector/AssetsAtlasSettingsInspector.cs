using Model;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;


[CustomEditor(typeof(AssestSpriteSettings))]
public class AssetsAtlasSettingsInspector : Editor
{
    private AssestSpriteSettings m_target;
    private bool atlasPathData;

    private void OnEnable()
    {
        this.m_target = (AssestSpriteSettings)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);
        m_target.atlasFile = EditorGUILayout.ObjectField(m_target.atlasFile, typeof(Object), false);
        if (GUILayout.Button("保存"))
        {
            AssetDatabase.SaveAssets();
        }
        GUILayout.Space(20);
        if (GUILayout.Button("导出"))
        {
            Export();
        }
        GUI.enabled = false;

        this.DrawDefaultInspector();

        atlasPathData = EditorGUILayout.Foldout(atlasPathData, "图集路径");
        if (atlasPathData)
        {

        }

    }
    private void Export()
    {
        m_target.atlasNameList.Clear();
        m_target.atlasPathList.Clear();
        m_target.imageNameList.Clear();
        m_target.atlasCommonPathList.Clear();
        string filePath = AssetDatabase.GetAssetPath(m_target.atlasFile);
        DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
        FileInfo[] fileInfo = directoryInfo.GetFiles();
        for(int i = 0; i<fileInfo.Length; i++)
        {
            if (fileInfo[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            string extensionName = fileInfo[i].Extension;
            string name = fileInfo[i].Name.Replace(extensionName, "");
            string atlasPath = FileHelper.AbsoluteSwitchRelativelyPath(fileInfo[i].FullName);
            Object atlasObj = AssetDatabase.LoadAssetAtPath(atlasPath, typeof(Object));

            m_target.atlasNameList.Add(name);
            m_target.atlasPathList.Add(atlasPath.Replace(fileInfo[i].Extension, ""));
     
            if (name.Contains("Common"))
            {
                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(FileHelper.AbsoluteSwitchRelativelyPath(fileInfo[i].FullName), typeof(SpriteAtlas)) as SpriteAtlas;
                Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
                spriteAtlas.GetSprites(sprites);
                for(int j = 0; j < spriteAtlas.spriteCount; j++)
                {
                    m_target.imageNameList.Add(sprites[j].name.Replace("(Clone)", ""));
                    m_target.atlasCommonPathList.Add(atlasPath.Replace(fileInfo[i].Extension, ""));
                }
            }
        }
        EditorUtility.SetDirty(m_target);
        AssetDatabase.SaveAssets();
    }
}