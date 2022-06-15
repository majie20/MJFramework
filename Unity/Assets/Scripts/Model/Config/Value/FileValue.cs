namespace Model
{
    public class FileValue
    {
        public const string FILE_EXTENSION_PATTERN = @".[a-zA-Z]+\z";//文件后缀匹配字符串

        public const string RES_PATH = "Assets/Res/";
        public const string CODE_DIR_PATH = "Assets/Res/Text/";
        public const string ATLAS_PATH = "Assets/Res/Atlas/";
        public const string UI_SPRITE_PATH = "Assets/Res/Texture/UI/";
        public const string UI_PREFAB_PATH = "Assets/Res/Prefabs/UI/";

        public const string GAME_LOAD_COMPLETE_ARS = "Assets/Res/Config/ScriptableObject/AssetReference/GameLoadComplete_ARS.asset";
        public const string INIT_ARS = "Assets/Res/Config/ScriptableObject/AssetReference/Init_ARS.asset";
        public const string UI_Sprite_Info = "Assets/Res/Config/Json/UISpriteInfo.json";
        public const string UI_PREFAB_TO_ATLAS_INFO = "Assets/Res/Config/Json/UIPrefabToAtlasInfo.json";

        public static string RAW_FILE_SAVE_PATH = FileHelper.JoinPath("RawFile", FileHelper.FilePos.PersistentDataPath,
            FileHelper.LoadMode.Stream);
    }
}