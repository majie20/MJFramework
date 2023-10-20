using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[TypeDrawer]
public class ListTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().FullName == typeof(List<>).FullName;
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        var type = memberType.GetGenericArguments().First();
        var typeDrawers = ComponentViewHelper.typeDrawers;

        EditorGUILayout.LabelField($"{memberName}:");

        for (int i = 0; i < typeDrawers.Count; i++)
        {
            var typeDrawer = typeDrawers[i];
            if (!typeDrawer.HandlesType(type))
            {
                continue;
            }

            var _list = (value as IList);
            var index = 0;
            foreach (var o in _list)
            {
                typeDrawer.DrawAndGetNewValue(type, $"Element_{index}", o, null);
                index++;
            }

            break;
        }

        return null;
    }
}