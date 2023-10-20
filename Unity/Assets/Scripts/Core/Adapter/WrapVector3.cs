#if ILRuntime

using UnityEngine;

namespace Model
{
    // 消除ILRuntime的GC用的
    public class WrapVector3
    {
        public Vector3 Value;
    }
}
#endif