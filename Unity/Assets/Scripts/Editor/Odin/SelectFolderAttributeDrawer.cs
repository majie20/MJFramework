using Model;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

public class SelectFolderAttributeDrawer : OdinAttributeDrawer<SelectFolderAttribute>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (this.Attribute.TargetType.FullName == typeof(string).FullName)
        {
            if (GUILayout.Button("选择"))
            {

            }
            return;
        }

    }
}