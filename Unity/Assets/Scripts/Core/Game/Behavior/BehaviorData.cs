namespace Model
{
    public class BehaviorData
    {
        //判断用途
        public const string If_JudgeDistanceByTargetPoint = "If_JudgeDistanceByTargetPoint"; //判断与目标点的距离
        public const string If_JudgeIsHurt                = "If_JudgeIsHurt"; //判断是否受击
        public const string If_JudgeIsInFirstAction       = "If_JudgeIsInFirstAction"; //判断是否是第一次行动

        //执行用途
        public const string Action_TogglePatrolPoint  = "Action_TogglePatrolPoint"; //切换巡逻点
        public const string Action_StayForWhile       = "Action_StayForWhile"; //待一会儿
        public const string Action_FirstActionExpired = "Action_FirstActionExpired"; //第一次行动过期

        //更新用途
        public const string Update_Test = "Update_Test";
    }
}