using MGame.Model;
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
        var components = componentView.components;
        for (int i = 0; i < components.Count; i++)
        {
            ComponentViewHelper.Draw(components[i]);
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

    public static void Draw(MGame.Model.Component obj)
    {
        try
        {
            var _t = obj.GetType();

            FieldInfo[] fields = _t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            EditorGUILayout.LabelField($"{_t.FullName}:", new GUIStyle { fontSize = 15 });

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

                object value = fieldInfo.GetValue(obj);

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