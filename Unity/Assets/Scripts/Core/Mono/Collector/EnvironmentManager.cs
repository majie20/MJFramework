using Sirenix.OdinInspector;
using SuperTiled2Unity;
using UnityEngine;

namespace Model
{
    public class EnvironmentManager : MonoBehaviour
    {
        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        public AreaEventTriggerHandle[] Areas;

        public Transform BornPoint;

        public SuperMap SuperMap;
    }
}