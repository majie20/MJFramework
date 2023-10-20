using GraphProcessor;
using Sirenix.OdinInspector;
using System;
using System.Collections;

namespace Model
{
    [Serializable]
    public struct Empty
    {
    }

    public abstract class P_BaseNode : BaseNode
    {
        public int Order;
#if UNITY_EDITOR

        public static IEnumerable GetIfCallValues = new ValueDropdownList<string>()
        {
            { "判断与目标点的距离", BehaviorData.If_JudgeDistanceByTargetPoint },
            { "判断是否受击", BehaviorData.If_JudgeIsHurt },
            { "判断是否是第一次行动", BehaviorData.If_JudgeIsInFirstAction },
        };

        public static IEnumerable GetLogicCallValues = new ValueDropdownList<string>()
        {
            { "切换巡逻点", BehaviorData.Action_TogglePatrolPoint },
            { "停留一段时间", BehaviorData.Action_StayForWhile },
            { "第一次行动过期", BehaviorData.Action_FirstActionExpired },
        };

        public static IEnumerable GetUpdateCallValues = new ValueDropdownList<string>()
        {
            { "Update_Test", BehaviorData.Update_Test },
        };

#endif
    }
}