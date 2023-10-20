namespace Model
{
    public class ConstData
    {
        public const string FILE_EXTENSION_PATTERN = @".[a-zA-Z]+\z"; //文件后缀匹配字符串

        public const string RES_PATH               = "Assets/Res/";
        public const string CODE_DIR_PATH          = "Assets/Res/Text/";
        public const string ENVIRONMENT_SCENE_PATH = "Assets/Res/Environment/Scene/";
        public const string ENVIRONMENT_LEVEL_PATH = "Assets/Res/Environment/Level/";
        public const string JSON_CONFIG_PATH       = "Assets/Res/Config/JsonConfig/";
        public const string SOUND_PATH             = "Assets/Res/AudioFiles/Sound/";
        public const string MUSIC_PATH             = "Assets/Res/AudioFiles/Music/";

        public const string GAME_LOAD_COMPLETE_ARS = "Assets/Res/Config/ScriptableObject/AssetReference/GameLoadComplete_ARS.asset";
        public const string INIT_ARS               = "Assets/Res/Config/ScriptableObject/AssetReference/Init_ARS.asset";

        public const string UNIT_INFO_SETTINGS = "Assets/Res/Config/ScriptableObject/UnitInfoSettings.asset";

        public const string UI_ROOT = "Assets/Res/UI/Prefab/UIRoot.prefab";

        //public const string PLAYER_ROOT        = "Assets/Res/Player/Player.prefab";

        public const string LOCAL_GAME_DATA    = "LOCAL_GAME_DATA.json";
        public const string Full_Friction      = "Assets/Res/PhysicsMaterial/FullFriction.physicsMaterial2D";
        public const string No_Friction        = "Assets/Res/PhysicsMaterial/NoFriction.physicsMaterial2D";
        public const string DEFAULT_TRANSITION = "Assets/Res/UI/Texture/NoAtlas/UIExtension/UIEffect/Default-Transition.png";

        public const string EXISTING        = "SQLite/existing.db";
        public const string ARK_PIXEL_ZH_CN = "Assets/Res/UI/Font/ark-pixel-font-12px-ttf-v2022.07.05/ark-pixel-12px-zh_cn.ttf";

        public const string I500             = "Assets/Res/UI/Texture/Emoji/Emoji2/i500.png";
        public const string BRICK_BACKGROUND = "Assets/Res/UI/Texture/StaticSprite/S_Start/BrickBackground.png";
        public const string ANIM_001         = "Assets/Res/UI/Texture/Emoji/Emoji1/anim_001/anim_001_{0}.png";

        public const string MAIN_SCENE = "Assets/Res/Scenes/Main.unity";

        public const string AUDIO_BGM1           = "Assets/Res/FMODBanks/BGM1.bank";
        public const string AUDIO_MASTER_STRINGS = "Assets/Res/FMODBanks/Master.strings.bank";
        public const string AUDIO_MASTER         = "Assets/Res/FMODBanks/Master.bank";

        public static string RAW_FILE_SAVE_PATH;
        public static string LOCAL_GAME_DATA_PATH;
    }
}