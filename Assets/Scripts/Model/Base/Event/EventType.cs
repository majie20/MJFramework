using System.Collections.Generic;

namespace MGame.Model
{
    public class AssetBundleLoadComplete : EventBase { }

    public class EventType
    {
        public static Dictionary<string, string[]> EventTypeGroupDic;

        public static string GameLoadComplete = "GameLoadComplete";
        public static string PrefabAssociateDataLoadComplete = "PrefabAssociateDataLoadComplete";
        public static string TextDataLoadComplete = "TextDataLoadComplete";

        public static void Init()
        {
            EventTypeGroupDic = new Dictionary<string, string[]>();
            EventTypeGroupDic.Add(GameLoadComplete, new[] { PrefabAssociateDataLoadComplete, TextDataLoadComplete });
        }
    }
}