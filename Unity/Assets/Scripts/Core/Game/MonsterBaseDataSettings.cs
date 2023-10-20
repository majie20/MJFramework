using GraphProcessor;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "MonsterBaseDataSettings", menuName = "ScriptableObjects/MonsterBaseDataSettings", order = 7)]
    public class MonsterBaseDataSettings : UnitBaseDataSettings
    {
        public int       Level;
        public BaseGraph BehaviorMapConfig;
    }
}