using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PrefabAssociateData
{
    public GameObject obj;
    public List<PrefabCreateData> datas = new List<PrefabCreateData>();
}

public class PrefabAssociateEditor : EditorWindow
{
    public GameObject targetObj;
    public List<PrefabAssociateData> bornDatas = new List<PrefabAssociateData>();

    private SerializedObject serObj;
    private SerializedProperty targetSerPro;
    private SerializedProperty bornDatasSerPro;

    private Vector2 m_ScrollPosition;

    private bool isHint = false;
    private string hintContent = "";

    private void Awake()
    {
        titleContent = new GUIContent("预制体关联工具");
        //使用当前类初始化
        serObj = new SerializedObject(this);
        //获取当前类中可序列话的属性
        targetSerPro = serObj.FindProperty("targetObj");
        bornDatasSerPro = serObj.FindProperty("bornDatas");
    }

    private void OnGUI()
    {
        if (serObj == null)
        {
            return;
        }

        //绘制标题
        GUILayout.Space(10);
        GUILayout.Label("预制体关联工具", new GUIStyle { fontSize = 24, alignment = TextAnchor.MiddleCenter });
        GUILayout.Space(10);

        if (isHint)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("提示:", new GUIStyle { fontSize = 15, alignment = TextAnchor.MiddleLeft });
            GUILayout.Label(hintContent, new GUIStyle { fontSize = 15, alignment = TextAnchor.MiddleLeft });
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        GUILayout.Space(10);
        GUILayout.Label("目标预制体:", new GUIStyle { fontSize = 15, alignment = TextAnchor.MiddleLeft });
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        targetSerPro.objectReferenceValue = EditorGUILayout.ObjectField(targetSerPro.objectReferenceValue, typeof(GameObject), false);
        
        if (targetSerPro.objectReferenceValue == null)
        {
            serObj.ApplyModifiedProperties();
            serObj.UpdateIfRequiredOrScript();
            return;
        }
        else
        {
            if (targetSerPro.objectReferenceValue != targetObj)
            {
                bornDatasSerPro.ClearArray();
                bornDatas = new List<PrefabAssociateData>();
                var jsonPath = GetPrefabJsonDataPath(targetSerPro.objectReferenceValue);
                if (File.Exists(jsonPath))
                {
                    using (FileStream fs = File.OpenRead(jsonPath))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var reader = new JsonTextReader(sr);
                            var datas = JToken.ReadFrom(reader);

                            for (int i = 0; i < datas.Count(); i++)
                            {
                                AddAssociate(datas[i]);
                            }
                        }
                    }
                }
            }
        }

        if (GUILayout.Button("生成数据文件"))
        {
            using (FileStream fs = File.Create(GetPrefabJsonDataPath(targetObj)))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");
                    for (int i = 0; i < bornDatas.Count; i++)
                    {
                        var data1 = bornDatas[i];
                        if (data1.obj == null)
                            continue;
                        sb.Append("{");
                        sb.Append($"\"name\":\"{data1.obj.name}\",");
                        var path = AssetDatabase.GetAssetPath(data1.obj);
                        sb.Append($"\"guid\":\"{AssetDatabase.AssetPathToGUID(path)}\",");
                        //sb.Append($"\"path\":\"{path}\",");
                        sb.Append($"\"abName\":\"{AssetDatabase.GetImplicitAssetBundleName(path)}\",");
                        sb.Append("\"datas\":[");
                        for (int j = 0; j < data1.datas.Count; j++)
                        {
                            var data2 = data1.datas[j];
                            sb.Append("{");
                            sb.Append($"\"name\":\"{data2.name}\",");
                            sb.Append($"\"tag\":\"{data2.tag}\",");
                            sb.Append($"\"layer\":\"{data2.layer}\",");
                            sb.Append($"\"parentPath\":\"{data2.parentPath}\",");
                            sb.Append($"\"isDisplay\":{data2.isDisplay.ToString().ToLower()},");
                            sb.Append($"\"positionX\":{data2.position.x},");
                            sb.Append($"\"positionY\":{data2.position.y},");
                            sb.Append($"\"positionZ\":{data2.position.z},");
                            sb.Append($"\"rotationX\":{data2.rotation.x},");
                            sb.Append($"\"rotationY\":{data2.rotation.y},");
                            sb.Append($"\"rotationZ\":{data2.rotation.z},");
                            sb.Append($"\"scaleX\":{data2.scale.x},");
                            sb.Append($"\"scaleY\":{data2.scale.y},");
                            sb.Append($"\"scaleZ\":{data2.scale.z}");
                            if (j == data1.datas.Count - 1)
                            {
                                sb.Append("}");
                            }
                            else
                            {
                                sb.Append("},");
                            }
                        }
                        sb.Append("]");
                        if (i == bornDatas.Count - 1)
                        {
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append("},");
                        }
                    }
                    sb.Append("]");
                    sw.Write(sb);
                }
            }

            Debug.Log($"预制体[{targetObj.name}]生成数据文件成功!");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("要引用的预制体：", new GUIStyle { fontSize = 15, alignment = TextAnchor.MiddleLeft });
        GUILayout.Space(10);

        var list1 = new List<int>();
        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        for (int i = 0; i < bornDatas.Count; i++)
        {
            var data1 = bornDatasSerPro.GetArrayElementAtIndex(i);
            var obj = (GameObject)data1.FindPropertyRelative("obj").objectReferenceValue;
            var datas = data1.FindPropertyRelative("datas");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Object:", new GUIStyle { alignment = TextAnchor.LowerLeft });
            data1.FindPropertyRelative("obj").objectReferenceValue = EditorGUILayout.ObjectField(obj, typeof(GameObject), false);
            if (GUILayout.Button("添加"))
            {
                int index = datas.arraySize;
                datas.InsertArrayElementAtIndex(index);
                var element = datas.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("name").stringValue = obj.name;
                element.FindPropertyRelative("tag").stringValue = obj.tag;
                element.FindPropertyRelative("layer").stringValue = LayerMask.LayerToName(obj.layer);
                element.FindPropertyRelative("parentPath").stringValue = "";
                element.FindPropertyRelative("isDisplay").boolValue = true;
                element.FindPropertyRelative("position").vector3Value = Vector3.zero;
                element.FindPropertyRelative("rotation").vector3Value = Vector3.zero;
                element.FindPropertyRelative("scale").vector3Value = Vector3.one;
            }
            if (GUILayout.Button("删除"))
            {
                //将元素添加进删除list
                list1.Add(i);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            var list2 = new List<int>();
            for (int j = 0; j < datas.arraySize; j++)
            {
                var data2 = datas.GetArrayElementAtIndex(j);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                data2.FindPropertyRelative("name").stringValue = EditorGUILayout.TextField(data2.FindPropertyRelative("name").stringValue);
                GUILayout.Label("Tag:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                data2.FindPropertyRelative("tag").stringValue = EditorGUILayout.TextField(data2.FindPropertyRelative("tag").stringValue);
                GUILayout.Label("Layer:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                data2.FindPropertyRelative("layer").stringValue = EditorGUILayout.TextField(data2.FindPropertyRelative("layer").stringValue);
                GUILayout.Label("ParentPath:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                data2.FindPropertyRelative("parentPath").stringValue = EditorGUILayout.TextField(data2.FindPropertyRelative("parentPath").stringValue);
                GUILayout.Label("IsDisplay:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                data2.FindPropertyRelative("isDisplay").boolValue = EditorGUILayout.Toggle(data2.FindPropertyRelative("isDisplay").boolValue);
                if (GUILayout.Button("X"))
                {
                    //将元素添加进删除list
                    list2.Add(j);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                data2.FindPropertyRelative("position").vector3Value = EditorGUILayout.Vector3Field("Position:", data2.FindPropertyRelative("position").vector3Value);
                data2.FindPropertyRelative("rotation").vector3Value = EditorGUILayout.Vector3Field("Rotation:", data2.FindPropertyRelative("rotation").vector3Value);
                data2.FindPropertyRelative("scale").vector3Value = EditorGUILayout.Vector3Field("Scale:", data2.FindPropertyRelative("scale").vector3Value);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            //遍历删除list，将其删除掉
            foreach (var j in list2)
            {
                datas.DeleteArrayElementAtIndex(j);
            }
        }
        EditorGUILayout.EndScrollView();  //结束 ScrollView 窗口

        var eventType = Event.current.type;
        //在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var o in DragAndDrop.objectReferences)
                {
                    if (PrefabUtility.IsPartOfPrefabAsset(o) && o != targetObj)
                    {
                        var b = true;
                        for (int i = 0; i < bornDatas.Count; i++)
                        {
                            if (bornDatas[i].obj == o)
                            {
                                b = false;
                                break;
                            }
                        }

                        if (b)
                        {
                            var jsonPath = GetPrefabJsonDataPath(o);
                            if (File.Exists(jsonPath))
                            {
                                using (FileStream fs = File.OpenRead(jsonPath))
                                {
                                    using (var sr = new StreamReader(fs))
                                    {
                                        var reader = new JsonTextReader(sr);
                                        var datas = JToken.ReadFrom(reader);
                                        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetObj));
                                        for (int i = 0; i < datas.Count(); i++)
                                        {
                                            if (datas[i]["guid"].ToString() == guid)
                                            {
                                                b = false;
                                                break;
                                            }
                                        }
                                        if (b)
                                            AddAssociate((GameObject)o);
                                    }
                                }
                            }
                            else
                                AddAssociate((GameObject)o);
                        }
                    }
                }
            }

            Event.current.Use();
        }

        //遍历删除list，将其删除掉
        foreach (var i in list1)
        {
            bornDatasSerPro.DeleteArrayElementAtIndex(i);
        }

        serObj.ApplyModifiedProperties();
        serObj.UpdateIfRequiredOrScript();
    }

    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="obj"></param>
    private SerializedProperty AddAssociate(GameObject obj)
    {
        int index = bornDatasSerPro.arraySize;
        bornDatasSerPro.InsertArrayElementAtIndex(index);
        var element = bornDatasSerPro.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("obj").objectReferenceValue = obj;
        element.FindPropertyRelative("datas").ClearArray();
        return element;
    }

    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="jobj"></param>
    private void AddAssociate(JToken jobj)
    {
        var sp = AddAssociate(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(jobj["guid"].ToString())));
        var spDatas = sp.FindPropertyRelative("datas");
        var datas = jobj["datas"];
        for (int i = 0; i < datas.Count(); i++)
        {
            var d = datas[i];

            int index = spDatas.arraySize;
            spDatas.InsertArrayElementAtIndex(index);
            var element = spDatas.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = d["name"].ToString();
            element.FindPropertyRelative("tag").stringValue = d["tag"].ToString();
            element.FindPropertyRelative("layer").stringValue = d["layer"].ToString();
            element.FindPropertyRelative("parentPath").stringValue = d["parentPath"].ToString();
            element.FindPropertyRelative("isDisplay").boolValue = bool.Parse(d["isDisplay"].ToString());
            element.FindPropertyRelative("position").vector3Value = new Vector3(float.Parse(d["positionX"].ToString()), float.Parse(d["positionY"].ToString()), float.Parse(d["positionZ"].ToString()));
            element.FindPropertyRelative("rotation").vector3Value = new Vector3(float.Parse(d["rotationX"].ToString()), float.Parse(d["rotationY"].ToString()), float.Parse(d["rotationZ"].ToString()));
            element.FindPropertyRelative("scale").vector3Value = new Vector3(float.Parse(d["scaleX"].ToString()), float.Parse(d["scaleY"].ToString()), float.Parse(d["scaleZ"].ToString()));
        }
    }

    /// <summary>
    /// 获取预制体的json数据路径
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private string GetPrefabJsonDataPath(Object obj)
    {
        var path = $"./{AssetDatabase.GetAssetPath(obj)}";
        var dir = Path.GetDirectoryName(path);
        var name = Path.GetFileNameWithoutExtension(path);
        return $"{dir}/{name}.json";
    }
}