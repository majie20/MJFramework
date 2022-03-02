using System.Collections.Generic;

namespace Model
{
    public class NP_LogicValue
    {
        public const string If_JudgeAndTargetDistance = "If_JudgeAndTargetDistance";

        public const string Action_Test = "Action_Test";

        public static readonly Dictionary<string, System.Func<NP_BaseBehaviorTree, bool>> JudgeUseCalls =
            new Dictionary<string, System.Func<NP_BaseBehaviorTree, bool>>
            {
                {If_JudgeAndTargetDistance,NP_LogicCall.If_JudgeAndTargetDistance}
            };

        public static readonly Dictionary<string, System.Action<NP_BaseBehaviorTree>> ActionUseCalls =
            new Dictionary<string, System.Action<NP_BaseBehaviorTree>>
            {
                {Action_Test,NP_LogicCall.Action_Test}
            };
    }

    public class NP_LogicCall
    {
        public static bool If_JudgeAndTargetDistance(NP_BaseBehaviorTree tree)
        {
            return true;
        }

        public static void Action_Test(NP_BaseBehaviorTree tree)
        {

        }
    }
}