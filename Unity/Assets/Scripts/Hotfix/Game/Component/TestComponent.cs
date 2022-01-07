using Model;
using UnityEngine;

namespace Hotfix
{
    [LifeCycle]
    public class TestComponent : Model.Component, IAwake<string>, IUpdateSystem
    {
        //private BodyConstructor bodyConstructor;

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Awake(string a)
        {
            //bodyConstructor = Entity.Transform.GetComponent<BodyConstructor>();
            //bodyConstructor.Assemble(Entity.Sign);
            Debug.Log($"Awake:{a}"); // MDEBUG:

            Model.Game.Instance.EventSystem.AddListener<string>(10000, aaa);
            Model.Game.Instance.EventSystem.Invoke(10000, "dadadasd");
            //Model.Game.Instance.EventSystem.RemoveListener<string>(10000, aaa);
            Model.Game.Instance.EventSystem.AddListener<string>(10000, aaa);
            Model.Game.Instance.EventSystem.Invoke(10000, "dadadasd");
            Model.Game.Instance.EventSystem.RemoveListener<string>(10000, aaa);
        }

        private void aaa(string a)
        {
            Debug.Log(a); // MDEBUG:
        }

        public void OnUpdate(float tick)
        {

            //Debug.Log($"Awake:{tick}"); // MDEBUG:
        }
    }
}