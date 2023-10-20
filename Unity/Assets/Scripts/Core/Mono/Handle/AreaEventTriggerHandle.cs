using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    public enum TriggerMode : byte
    {
        Trigger,
        PlayOnStart,
        Event,
    }

    public enum WrapMode : byte
    {
        Once,
        Loop,
    }

    public class AreaEventTriggerHandle : MonoBehaviour
    {
        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public UnitBornHandle[] UnitBornHandles;

        [EnumToggleButtons]
        public WrapMode WrapMode;

        [ShowIf("WrapMode", Model.WrapMode.Loop)]
        public int Loop;

        [ShowIf("WrapMode", Model.WrapMode.Loop)]
        public float Interval;

        public TriggerMode TriggerMode;

        [ShowIf("TriggerMode", Model.TriggerMode.Trigger)]
        public string TrgetTag;
    }
}