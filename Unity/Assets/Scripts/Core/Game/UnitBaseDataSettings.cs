using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "UnitBaseDataSettings", menuName = "ScriptableObjects/UnitBaseDataSettings", order = 6)]
    public class UnitBaseDataSettings : ScriptableObject
    {
        public int UnitId;
    }
}