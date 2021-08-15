using MGame.Model;
using UnityEngine;

namespace MGame.Hotfix
{
    public class TestComponent : Component
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
    }
}