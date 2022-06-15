using System.Text.RegularExpressions;

public class EditorConfig
{
    public static string PROJECT_PATH = Regex.Replace(System.IO.Path.GetFullPath("./../"), @"\\", "/");//项目根目录：Work/

    public const string RES_PATH = "Assets/Res/";
    public const string CODE_DIR_PATH = "Assets/Res/Text/";
    public const string ATLAS_PATH = "Assets/Res/Atlas/";
    public const string UI_SPRITE_PATH = "Assets/Res/Texture/UI/";
    public const string UI_PREFAB_PATH = "Assets/Res/Prefabs/UI/";
    public const string DYNAMIC_SPRITE_PATH = "Assets/Res/Texture/UI/DynamicSprite";


    public const string UI_Atlas = "Assets/Res/ResTemp/Template/Altas/UIAtlas.spriteatlas";
    public const string ASSETS_BUNDLE_SETTINGS_PATH = "Assets/Resources/AssetsBundleSettings.asset";
    public const string GAME_LOAD_COMPLETE_AC = "Assets/Res/ResTemp/ScriptableObject/AssetCollector/GameLoadComplete_AC.asset";
    public const string INIT_AC = "Assets/Res/ResTemp/ScriptableObject/AssetCollector/Init_AC.asset";
    public const string RES_TYPE_SETTINGS_PATH = "Assets/Res/ResTemp/ScriptableObject/ResTypeSettings.asset";
    public const string SPRITE_ATLAS_SETTINGS = "Assets/Res/ResTemp/ScriptableObject/SpriteAtlasSettings.asset";
    public const string UI_ATLAS_LINK_CONFIG_PATH = "Assets/Res/Config/ScriptableObject/UIPrefabToAtlasSettings.asset";
    public const string UI_Sprite_Info = "Assets/Res/Config/Json/UISpriteInfo.json";
    public const string UI_PREFAB_TO_ATLAS_INFO = "Assets/Res/Config/Json/UIPrefabToAtlasInfo.json";
}