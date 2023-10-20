using System.Text.RegularExpressions;

public class EditorConst
{
    public static string PROJECT_PATH = Regex.Replace(System.IO.Path.GetFullPath("./../"), @"\\", "/"); //项目根目录：Work/

    public const string RES_PATH                 = "Assets/Res/";
    public const string CODE_DIR_PATH            = "Assets/Res/Text/";
    public const string UI_PREFAB_PATH           = "Assets/Res/UI/Prefab";
    public const string DYNAMIC_SPRITE_PATH      = "Assets/Res/UI/Texture/DynamicSprite";
    public const string UNIT_PREFAB_PATH         = "Assets/Res/Unit";
    public const string ENVIRONMENT_PATH         = "Assets/Res/Environment";
    public const string JSON_CONFIG              = "Assets/Res/Config/JsonConfig";
    public const string ASSET_COLLECTOR          = "Assets/Res/Config/ScriptableObject/AssetCollector";
    public const string PLUGINS_ILRUNTIME_       = "Assets/Plugins/ILRuntime~";
    public const string PLUGINS_ILRUNTIME        = "Assets/Plugins/ILRuntime";
    public const string THIRDPARTY_ILRUNTIME_    = "Assets/Scripts/ThirdParty/ILRuntime~";
    public const string THIRDPARTY_ILRUNTIME     = "Assets/Scripts/ThirdParty/ILRuntime";
    public const string WX_WEBGL_TEMPLATES_      = "Assets/WebGLTemplates~";
    public const string WX_WEBGL_TEMPLATES       = "Assets/WebGLTemplates";
    public const string WX_WASM_SDK_V2_          = "Assets/WX-WASM-SDK-V2~";
    public const string WX_WASM_SDK_V2           = "Assets/WX-WASM-SDK-V2";
    public const string BETTER_STREAMING_ASSETS_ = "Assets/Scripts/ThirdParty/BetterStreamingAssets~";
    public const string BETTER_STREAMING_ASSETS  = "Assets/Scripts/ThirdParty/BetterStreamingAssets";
    public const string FMOD_PATH_               = "Assets/Plugins/FMOD~";
    public const string FMOD_PATH                = "Assets/Plugins/FMOD";
    public const string BYTE_GAME_PATH_          = "Assets/Plugins/ByteGame~";
    public const string BYTE_GAME_PATH           = "Assets/Plugins/ByteGame";
    public const string STARK_MINI_UNITY_PATH_   = "Packages/com.bytedance.starkminiunity~";
    public const string STARK_MINI_UNITY_PATH    = "Packages/com.bytedance.starkminiunity";

    public const string AUDIO_PATH      = "Assets/Res/Audio/";
    public const string FMOD_BANKS_PATH = "Assets/Res/FMODBanks/";

    //public const string UNIT                  = "Assets/Res/Config/ScriptableObject/AssetReference/Unit";

    public const string ILBINDING      = "Assets/Scripts/Core/Generate/ILBinding";
    public const string HotfixFilePath = "Assets/Res/Text/Hotfix.dll.bytes";
    public const string IL_HELPER_CS   = "Assets/Scripts/Core/Helper/ILHelper.cs";

    public const string UI_Atlas                          = "Assets/Res/Template/Altas/UIAtlas.spriteatlas";
    public const string ASSETS_BUNDLE_SETTINGS_PATH       = "Assets/Resources/AssetsBundleSettings.asset";
    public const string ASSET_BUNDLE_BUILDER_SETTING_PATH = "Assets/YooAssetSetting/AssetBundleBuilderSetting.asset";
    public const string GAME_LOAD_COMPLETE_AC             = "Assets/Res/Config/ScriptableObject/AssetCollector/~GameLoadComplete_AC.asset";
    public const string INIT_AC                           = "Assets/Res/Config/ScriptableObject/AssetCollector/~Init_AC.asset";
    public const string RES_TYPE_SETTINGS_PATH            = "Assets/Res/Config/ScriptableObject/~ResTypeSettings.asset";
    public const string UI_SPRITE_ATLAS_SETTINGS          = "Assets/Res/UI/~SpriteAtlasSettings.asset";
    public const string UNIT_SPRITE_ATLAS_SETTINGS        = "Assets/Res/Unit/~SpriteAtlasSettings.asset";

    public const string UNIT_INFO_SETTINGS  = "Assets/Res/Config/ScriptableObject/UnitInfoSettings.asset";
    public const string STEP_SAVE_SETTINGS  = "Assets/Scripts/Editor/ProductionPipeline/StepSaveSettings.asset";
    public const string GAME_UNITY          = "Assets/Res/Scenes/Game.unity";
    public const string UNITY_HOTFIX_ASMDEF = "Assets/Scripts/Hotfix/Unity.Hotfix.asmdef";
    public const string GOTO_BUILD_MODE     = "Assets/Scripts/Editor/ProductionPipeline/GotoBuildMode.asset";
    public const string GOTO_DEVELOP_MODE   = "Assets/Scripts/Editor/ProductionPipeline/GotoDevelopMode.asset";
    public const string LINK_PATH           = "Assets/Scripts/link.xml";
}