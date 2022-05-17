using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Model
{
    #region 加载界面相关事件

    public class E_LoadingViewProgressRefresh : EventBase1<int, int, long, long>
    { }

    public class E_LoadStateSwitch : EventBase1<LoadProgressType>
    { }

    public class E_GameLoadComplete : EventBaseAsync1
    { }

    #endregion 加载界面相关事件

    #region UI框架相关事件

    public class E_CloseUIViewEvent : EventBaseAsync1
    { }

    public class E_SetMaskModeEvent : EventBase1<int>
    { }

    public class E_OnCloseUIBlackMaskEvent : EventBase1
    { }

    public class E_SetUIBlackMaskParentEvent : EventBase1<Canvas>
    { }

    public class E_SetUIBlackMaskSortingOrderEvent : EventBase1<int>
    { }

    public class E_CloseUIBlackMaskEvent : EventBase1
    { }

    #endregion UI框架相关事件

    #region 按键触发事件

    public class E_ButtonPressedFirstTimeJump : EventBase1
    { }

    public class E_ButtonReleasedJump : EventBase1
    { }

    public class E_ButtonPressedJump : EventBase1
    { }

    public class E_ButtonPressedFirstTimeDash : EventBase1
    { }

    public class E_ButtonReleasedDash : EventBase1
    { }

    public class E_ButtonPressedDash : EventBase1
    { }

    public class E_ButtonPressedFirstTimeCrouch : EventBase1
    { }

    public class E_ButtonReleasedCrouch : EventBase1
    { }

    public class E_ButtonPressedCrouch : EventBase1
    { }

    public class E_ButtonPressedFirstTimeRun : EventBase1
    { }

    public class E_ButtonReleasedRun : EventBase1
    { }

    public class E_ButtonPressedRun : EventBase1
    { }

    public class E_ButtonPressedFirstTimeInteract : EventBase1
    { }

    public class E_ButtonReleasedInteract : EventBase1
    { }

    public class E_ButtonPressedInteract : EventBase1
    { }

    #endregion 按键触发事件

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

    public class E_LevelLoadFinish : EventBase1
    { }

    public class E_GamePause : EventBase1
    { }

    public class E_GameContinue : EventBase1
    { }

    public class E_ExitLevel : EventBase1
    { }

    #endregion 游戏管理

    #region 声音管理事件

    //设置音效大小
    public class E_SetSoundValue : EventBase1<float>
    { }

    //设置背景音乐大小
    public class E_SetMusicValue : EventBase1<float>
    { }

    /// <summary>
    /// 停止音效
    /// </summary>
    public class E_StopSound : EventBase1<int>
    { }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public class E_StopMusic : EventBase1
    { }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public class E_PlayMusic : EventBase1<string>
    { }

    #endregion 声音管理事件

    #region Http相关

    public class E_HttpStatusError : EventBaseAsync1<HttpStatusCode>
    { }

    #endregion Http相关

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