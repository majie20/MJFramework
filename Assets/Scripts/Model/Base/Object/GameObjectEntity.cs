#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace MGame.Model
{
    public class GameObjectEntity : OrdinaryEntity
    {
#if !UNITY_EDITOR
        public GameObject gameObject { set; get; }
        public Transform transform { set; get; }
        public Entity parent { set; get; }
#endif

        public GameObjectEntity() 
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}