#if ILRuntime
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
#endif

using Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CustomEditor(typeof(ComponentView))]
public class ComponentViewEditor : Editor
{
    private string text = "";

    private static ComponentViewEditor instance;
    private static int                 time;

    public override void OnInspectorGUI()
    {
        instance = this;
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

    private void OnDestroy()
    {
        instance = null;
    }

    static ComponentViewEditor()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (instance != null)
        {
            if (++time > 10)
            {
                instance.Repaint();
                time = 0;
            }
        }
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
            FieldInfo[] fields = obj.Value.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

#if ILRuntime
            if (obj.Key is CrossBindingAdaptorType instance)
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
#else
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
#endif
        }
        catch (Exception e)
        {
            Debug.Log($"component view error: {obj.Value.FullName}+==>{e}"); // MDEBUG:
        }
    }
}