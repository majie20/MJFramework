using Slate;
using Slate.ActionClips;
using UnityEngine;

namespace Model
{
    [Category("自定义")]
    public class S_TimeIntervalAction : ActorActionClip
    {
        [SerializeField]
        [HideInInspector]
        private float _length = 1;

        public override float length
        {
            get { return _length; }
            set { _length = value; }
        }
    }
}