using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComponentView))]
public class ComponentViewEditor : Editor
{
    private string text = "";

    public override void OnInspectorGUI()
    {
        this.text = EditorGUILayout.TextField(text);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical();

        ComponentView componentView = (ComponentView)target;
        var dic = componentView.dic;

        foreach (var v in dic)
        {
            if (Regex.IsMatch(v.Value.FullName.ToLower(), this.text.ToLower()))
            {
                ComponentViewHelper.Draw(v);
            }
        }

        EditorGUILayout.EndVertical();
    }
}

public static class ComponentViewHelper
{
    public static readonly List<ITypeDrawer> typeDrawers = new List<ITypeDrawer>();

    static ComponentViewHelper()
    {
        Assembly assembly = typeof(ComponentViewHelper).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsDefined(typeof(TypeDrawerAttribute)))
            {
                continue;
            }

            ITypeDrawer iTypeDrawer = (ITypeDrawer)Activator.CreateInstance(type);
            typeDrawers.Add(iTypeDrawer);
        }
    }

    public static void Draw(KeyValuePair<Model.Component, Type> obj)
    {
        try
        {
            EditorGUILayout.LabelField($"{obj.Value.FullName}:", new GUIStyle { fontSize = 12 });
            FieldInfo[] fields = obj.Value.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (obj.Key is CrossBindingAdaptorType instance)
            {
                Type tempType = null;
                if (obj.Key is ComponentAdapter.Adapter)
                {
                    tempType = typeof(Model.Component);
                }

                if (tempType == null)
                {
                    return;
                }

                FieldInfo[] tempFields = tempType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                foreach (FieldInfo fieldInfo in tempFields)
                {
                    Type type = fieldInfo.FieldType;
                    if (type.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.Name == "awakeCalled" || fieldInfo.Name == "called")
                    {
                        continue;
                    }

                    for (int i = 0; i < typeDrawers.Count; i++)
                    {
                        var typeDrawer = typeDrawers[i];

                        if (!typeDrawer.HandlesType(type))
                        {
                            continue;
                        }

                        string fieldName = fieldInfo.Name;
                        object value = fieldInfo.GetValue(obj.Key);
                        typeDrawer.DrawAndGetNewValue(type, fieldName, value, null);

                        break;
                    }
                }

                foreach (FieldInfo fieldInfo in fields)
                {
                    Type type = fieldInfo.FieldType;
                    if (type.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.Name == "isInvokingToString")
                    {
                        continue;
                    }

                    if (type == typeof(ILTypeInstance) || type == typeof(ILRuntime.Runtime.Enviorment.AppDomain) || type == typeof(CrossBindingMethodInfo))
                    {
                        continue;
                    }

                    var result = false;
                    foreach (FieldInfo info in tempFields)
                    {
                        if (fieldInfo.Name == info.Name)
                        {
                            result = true;
                            break;
                        }
                    }

                    if (result)
                    {
                        continue;
                    }

                    for (int i = 0; i < typeDrawers.Count; i++)
                    {
                        var typeDrawer = typeDrawers[i];

                        if (!typeDrawer.HandlesType(type))
                        {
                            continue;
                        }

                        string fieldName = fieldInfo.Name;
                        object value = fieldInfo.GetValue(instance.ILInstance);
                        typeDrawer.DrawAndGetNewValue(type, fieldName, value, null);

                        break;
                    }
                }
            }
            else
            {
                foreach (FieldInfo fieldInfo in fields)
                {
                    Type type = fieldInfo.FieldType;
                    if (type.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.Name == "awakeCalled" || fieldInfo.Name == "called")
                    {
                        continue;
                    }

                    for (int i = 0; i < typeDrawers.Count; i++)
                    {
                        var typeDrawer = typeDrawers[i];

                        if (!typeDrawer.HandlesType(type))
                        {
                            continue;
                        }

                        string fieldName = fieldInfo.Name;
                        object value = fieldInfo.GetValue(obj.Key);
                        typeDrawer.DrawAndGetNewValue(type, fieldName, value, null);

                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"component view error: {obj.Value.FullName}+==>{e}"); // MDEBUG:
        }
    }
}