//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

////public class ModelUtils : AssetPostprocessor
////{
////    private void OnPreprocessModel()
////    {
////        if (assetImporter is ModelImporter model)
////        {
////            var fileName = Path.GetFileNameWithoutExtension(assetPath);
////            var jsonPath = $"./Assets/StreamingAssets/TXT/{fileName}.json";
////            if (!File.Exists(jsonPath))
////            {
////                return;
////            }

////            var clipList = new List<ModelImporterClipAnimation>();

////            var text = File.ReadAllText(jsonPath);

////            using (var sr = new StringReader(text))
////            {
////                var reader = new JsonTextReader(sr);
////                var obj = JToken.ReadFrom(reader);

////                clipList.AddRange(obj
////                    .Select(info => new ModelImporterClipAnimation
////                    {
////                        name = info["EnglishName"].ToString(),
////                        firstFrame = float.Parse(info["beginNum"].ToString()),
////                        lastFrame = float.Parse(info["endNum"].ToString()),
////                        loopTime = int.Parse(info["loop"].ToString()) == 1
////                    }));
////            }

////            model.clipAnimations = clipList.ToArray();
////        }
////    }
////}

//public class ModelEditor : EditorWindow
//{
//    //[MenuItem("Tools/UtilsEditor/ModelEditorPanel")]
//    public static void OpenModelUtilsPanel()
//    {
//        CreateInstance<ModelEditor>().Show();
//    }

//    [SerializeField]
//    public GameObject originObj;

//    [SerializeField]
//    public List<GameObject> targetObjs1 = new List<GameObject>();

//    [SerializeField]
//    public List<GameObject> targetObjs2 = new List<GameObject>();

//    protected SerializedObject serObj;
//    protected SerializedProperty originSerPro;
//    protected SerializedProperty targetSerPro1;
//    protected SerializedProperty targetSerPro2;

//    private void Awake()
//    {
//        titleContent = new GUIContent("模型处理实用工具");
//        //使用当前类初始化
//        serObj = new SerializedObject(this);
//        //获取当前类中可序列话的属性
//        originSerPro = serObj.FindProperty("originObj");
//        targetSerPro1 = serObj.FindProperty("targetObjs1");
//        targetSerPro2 = serObj.FindProperty("targetObjs2");
//    }

//    private void OnGUI()
//    {
//        serObj.Update();

//        //绘制标题
//        GUILayout.Space(10);
//        GUILayout.Label("模型处理实用工具", new GUIStyle { fontSize = 24, alignment = TextAnchor.MiddleCenter });
//        GUILayout.Space(10);

//        EditorGUI.BeginChangeCheck();

//        GUILayout.Label("处理模型：", new GUIStyle { alignment = TextAnchor.MiddleLeft });
//        EditorGUILayout.PropertyField(targetSerPro1, true);
//        GUILayout.Space(10);
//        if (GUILayout.Button("开始"))
//        {
//            ModelDispose();
//        }
//        if (GUILayout.Button("清理"))
//        {
//            targetObjs1 = new List<GameObject>();
//        }
//        GUILayout.Space(10);
//        GUILayout.Label("复制动画事件：", new GUIStyle { alignment = TextAnchor.MiddleLeft });
//        EditorGUILayout.PropertyField(originSerPro, true);
//        EditorGUILayout.PropertyField(targetSerPro2, true);
//        if (GUILayout.Button("开始"))
//        {
//            CopyEventToTargetModel();
//        }
//        if (GUILayout.Button("清理"))
//        {
//            originObj = null;
//            targetObjs2 = new List<GameObject>();
//        }

//        //结束检查是否有修改
//        if (EditorGUI.EndChangeCheck())
//        {
//            //提交修改
//            serObj.ApplyModifiedProperties();
//        }
//    }

//    private void ModelDispose()
//    {
//        for (int i = 0; i < targetObjs1.Count; i++)
//        {
//            var assetPath = AssetDatabase.GetAssetPath(targetObjs1[i]);

//            if (AssetImporter.GetAtPath(assetPath) is ModelImporter model)
//            {
//                model.animationType = ModelImporterAnimationType.Human;
//                model.animationCompression = ModelImporterAnimationCompression.Optimal;
//                //model.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;

//                var fileName = Path.GetFileNameWithoutExtension(assetPath);
//                var jsonPath = $"./Assets/StreamingAssets/TXT/{fileName}.json";
//                if (!File.Exists(jsonPath))
//                {
//                    return;
//                }

//                var clipList = new List<ModelImporterClipAnimation>();

