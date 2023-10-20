using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class NP_LogicValue
    {
        public static readonly Dictionary<string, System.Func<NP_BaseBehaviorTree, bool>> JudgeUseCalls = new Dictionary<string, System.Func<NP_BaseBehaviorTree, bool>>
        {
            { BehaviorData.If_JudgeDistanceByTargetPoint, NP_LogicCall.If_JudgeDistanceByTargetPoint },
            { BehaviorData.If_JudgeIsHurt, NP_LogicCall.If_JudgeIsHurt },
            { BehaviorData.If_JudgeIsInFirstAction, NP_LogicCall.If_JudgeIsInFirstAction },
        };

        public static readonly Dictionary<string, System.Action<NP_BaseBehaviorTree>> ActionUseCalls = new Dictionary<string, System.Action<NP_BaseBehaviorTree>>
        {
            { BehaviorData.Action_TogglePatrolPoint, NP_LogicCall.Action_TogglePatrolPoint },
            { BehaviorData.Action_StayForWhile, NP_LogicCall.Action_StayForWhile },
            { BehaviorData.Action_FirstActionExpired, NP_LogicCall.Action_FirstActionExpired },
        };

        public static readonly Dictionary<string, System.Action<NP_BaseBehaviorTree>> UpdateUseCalls =
            new Dictionary<string, System.Action<NP_BaseBehaviorTree>> { { BehaviorData.Update_Test, NP_LogicCall.Update_Test }, };
    }

    public class NP_LogicCall
    {
        /// <summary>
        /// 判断与目标点的距离
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static bool If_JudgeDistanceByTargetPoint(NP_BaseBehaviorTree tree)
        {
            return tree.JudgeDistanceByTargetPoint();
        }

        /// <summary>
        /// 判断是否受击
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static bool If_JudgeIsHurt(NP_BaseBehaviorTree tree)
        {
            return tree.JudgeIsHurt();
        }

        /// <summary>
        /// 判断是否是第一次行动
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static bool If_JudgeIsInFirstAction(NP_BaseBehaviorTree tree)
        {
            return tree.If_JudgeIsInFirstAction();
        }

        /// <summary>
        /// 切换巡逻点
        /// </summary>
        /// <param name="tree"></param>
        public static void Action_TogglePatrolPoint(NP_BaseBehaviorTree tree)
        {
            tree.TogglePatrolPoint();
        }

        /// <summary>
        /// 待一会儿
        /// </summary>
        /// <param name="tree"></param>
        public static void Action_StayForWhile(NP_BaseBehaviorTree tree)
        {
            tree.StayForWhile();
        }

        /// <summary>
        /// 第一次行动过期
        /// </summary>
        /// <param name="tree"></param>
        public static void Action_FirstActionExpired(NP_BaseBehaviorTree tree)
        {
            tree.FirstActionExpired();
        }

        public static void Update_Test(NP_BaseBehaviorTree tree)
        {
        }
    }
}