using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    public class MonsterBornHandle : UnitBornHandle
    {
        public bool IsPatrol;

        [ShowIf("IsPatrol", true)]
        public Transform[] PatrolPointList;
    }
}