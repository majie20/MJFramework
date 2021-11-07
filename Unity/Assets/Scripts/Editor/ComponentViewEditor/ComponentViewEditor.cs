using Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComponentView))]
public class ComponentViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        ComponentView componentView = (ComponentView)target;
        var dic = componentView.dic;

        foreach (var v in dic)
        {
            ComponentViewHelper.Draw(v, componentView.isHotfix);
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

    public static void Draw(KeyValuePair<object, Type> obj, bool isHotfix)
    {
        try
        {
            EditorGUILayout.LabelField($"{obj.Value.FullName}:", new GUIStyle { fontSize = 12 });

            if (isHotfix)
            {
                return;
            }
            FieldInfo[] fields = obj.Value.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

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

                object value = fieldInfo.GetValue(obj.Key);

                for (int i = 0; i < typeDrawers.Count; i++)
                {
                    var typeDrawer = typeDrawers[i];

                    if (!typeDrawer.HandlesType(type))
                    {
                        continue;
                    }

                    string fieldName = fieldInfo.Name;
                    if (fieldName.Length > 17 && fieldName.Contains("k__BackingField"))
                    {
                        fieldName = fieldName.Substring(1, fieldName.Length - 17);
                    }
                    typeDrawer.DrawAndGetNewValue(type, fieldName, value, null);

                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"component view error: {obj.GetType().FullName}+==>{e}"); // MDEBUG:
        }
    }
}