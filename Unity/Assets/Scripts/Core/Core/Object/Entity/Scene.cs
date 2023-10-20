using UnityEngine;

namespace Model
{
    public class Scene : Entity
    {
        public Scene()
        {
            GameObject = new GameObject("Scene");
            Transform = GameObject.transform;
            Transform.SetParent(Game.Instance.Transform);
            Guid = GuidHelper.GuidToLongID();
            AwakeCalled = true;
            Called = false;

            this.AddComponentView();
            this.EventSystem = new EventSystem(false);
        }

        public override void Dispose()
        {
            IsDispose = true;

            this.EventSystem.Dispose();
            this.EventSystem = null;

            DisposeTimer();

            //if (_childDic.Count > 0)
            //{
            //    foreach (var child in _childDic.Values)
            //    {
            //        child.Dispose();
            //    }
            //}

            //if (_componentDic.Count > 0)
            //{
            //    foreach (var value in _componentDic.Values)
            //    {
            //        value.Dispose();
            //    }
            //}

            _componentDic = null;
            _componentView = null;
            _childDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;

            IsDispose = false;
        }
    }
}