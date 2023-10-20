// created by StarkMini

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StarkMini.Examples.UI {
    /// <summary>
    /// 通用的本地化多语言管理。 通常调用`Get()`以 key 获取本地化语言文本。 支持通过接口增加自定义的本地化语言文本。
    /// </summary>
    public class LocaleManager {
        public const string Lang_Chinese = "zh";
        public const string Lang_English = "en";

        /// <summary>
        /// 设置默认语言。 注：当以key获取语言文本时，若当前语言未配置此key，则使用默认语言中的此key的文本。
        /// </summary>
        public static string DefaultLanguage { get; set; } = Lang_Chinese;

        /// <summary>
        /// 当前语言。 
        /// </summary>
        public static string CurrentLanguage { get; set; } = DefaultLanguage;

        /// <summary>
        /// 以 key 获取本地化语言文本。 文本字典按当前语言`CurrentLanguage`。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static string Get(string key, string defaultText = null) {
            if (!_isInitialized) {
                Init();
            }

            var dict = GetDict(CurrentLanguage);
            if (dict != null) {
                if (dict.ContainsKey(key)) {
                    dict.TryGetValue(key, out string text);
                    return text;
                }
            }

            if (defaultText == null) {
                var defaultDict = GetDict(DefaultLanguage);
                defaultDict.TryGetValue(key, out string text);
                return text;
            }
            return defaultText;
        }

        public static bool IsInitialized {
            get => _isInitialized;
        }

        /// <summary>
        /// 初始化。并非强制手动调用。在首次调用其他功能时，会自动检查和完成初始化。
        /// </summary>
        public static void Init() {
            if (_isInitialized) {
                return;
            }

            InitDefaultDicts();
            InitDefaultValues();
        }

        /// <summary>
        /// 增加自定义的本地化语言文本
        /// </summary>
        /// <param name="language"></param>
        /// <param name="textsDict"></param>
        public static void AddLocaleTexts(string language, Dictionary<string, string> textsDict) {
            if (!_isInitialized) {
                Init();
            }

            var dict = GetOrCreateDict(language);
            if (dict != null) {
                foreach (var pair in textsDict) {
                    dict[pair.Key] = pair.Value;
                }
            } else {
                Debug.LogError($"locale dictionary error! language: {language}");
            }
        }

        private static Dictionary<string, string> GetDict(string language) {
            _localeDict.TryGetValue(language, out var dict);
            return dict;
        }

        private static Dictionary<string, string> GetOrCreateDict(string language) {
            var dict = GetDict(language);
            if (dict == null) {
                InitDict(language);
                dict = GetDict(language);
            }
            return dict;
        }

        private static void InitDefaultDicts() {
            InitDict(Lang_Chinese);
            InitDict(Lang_English);
        }
        private static void InitDict(string language) {
            _localeDict[language] = new Dictionary<string, string>();
        }

        public const string TextKey_Load_GameResErrorMsg = "Load.GameResErrorMsg";
        public const string TextKey_Load_ErrorTitle = "Load.ErrorTitle";
        public const string TextKey_Load_ErrorMsg = "Load.ErrorMsg";
        public const string TextKey_Retry = "Retry";
        public const string TextKey_OK = "OK";
        public const string TextKey_SDK_AdLoadErrorMsg = "AdLoadErrorMsg";
        public const string TextKey_SDK_AdExceptionMsg = "AdExceptionMsg";

        private static void InitDefaultValues() {
            // for "zh"
            var zh = GetDict(Lang_Chinese);
            zh[TextKey_Load_GameResErrorMsg] = "游戏资源错误！\n请重新启动游戏。";
            zh[TextKey_Load_ErrorTitle] = "糟糕";
            zh[TextKey_Load_ErrorMsg] = "网络不给力，请检查网络状况后重试";
            zh[TextKey_Retry] = "重试";
            zh[TextKey_OK] = "确定";
            zh[TextKey_SDK_AdLoadErrorMsg] = "广告加载失败，请检查网络状况后重试";
            zh[TextKey_SDK_AdExceptionMsg] = "广告加载失败，系统异常。";

            // for "en"
            var en = GetDict(Lang_English);
            en[TextKey_Load_GameResErrorMsg] = "Resources error! Please restart the game and retry.";
            en[TextKey_Load_ErrorTitle] = "Oops";
            en[TextKey_Load_ErrorMsg] = "Loading Failed! Please check your network condition and retry.";
            en[TextKey_Retry] = "Retry";
            en[TextKey_OK] = "OK";
            en[TextKey_SDK_AdLoadErrorMsg] = "Ad Load Failed! Please check your network condition and retry.";
            en[TextKey_SDK_AdExceptionMsg] = "Ad Load Failed! System error.";
        }

        private static bool _isInitialized = false;
        private static Dictionary<string, Dictionary<string, string>> _localeDict = new Dictionary<string, Dictionary<string, string>>();
    }
}
