using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    #region 加载界面相关事件

    public class E_LoadingViewProgressRefresh1 : EventBase1<int, int, long, long>
    {
    }

    public class E_LoadingViewProgressRefresh2 : EventBase1<float>
    { }

    public class E_LoadStateSwitch : EventBase1<LoadProgressType>
    { }



    #endregion 加载界面相关事件

    #region 角色控制事件

    public class E_CharacterWalk : EventBase1<Vector2, float>
    { }

    public class E_CharacterRun : EventBase1<Vector2, float>
    { }

    public class E_CharacterStateMachineToggle : EventBase1<StateMachineType>
    { }

    #endregion 角色控制事件

    #region 游戏管理

    public class E_OpenLevel : EventBase1<int>
    { }

    public class E_LoadSceneFinish : EventBase1
    { }

    public class E_GamePause : EventBase1
    { }

    public class E_GameContinue : EventBase1
    { }

    public class E_ExitLevel : EventBase1
    { }

    #endregion 游戏管理

    public class EventType
    {
        public static Dictionary<uint, uint[]> EventTypeGroupDic;

        private static uint GameMain = 100000;

        public static uint GameLoadComplete = Add(ref GameMain);//游戏加载完成
        public static uint PrefabAssociateDataLoadComplete = Add(ref GameMain);
        public static uint TextDataLoadComplete = Add(ref GameMain);

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