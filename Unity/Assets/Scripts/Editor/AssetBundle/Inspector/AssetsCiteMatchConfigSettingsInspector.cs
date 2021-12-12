using UnityEditor;
using UnityEngine;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(AssetsCiteMatchConfigSettings))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
//[CanEditMultipleObjects]
public class AssetsCiteMatchConfigSettingsInspector : Editor
{
    private AssetsCiteMatchConfigSettings _src;

    private void OnEnable() => this._src = this.target as AssetsCiteMatchConfigSettings;

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        this.DrawDefaultInspector();
        GUI.enabled = true;
    }
}