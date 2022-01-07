using UnityEngine;

namespace Hotfix
{
    [LifeCycle]
    public class Test1Component : Model.Component, IAwake, IUpdateSystem
    {
        public void Awake()
        {
            Debug.Log($"Awake:"); // MDEBUG:
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnUpdate(float tick)
        {
            Debug.Log($"OnUpdate:{tick}"); // MDEBUG:
        }
    }
}