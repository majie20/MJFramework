using UnityEngine;

namespace Model
{
    public class PlatformHelper
    {
        public static string GetPlatformSign()
        {
#if UNITY_STANDALONE_WIN
            return "StandaloneWindows64";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "ios";
#elif UNITY_WEBGL
            return "webgl";
#endif
        }

        public string IsRunningOnEmulator6()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass buildClass = new AndroidJavaClass("android.os.Build");
                string radioVersion = buildClass.CallStatic<string>("getRadioVersion");

                return radioVersion;
            }

            return "";
        }
    }
}