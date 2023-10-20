using GraphProcessor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Model
{
    [System.Serializable, NodeMenuItem("角色/移动")]
    public class P_MoveNode : P_BaseNode
    {
        [Input]
        public Empty In;

        [Output]
        public Empty Out;

        public MoveWay Way;

        [ShowIf("Way", MoveWay.Normal)]
        public Vector2 Direction;
    }
}