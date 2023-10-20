#if ILRuntime

using UnityEngine;

namespace Model
{
    // 消除ILRuntime的GC用的
    public class WrapVector2
    {
        public Vector2 Value;
    }
}
#endif