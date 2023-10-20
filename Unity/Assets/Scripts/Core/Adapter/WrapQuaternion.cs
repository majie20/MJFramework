#if ILRuntime

using UnityEngine;

namespace Model
{
    // 消除ILRuntime的GC用的
    public class WrapQuaternion
    {
        public Quaternion Value;
    }
}
#endif