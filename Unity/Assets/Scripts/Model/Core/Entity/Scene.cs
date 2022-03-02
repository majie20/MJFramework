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

            this.AddComponentView();

            ObjectHelper.CreateComponent<HttpComponent>(this, false);
            ObjectHelper.CreateComponent<HotComponent>(this, false);
            ObjectHelper.CreateComponent<NPContextComponent>(this, false);
        }

        public override void Dispose()
        {
            foreach (var child in childDic.Values)
            {
                child.Dispose();
            }
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }

            componentDic = null;
            componentView = null;
            childDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
        }
    }
}