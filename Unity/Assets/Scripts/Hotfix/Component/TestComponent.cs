using Model;
using UnityEngine;

namespace Hotfix
{
    [LifeCycle]
    public class TestComponent : Component, IAwake<string>, IUpdateSystem
    {
        private BodyConstructor bodyConstructor;

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Awake(string a)
        {
            bodyConstructor = Entity.Transform.GetComponent<BodyConstructor>();
            bodyConstructor.Assemble(Entity.Sign);
            Debug.Log($"Awake:{a}"); // MDEBUG:
        }

        public void OnUpdate(float tick)
        {
            Debug.Log($"Awake:{tick}"); // MDEBUG:
        }
    }
}