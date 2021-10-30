using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PrefabAssociateEditorData
{
    public GameObject obj;

    public List<PrefabCreateEditorData> datas = new List<PrefabCreateEditorData>();
}

[Serializable]
public class PrefabCreateEditorData
{
    public GameObject createObj;
    public string name;
    public string tag;
    public string layer;
    public string parentPath;
    public bool isDisplay;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
}

public class PrefabCreateJsonEditorData
{
    public GameObject obj;
    public SerializedProperty sp;
    public int index;
    public string name;
    public string tag;
    public string layer;
    public string parentPath;
    public bool isDisplay;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;

    public PrefabCreateJsonEditorData(GameObject obj, SerializedProperty sp, int index, string name, string tag, string layer,
        string parentPath, bool isDisplay, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
    {
        this.obj = obj;
        this.sp = sp;
        this.index = index;
        this.name = name;
        this.tag = tag;
        this.layer = layer;
        this.parentPath = parentPath;
        this.isDisplay = isDisplay;
        this.localPosition = localPosition;
        this.localEulerAngles = localEulerAngles;
        this.localScale = localScale;
    }
}

public class PrefabAssociateEditor : EditorWindow
{
    public GameObject targetObj;
    public List<PrefabAssociateEditorData> paDatas = new List<PrefabAssociateEditorData>();

    private SerializedObject serObj;
    private SerializedProperty targetSerPro;
    private SerializedProperty paDatasSerPro;

    /// <summary>
    /// 滑动条位置
    /// </summary>
    private Vector2 m_ScrollPosition;

    //提示相关
    private bool isHint = false;

    private string hintContent = "";

    /// <summary>
    /// 要操作的游戏物体
    /// </summary>
    private GameObject instanceTargetObj;

    private void Awake()
    {
        titleContent = new GUIContent("预制体关联工具");
        //使用当前类初始化
        serObj = new SerializedObject(this);
        //获取当前类中可序列话的属性
        targetSerPro = serObj.FindProperty("targetObj");
        paDatasSerPro = serObj.FindProperty("paDatas");
    }

    private void OnDestroy()
    {
        UnityEngine.Object.DestroyImmediate(instanceTargetObj);
    }

    private void OnInspectorUpdate()
    {
        if (serObj == null)
        {
            return;
        }
        this.Repaint();
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
                UnityEngine.Object.DestroyImmediate(instanceTargetObj);
                Transform parentTran = null;
                if (((GameObject)targetSerPro.objectReferenceValue).GetComponent<RectTransform>() != null)
                {
                    parentTran = GameObject.FindObjectOfType<Canvas>().transform;
                }
                instanceTargetObj = PrefabUtility.InstantiatePrefab(targetSerPro.objectReferenceValue, parentTran) as GameObject;
                ReadDataFile();
            }
        }
        GenerateDataFile();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("要引用的预制体：", new GUIStyle { fontSize = 15, alignment = TextAnchor.MiddleLeft });
        GUILayout.Space(10);