//                //using (FileStream fs = File.OpenRead(jsonPath))
//                //{
//                //    using (var sr = new StreamReader(fs))
//                //    {
//                //        var reader = new JsonTextReader(sr);
//                //        var obj = JToken.ReadFrom(reader);

//                //        clipList.AddRange(obj
//                //            .Select(info => new ModelImporterClipAnimation
//                //            {
//                //                name = info["EnglishName"].ToString(),
//                //                firstFrame = float.Parse(info["beginNum"].ToString()),
//                //                lastFrame = float.Parse(info["endNum"].ToString()),
//                //                loopTime = int.Parse(info["loop"].ToString()) == 1
//                //            }));
//                //    }
//                //}

//                model.clipAnimations = clipList.ToArray();

//                model.SaveAndReimport();
//            }
//        }
//    }

//    private void CopyEventToTargetModel()
//    {
//        Dictionary<string, SerializedProperty> copyClips = null;

//        for (int i = 0; i < targetObjs2.Count; i++)
//        {
//            var assetPath = AssetDatabase.GetAssetPath(targetObjs2[i]);

//            if (AssetImporter.GetAtPath(assetPath) is ModelImporter model)
//            {
//                SerializedObject modelImporterObj = new SerializedObject(model);
//                SerializedProperty rootNodeProperty = modelImporterObj.FindProperty("m_ClipAnimations");

//                if (originObj != null && rootNodeProperty != null && rootNodeProperty.isArray)
//                {
//                    if (copyClips == null)
//                        copyClips = GetModelAssetAnimClip(originObj);

//                    for (int j = 0; j < rootNodeProperty.arraySize; j++)
//                    {
//                        SerializedProperty item = rootNodeProperty.GetArrayElementAtIndex(j);

//                        var name = item.FindPropertyRelative("name").stringValue;
//                        if (copyClips.ContainsKey(name))
//                        {
//                            //var firstFrame1 = item.FindPropertyRelative("firstFrame").floatValue;
//                            //var lastFrame1 = item.FindPropertyRelative("lastFrame").floatValue;
//                            var originEvents = item.FindPropertyRelative("events");
//                            var copyItem = copyClips[name];
//                            var copyEvents = copyItem.FindPropertyRelative("events");
//                            //var firstFrame2 = copyItem.FindPropertyRelative("firstFrame").floatValue;
//                            //var lastFrame2 = copyItem.FindPropertyRelative("lastFrame").floatValue;

//                            if (originEvents.isArray && originEvents.arraySize > 0)
//                            {
//                                originEvents.ClearArray();
//                            }

//                            for (int k = 0; k < copyEvents.arraySize; k++)
//                            {
//                                originEvents.InsertArrayElementAtIndex(k);
//                                var copyEvent = copyEvents.GetArrayElementAtIndex(k);
//                                var functionName = copyEvent.FindPropertyRelative("functionName").stringValue;
//                                var time = copyEvent.FindPropertyRelative("time").floatValue;
//                                var data = copyEvent.FindPropertyRelative("data").stringValue;
//                                var intParameter = copyEvent.FindPropertyRelative("intParameter").intValue;

//                                var originEvent = originEvents.GetArrayElementAtIndex(k);
//                                originEvent.FindPropertyRelative("functionName").stringValue = functionName;
//                                originEvent.FindPropertyRelative("time").floatValue = time;/// (lastFrame2 - firstFrame2) * (lastFrame1 - firstFrame1) ;
//                                originEvent.FindPropertyRelative("intParameter").intValue = intParameter;
//                                originEvent.FindPropertyRelative("data").stringValue = data;
//                            }
//                        }
//                    }
//                }

//                modelImporterObj.ApplyModifiedProperties();
//                model.SaveAndReimport();
//            }
//        }
//    }

//    private Dictionary<string, SerializedProperty> GetModelAssetAnimClip(GameObject obj)
//    {
//        var path = AssetDatabase.GetAssetPath(obj);
//        var clipDic = new Dictionary<string, SerializedProperty>();

//        if (AssetImporter.GetAtPath(path) is ModelImporter model)
//        {
//            SerializedObject modelImporterObj = new SerializedObject(model);
//            SerializedProperty rootNodeProperty = modelImporterObj.FindProperty("m_ClipAnimations");
//            if (rootNodeProperty != null && rootNodeProperty.isArray)
//            {
//                for (int j = 0; j < rootNodeProperty.arraySize; j++)
//                {
//                    SerializedProperty item = rootNodeProperty.GetArrayElementAtIndex(j);
//                    clipDic.Add(item.FindPropertyRelative("name").stringValue, item);
//                }
//            }
//        }
//        return clipDic;
//    }
//}