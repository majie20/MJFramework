using System.Net;
using UnityEngine;

namespace Model
{
    #region UI框架相关事件

    public class E_CloseUIViewEvent : EventBaseAsync1
    { }

    public class E_SetMaskModeEvent : EventBase1<int>
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
    public class E_PlayMusic : EventBaseAsync1<int>
    { }

    #endregion 声音管理事件

    #region Http相关

    public class E_HttpStatusError : EventBaseAsync1<HttpStatusCode>
    { }

    #endregion Http相关

    #region Camera相关事件

    public class E_SetMainCameraShow : EventBase1<bool>
    { }

    public class E_SetMainCameraFollowTarget : EventBase1<Transform>
    { }

    #endregion Camera相关事件
}