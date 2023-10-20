using System.Net;
using UnityEngine;

namespace Model
{
    #region UI框架相关事件

    public class E_CloseUIView : EventBase1
    {
    }

    public class E_SetMaskMode : EventBase1<int>
    {
    }

    #endregion UI框架相关事件

    #region 按键触发事件

    public class E_ButtonPressedFirstTimeJump : EventBase1
    {
    }

    public class E_ButtonReleasedJump : EventBase1
    {
    }

    public class E_ButtonPressedJump : EventBase1
    {
    }

    public class E_ButtonPressedFirstTimeDash : EventBase1
    {
    }

    public class E_ButtonReleasedDash : EventBase1
    {
    }

    public class E_ButtonPressedDash : EventBase1
    {
    }

    public class E_ButtonPressedFirstTimeCrouch : EventBase1
    {
    }

    public class E_ButtonReleasedCrouch : EventBase1
    {
    }

    public class E_ButtonPressedCrouch : EventBase1
    {
    }

    public class E_ButtonPressedFirstTimeRun : EventBase1
    {
    }

    public class E_ButtonReleasedRun : EventBase1
    {
    }

    public class E_ButtonPressedRun : EventBase1
    {
    }

    public class E_ButtonPressedFirstTimeInteract : EventBase1
    {
    }

    public class E_ButtonReleasedInteract : EventBase1
    {
    }

    public class E_ButtonPressedInteract : EventBase1
    {
    }

    #endregion 按键触发事件

    #region Http相关

    public class E_HttpStatusError : EventBaseAsync1<HttpStatusCode>
    {
    }

    #endregion Http相关

    #region 物理碰撞

    public class E_TriggerEnter2D : EventBase1<UnitColliderType, Collider2D>
    {
    }

    public class E_TriggerStay2D : EventBase1<UnitColliderType, Collider2D>
    {
    }

    public class E_TriggerExit2D : EventBase1<UnitColliderType, Collider2D>
    {
    }

    public class E_CollisionEnter2D : EventBase1<UnitColliderType, Collision2D>
    {
    }

    public class E_CollisionStay2D : EventBase1<UnitColliderType, Collision2D>
    {
    }

    public class E_CollisionExit2D : EventBase1<UnitColliderType, Collision2D>
    {
    }

    #endregion 物理碰撞

    #region Animation

    public class E_AnimationEnd : EventBase1<S_AnimationEnd>
    {
    }

    public class E_AttackStart : EventBase1<S_AttackStart>
    {
    }

    #endregion Animation

    #region Camera相关事件

    public class E_SetMainCameraShow : EventBase1<bool>
    {
    }

    public class E_SetMainCameraFollowTarget : EventBase1<Transform>
    {
    }

    #endregion Camera相关事件

    #region 游戏播放相关

    public class E_GameQuit : EventBase1
    {
    }

    #endregion
}