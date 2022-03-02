using System.Collections.Generic;

namespace Model
{
    public class AssetBundleLoadComplete : EventBase
    { }

    public class NetTestSend : EventBase<string>
    { }

    public class LoadingViewProgressRefresh : EventBase<float>
    { }

    public class LoadStateSwitch : EventBase<LoadType>
    { }

    public class CloseUIViewEvent : EventBase
    { }


    public class EventType
    {
        private static uint GameMain = 100000;

        public static Dictionary<uint, uint[]> EventTypeGroupDic;

        public static uint GameLoadComplete = Add(ref GameMain);
        public static uint PrefabAssociateDataLoadComplete = Add(ref GameMain);
        public static uint TextDataLoadComplete = Add(ref GameMain);

        public static uint NP_Logic = 100000000;

        public static uint Add(ref uint value)
        {
            return value++;
        }

        public static void Init()
        {
            EventTypeGroupDic = new Dictionary<uint, uint[]>();
            //EventTypeGroupDic.Add(GameLoadComplete, new[] { PrefabAssociateDataLoadComplete, TextDataLoadComplete });
        }
    }
}