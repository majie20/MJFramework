using System.Collections.Generic;

namespace Hotfix
{

    public class CloseUIViewEvent : EventBase
    { }
    public class EventType
    {
        private static uint GameMain = 100000;

        public static Dictionary<uint, uint[]> EventTypeGroupDic;

        public static uint Add(ref uint value)
        {
            value++;
            return value;
        }

        public static void Init()
        {
            EventTypeGroupDic = new Dictionary<uint, uint[]>();
        }
    }
}