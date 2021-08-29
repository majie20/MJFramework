using MGame.Model;
using UnityEngine;

namespace MGame.Hotfix
{
    [LifeCycle]
    public class TestComponent : Component, IAwake, IAwake<string>
    {
        private BodyConstructor bodyConstructor;

        public TestComponent()
        {
        }

        public override Component Init()
        {
            base.Init();

            bodyConstructor = Entity.Transform.GetComponent<BodyConstructor>();
            bodyConstructor.Assemble(Entity.Sign);

            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Awake()
        {
            Debug.Log("Awake"); // MDEBUG:
        }

        public void Awake(string a)
        {
            Debug.Log($"Awake:{a}"); // MDEBUG:
        }
    }
}