        var list1 = new List<int>();
        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        for (int i = 0; i < paDatas.Count; i++)
        {
            var data1 = paDatasSerPro.GetArrayElementAtIndex(i);
            var objSerPro = data1.FindPropertyRelative("obj");
            var obj = (GameObject)objSerPro.objectReferenceValue;
            var datas = data1.FindPropertyRelative("datas");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Object:", new GUIStyle { alignment = TextAnchor.LowerLeft });
            objSerPro.objectReferenceValue = EditorGUILayout.ObjectField(obj, typeof(GameObject), false);
            var dataObj = objSerPro.objectReferenceValue;
            if (paDatas[i].obj != null && dataObj != null && paDatas[i].obj != dataObj)
            {
                for (int j = 0; j < datas.arraySize; j++)
                {
                    var data = datas.GetArrayElementAtIndex(j);
                    var name = data.FindPropertyRelative("name").stringValue;
                    var tag = data.FindPropertyRelative("tag").stringValue;
                    var layer = data.FindPropertyRelative("layer").stringValue;
                    var parentPath = data.FindPropertyRelative("parentPath").stringValue;
                    var isDisplay = data.FindPropertyRelative("isDisplay").boolValue = true;
                    var localPosition = data.FindPropertyRelative("localPosition").vector3Value;
                    var localEulerAngles = data.FindPropertyRelative("localEulerAngles").vector3Value;
                    var localScale = data.FindPropertyRelative("localScale").vector3Value;
                    var o = data.FindPropertyRelative("createObj");
                    //删除这个预制体的克隆
                    UnityEngine.Object.DestroyImmediate(o.objectReferenceValue);

                    o.objectReferenceValue = CreateChildObj((GameObject)dataObj, name, tag, layer, parentPath, isDisplay, localPosition, localEulerAngles, localScale);
                }
            }

            if (GUILayout.Button("添加"))
            {
                if (obj != null)
                {
                    SetPrefabCreateSerializedProperty(datas, obj.name, obj.tag, LayerMask.LayerToName(obj.layer), "",
                        true, Vector3.zero, Vector3.zero, Vector3.one, obj);
                }
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
                var createObjSerPro = data2.FindPropertyRelative("createObj").objectReferenceValue as GameObject;
                var nameSerPro = data2.FindPropertyRelative("name");
                var tagSerPro = data2.FindPropertyRelative("tag");
                var layerSerPro = data2.FindPropertyRelative("layer");
                var parentPathSerPro = data2.FindPropertyRelative("parentPath");
                var isDisplaySerPro = data2.FindPropertyRelative("isDisplay");
                var localPositionSerPro = data2.FindPropertyRelative("localPosition");
                var localEulerAnglesSerPro = data2.FindPropertyRelative("localEulerAngles");
                var localScaleSerPro = data2.FindPropertyRelative("localScale");

                nameSerPro.stringValue = createObjSerPro.name;
                tagSerPro.stringValue = createObjSerPro.tag;
                layerSerPro.stringValue = LayerMask.LayerToName(createObjSerPro.layer);
                parentPathSerPro.stringValue = GetRoute(createObjSerPro.transform);
                isDisplaySerPro.boolValue = createObjSerPro.activeSelf;
                localPositionSerPro.vector3Value = createObjSerPro.transform.localPosition;
                localEulerAnglesSerPro.vector3Value = createObjSerPro.transform.localEulerAngles;
                localScaleSerPro.vector3Value = createObjSerPro.transform.localScale;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                EditorGUILayout.TextField(nameSerPro.stringValue);
                GUILayout.Label("Tag:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                EditorGUILayout.TextField(tagSerPro.stringValue);
                GUILayout.Label("Layer:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                EditorGUILayout.TextField(layerSerPro.stringValue);
                GUILayout.Label("ParentPath:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                EditorGUILayout.TextField(parentPathSerPro.stringValue);
                GUILayout.Label("IsDisplay:", new GUIStyle { alignment = TextAnchor.LowerLeft });
                EditorGUILayout.Toggle(isDisplaySerPro.boolValue);
                if (GUILayout.Button("X"))
                {
                    //将元素添加进删除list
                    list2.Add(j);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.Vector3Field("Position:", localPositionSerPro.vector3Value);
                EditorGUILayout.Vector3Field("Rotation:", localEulerAnglesSerPro.vector3Value);
                EditorGUILayout.Vector3Field("Scale:", localScaleSerPro.vector3Value);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            //遍历删除list，将其删除掉
            foreach (var j in list2)
            {
                //删除这个预制体的这项克隆
                UnityEngine.Object.DestroyImmediate(datas.GetArrayElementAtIndex(j).FindPropertyRelative("createObj").objectReferenceValue);
                datas.DeleteArrayElementAtIndex(j);
            }
        }
        EditorGUILayout.EndScrollView();  //结束 ScrollView 窗口

        //遍历删除list，将其删除掉
        foreach (var i in list1)
        {
            var datas = paDatasSerPro.GetArrayElementAtIndex(i).FindPropertyRelative("datas");
            for (int j = 0; j < datas.arraySize; j++)
            {
                //删除这个预制体的所有克隆
                UnityEngine.Object.DestroyImmediate(datas.GetArrayElementAtIndex(j).FindPropertyRelative("createObj").objectReferenceValue);
            }
            paDatasSerPro.DeleteArrayElementAtIndex(i);
        }

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
                        for (int i = 0; i < paDatas.Count; i++)
                        {
                            if (paDatas[i].obj == o)
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

        serObj.ApplyModifiedProperties();
        serObj.UpdateIfRequiredOrScript();
    }

    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="obj"></param>
    private SerializedProperty AddAssociate(GameObject obj)
    {
        int index = paDatasSerPro.arraySize;
        paDatasSerPro.InsertArrayElementAtIndex(index);
        var element = paDatasSerPro.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("obj").objectReferenceValue = obj;
        element.FindPropertyRelative("datas").ClearArray();
        return element;
    }

    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="jObj"></param>
    private void AddAssociate(JToken jObj, ref List<PrefabCreateJsonEditorData> pcJsonDatas)
    {
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(jObj["guid"].ToString()));
        var sp = AddAssociate(obj);
        var spDatas = sp.FindPropertyRelative("datas");
        var datas = jObj["datas"];
        for (int i = 0; i < datas.Count(); i++)
        {
            var d = datas[i];
            var index = int.Parse(d["index"].ToString());
            var name = d["name"].ToString();
            var tag = d["tag"].ToString();
            var layer = d["layer"].ToString();
            var parentPath = d["parentPath"].ToString();
            var isDisplay = bool.Parse(d["isDisplay"].ToString());
            var localPosition = new Vector3(float.Parse(d["localPositionX"].ToString()), float.Parse(d["localPositionY"].ToString()), float.Parse(d["localPositionZ"].ToString()));
            var localEulerAngles = new Vector3(float.Parse(d["localEulerAnglesX"].ToString()), float.Parse(d["localEulerAnglesY"].ToString()), float.Parse(d["localEulerAnglesZ"].ToString()));
            var localScale = new Vector3(float.Parse(d["localScaleX"].ToString()), float.Parse(d["localScaleY"].ToString()), float.Parse(d["localScaleZ"].ToString()));

            var e = SetPrefabCreateSerializedProperty(spDatas, name, tag, layer, parentPath, isDisplay,
                localPosition, localEulerAngles, localScale);

            pcJsonDatas.Add(new PrefabCreateJsonEditorData(obj, e.FindPropertyRelative("createObj"), index, name, tag, layer, parentPath, isDisplay, localPosition, localEulerAngles, localScale));
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

    /// <summary>
    /// 创建子物体
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="layer"></param>
    /// <param name="parentPath"></param>
    /// <param name="isDisplay"></param>
    /// <param name="localPosition"></param>
    /// <param name="localEulerAngles"></param>
    /// <param name="localScale"></param>
    /// <returns></returns>
    private GameObject CreateChildObj(GameObject source, string name, string tag, string layer, string parentPath, bool isDisplay, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
    {
        GameObject obj;
        if (string.IsNullOrEmpty(parentPath))
            obj = PrefabUtility.InstantiatePrefab(source, instanceTargetObj.transform) as GameObject;
        else
            obj = PrefabUtility.InstantiatePrefab(source, instanceTargetObj.transform.Find(parentPath)) as GameObject;
        obj.name = name;
        obj.tag = tag;
        obj.layer = LayerMask.NameToLayer(layer);
        obj.SetActive(isDisplay);
        obj.transform.localPosition = localPosition;
        obj.transform.localEulerAngles = localEulerAngles;
        obj.transform.localScale = localScale;
        return obj;
    }

    private string GetRoute(Transform tran)
    {
        var result = "";
        var parent = tran.parent;
        while (parent != null && parent != instanceTargetObj.transform)
        {
            if (result == "")
            {
                result = parent.name;
            }
            else
            {
                result = $"{parent.name}/{result}";
            }
            parent = parent.parent;
        }

        return result;
    }

    private SerializedProperty SetPrefabCreateSerializedProperty(SerializedProperty datas, string name, string tag, string layer,
        string parentPath, bool isDisplay, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale, GameObject obj = null)
    {
        int index = datas.arraySize;
        datas.InsertArrayElementAtIndex(index);
        var element = datas.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("name").stringValue = name;
        element.FindPropertyRelative("tag").stringValue = tag;
        element.FindPropertyRelative("layer").stringValue = layer;
        element.FindPropertyRelative("parentPath").stringValue = parentPath;
        element.FindPropertyRelative("isDisplay").boolValue = isDisplay;
        element.FindPropertyRelative("localPosition").vector3Value = localPosition;
        element.FindPropertyRelative("localEulerAngles").vector3Value = localEulerAngles;
        element.FindPropertyRelative("localScale").vector3Value = localScale;
        if (obj != null)
        {
            element.FindPropertyRelative("createObj").objectReferenceValue = CreateChildObj(obj, name, tag, layer, parentPath, isDisplay, localPosition, localEulerAngles, localScale);
        }

        return element;
    }

    private void GenerateDataFile()
    {
        if (GUILayout.Button("生成数据文件"))
        {
            if (paDatas.Count == 0)
            {
                Debug.LogWarning($"预制体[{targetObj.name}]没有数据要生成!");
            }
            else
            {
                var trans = instanceTargetObj.GetComponentsInChildren<Transform>(true);

                using (FileStream fs = File.Create(GetPrefabJsonDataPath(targetObj)))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        for (int i = 0; i < paDatas.Count; i++)
                        {
                            var data1 = paDatas[i];
                            if (data1.obj == null)
                                continue;
                            sb.Append("{");
                            sb.Append($"\"name\":\"{data1.obj.name}\",");
                            var path = AssetDatabase.GetAssetPath(data1.obj);
                            sb.Append($"\"guid\":\"{AssetDatabase.AssetPathToGUID(path)}\",");
                            sb.Append($"\"abName\":\"{AssetDatabase.GetImplicitAssetBundleName(path)}\",");
                            sb.Append("\"datas\":[");
                            for (int j = 0; j < data1.datas.Count; j++)
                            {
                                var data2 = data1.datas[j];
                                sb.Append("{");
                                for (int k = 1; k < trans.Length; k++)
                                {
                                    if (trans[k] == data2.createObj.transform)
                                    {
                                        sb.Append($"\"index\":{k},");
                                        break;
                                    }
                                }
                                sb.Append($"\"name\":\"{data2.name}\",");
                                sb.Append($"\"tag\":\"{data2.tag}\",");
                                sb.Append($"\"layer\":\"{data2.layer}\",");
                                sb.Append($"\"parentPath\":\"{data2.parentPath}\",");
                                sb.Append($"\"isDisplay\":{data2.isDisplay.ToString().ToLower()},");
                                sb.Append($"\"localPositionX\":{data2.localPosition.x},");
                                sb.Append($"\"localPositionY\":{data2.localPosition.y},");
                                sb.Append($"\"localPositionZ\":{data2.localPosition.z},");
                                sb.Append($"\"localEulerAnglesX\":{data2.localEulerAngles.x},");
                                sb.Append($"\"localEulerAnglesY\":{data2.localEulerAngles.y},");
                                sb.Append($"\"localEulerAnglesZ\":{data2.localEulerAngles.z},");
                                sb.Append($"\"localScaleX\":{data2.localScale.x},");
                                sb.Append($"\"localScaleY\":{data2.localScale.y},");
                                sb.Append($"\"localScaleZ\":{data2.localScale.z}");
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
                            if (i == paDatas.Count - 1)
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
                AssetDatabase.Refresh();
                Debug.Log($"预制体[{targetObj.name}]生成数据文件成功!");
            }
        }
    }

    private void ReadDataFile()
    {
        paDatasSerPro.ClearArray();
        paDatas = new List<PrefabAssociateEditorData>();
        var jsonPath = GetPrefabJsonDataPath(targetSerPro.objectReferenceValue);
        if (File.Exists(jsonPath))
        {
            var pcJsonDatas = new List<PrefabCreateJsonEditorData>();
            using (FileStream fs = File.OpenRead(jsonPath))
            {
                using (var sr = new StreamReader(fs))
                {
                    var reader = new JsonTextReader(sr);
                    var datas = JToken.ReadFrom(reader);

                    for (int i = 0; i < datas.Count(); i++)
                    {
                        AddAssociate(datas[i], ref pcJsonDatas);
                    }
                }
            }
            pcJsonDatas.Sort(((d1, d2) =>
            {
                if (d1.index < d2.index) return -1;
                return 1;
            }));

            for (int i = 0; i < pcJsonDatas.Count; i++)
            {
                var d = pcJsonDatas[i];
                d.sp.objectReferenceValue = CreateChildObj(d.obj, d.name, d.tag, d.layer, d.parentPath, d.isDisplay, d.localPosition, d.localEulerAngles, d.localScale);
            }
        }
    }
}