using System.Collections.Generic;

namespace MGame.Hotfix
{
    public class EventType
    {
        public static Dictionary<string, string[]> EventTypeGroupDic;

        public static void Init()
        {
            EventTypeGroupDic = new Dictionary<string, string[]>();
        }
    }
}