namespace Model
{
    public class PlatformHelper
    {
        public static string GetPlatformSign()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            return "StandaloneWindows64";
#elif UNITY_ANDROID && !UNITY_EDITOR
        return "Android";
#elif UNITY_IOS && !UNITY_EDITOR
        return "ios";
#endif
        }
    }
}