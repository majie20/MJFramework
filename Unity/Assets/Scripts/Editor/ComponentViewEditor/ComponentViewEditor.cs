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
using Component = Model.Component;

[InitializeOnLoad]
[CustomEditor(typeof(ComponentView))]
public class ComponentViewEditor : Editor
{
    private string _text = "";

    private static ComponentViewEditor instance;
    private static int                 time;

    public override void OnInspectorGUI()
    {
        instance = this;
        this._text = EditorGUILayout.TextField(_text);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical();

        ComponentView componentView = (ComponentView)target;
        var dic = componentView.dic;

        foreach (var v in dic)
        {
            if (Regex.IsMatch(v.Value.FullName.ToLower(), this._text.ToLower()))
            {
                ComponentViewHelper.Draw(v.Key, v.Value);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void OnDestroy()
    {
        instance = null;
        ComponentViewHelper.Init();
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

[InitializeOnLoad]
public static class ComponentViewHelper
{
    public static readonly List<ITypeDrawer>           TypeDrawers = new();
    private static         Dictionary<Component, bool> Map         = new();

    static ComponentViewHelper()
    {
        Init();
    }

    public static void Init()
    {
        Map.Clear();
        TypeDrawers.Clear();
        Assembly assembly = typeof(ComponentViewHelper).Assembly;

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsDefined(typeof(TypeDrawerAttribute)))
            {
                continue;
            }

            ITypeDrawer iTypeDrawer = (ITypeDrawer)Activator.CreateInstance(type);
            TypeDrawers.Add(iTypeDrawer);
        }
    }

    public static void Draw(Model.Component obj, Type type)
    {
        try
        {
            Map.TryGetValue(obj, out var b);
            b = EditorGUILayout.Foldout(b, $"{type.FullName}:");
            Map[obj] = b;

            if (b)
            {
                return;
            }

            //EditorGUILayout.LabelField($"{type.FullName}:", new GUIStyle { fontSize = 12 });
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            EditorGUI.indentLevel++;

#if ILRuntime
            if (obj is CrossBindingAdaptorType instance)
            {
                foreach (FieldInfo fieldInfo in fields)
                {
                    Type fieldType = fieldInfo.FieldType;

                    if (fieldType.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    ITypeDrawer typeDrawer1 = null;

                    for (int i = 0; i < TypeDrawers.Count; i++)
                    {
                        var typeDrawer = TypeDrawers[i];

                        if (!typeDrawer.HandlesType(fieldType))
                        {
                            continue;
                        }

                        typeDrawer1 = typeDrawer;

                        break;
                    }

                    if (typeDrawer1 == null)
                    {
                        typeDrawer1 = new ObjectTypeDrawer();
                    }

                    string fieldName = fieldInfo.Name;
                    object value = fieldInfo.GetValue(instance.ILInstance);
                    typeDrawer1.DrawAndGetNewValue(fieldType, fieldName, value, null);
                }
            }
            else
            {
#endif
            foreach (FieldInfo fieldInfo in fields)
            {
                Type fieldType = fieldInfo.FieldType;

                if (fieldType.IsDefined(typeof(HideInInspector), false))
                {
                    continue;
                }

                if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                {
                    continue;
                }

                ITypeDrawer typeDrawer1 = null;

                for (int i = 0; i < TypeDrawers.Count; i++)
                {
                    var typeDrawer = TypeDrawers[i];

                    if (!typeDrawer.HandlesType(fieldType))
                    {
                        continue;
                    }

                    typeDrawer1 = typeDrawer;

                    break;
                }

                if (typeDrawer1 == null)
                {
                    typeDrawer1 = new ObjectTypeDrawer();
                }

                string fieldName = fieldInfo.Name;
                object value = fieldInfo.GetValue(obj);
                typeDrawer1.DrawAndGetNewValue(fieldType, fieldName, value, null);
            }
#if ILRuntime
            }
#endif
            EditorGUI.indentLevel--;
        }
        catch (Exception e)
        {
            Debug.LogError($"component view error: {type.FullName}+==>{e}"); // MDEBUG:
        }
    }
}