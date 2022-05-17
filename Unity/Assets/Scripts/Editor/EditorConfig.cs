
using System.Text.RegularExpressions;

public class EditorConfig
{
    public static string PROJECT_PATH = Regex.Replace(System.IO.Path.GetFullPath("./../"), @"\\", "/");//项目根目录：Work/
    public static string DYNAMIC_SPRITE_PATH = "Assets/Res/Texture/UI/DynamicSprite";
    public static string UI_PREFAB_PATH = "Assets/Res/Prefabs/UI";
    public static string UI_PREFAB_TO_ATLAS_SETTINGS_PATH = "Assets/Res/Config/ScriptableObject/UIPrefabToAtlasSettings.asset";
    public static string ATLAS_PATH = "Assets/Res/Atlas";
    public static string ASSETS_BUNDLE_SETTINGS_PATH = "Assets/Resources/AssetsBundleSettings.asset";
}
