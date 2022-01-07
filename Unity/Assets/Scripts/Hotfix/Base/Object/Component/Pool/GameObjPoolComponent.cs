using UnityEngine;

namespace Hotfix
{
    [LifeCycle]
    public class GameObjPoolComponent : Component, IAwake
    {
        public void Awake()
        {
        }

        public override void Dispose()
        {
            Entity = null;
        }

        public GameObject HatchGameObjByName(string name, Transform parent, bool isAB)
        {
            return Model.Game.Instance.ObjectPool.GetComponent<Model.GameObjPoolComponent>().HatchGameObjByName(name, parent, isAB);
        }

        public void RecycleGameObj(string sign, GameObject obj)
        {
            Model.Game.Instance.ObjectPool.GetComponent<Model.GameObjPoolComponent>().RecycleGameObj(sign, obj);
        }
    }
}