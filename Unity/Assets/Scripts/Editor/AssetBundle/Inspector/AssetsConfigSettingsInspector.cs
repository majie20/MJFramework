using Model;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(AssetsConfigSettings))]
public class AssetsConfigSettingsInspector : Editor
{
    private AssetsConfigSettings m_target;

    /// <summary>
    /// 滑动条位置
    /// </summary>
    private Vector2 m_ScrollPosition;

    private void OnEnable()
    {
        this.m_target = (AssetsConfigSettings)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        var settings = EditorGUILayout.ObjectField(m_target.Settings, typeof(Object), false);
        if (settings as AssetsCiteMatchConfigSettings || settings == null)
        {
            m_target.Settings = (AssetsCiteMatchConfigSettings)settings;
        }

        var fileDirABList = m_target.FileDirABList;
        if (GUILayout.Button("添加新的资源引用配置"))
        {
            fileDirABList.Add(new AssetsCiteMatchConfig());
        }

        GUILayout.Space(10);

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

        for (int i = 0; i < fileDirABList.Count; i++)
        {
            GUILayout.Label($"{i}-----------------------------------------------------");
            GUILayout.BeginHorizontal();
            GUILayout.Label("扩展名:", new GUIStyle { alignment = TextAnchor.MiddleLeft, fixedWidth = 100 });
            fileDirABList[i].Extension = EditorGUILayout.TextField(fileDirABList[i].Extension);
            if (GUILayout.Button("删除配置"))
            {
                fileDirABList.RemoveAt(i);
                continue;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加资源引用地址"))
            {
                fileDirABList[i].DirList.Add(null);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            for (int j = 0; j < fileDirABList[i].DirList.Count; j++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("文件夹:", new GUIStyle { alignment = TextAnchor.LowerLeft, fixedWidth = 100 });
                Object dir = EditorGUILayout.ObjectField(fileDirABList[i].DirList[j], typeof(Object), false);

                if (Directory.Exists(AssetDatabase.GetAssetPath(dir)) || dir == null)
                {
                    fileDirABList[i].DirList[j] = dir;
                }

                if (GUILayout.Button("X"))
                {
                    fileDirABList[i].DirList.RemoveAt(j);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }
        GUILayout.Space(20);

        EditorUtility.SetDirty(target);
        if (GUILayout.Button("保存"))
        {
            AssetDatabase.SaveAssets();
        }
        GUILayout.Space(20);
        if (GUILayout.Button("导出"))
        {
            Export(m_target);
        }

        EditorGUILayout.EndScrollView();  //结束 ScrollView 窗口
    }

    private static List<string> PathList;
    private static List<Object> AssetsList;
    public static void Export(AssetsConfigSettings m_target)
    {
        PathList = new List<string>();
        AssetsList = new List<Object>();
        for (int i = 0; i < m_target.FileDirABList.Count; i++)
        {
            var config = m_target.FileDirABList[i];
            for (int j = 0; j < config.DirList.Count; j++)
            {
                AddResourcePath(AssetDatabase.GetAssetPath(config.DirList[j]), config);
            }
        }

        m_target.Settings.PathList = PathList;
        m_target.Settings.AssetsList = AssetsList;
        EditorUtility.SetDirty(m_target.Settings);
        AssetDatabase.SaveAssets();
    }

    private static void AddResourcePath(string path, AssetsCiteMatchConfig config)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
        foreach (FileSystemInfo info in fileInfo)
        {
            //判断是否文件夹
            if (info is DirectoryInfo)
            {
                AddResourcePath(info.FullName, config);
            }
            else
            {
                if (info.Extension != ".meta")
                {
                    if (string.IsNullOrEmpty(config.Extension) || config.Extension.Contains(info.Extension))
                    {
                        var p = FileHelper.AbsoluteSwitchRelativelyPath(info.FullName);
                        PathList.Add(Regex.Replace(p, FileValue.FILE_EXTENSION_PATTERN, ""));
                        AssetsList.Add(AssetDatabase.LoadMainAssetAtPath(p));
                    }
                }
            }
        }
    }
}