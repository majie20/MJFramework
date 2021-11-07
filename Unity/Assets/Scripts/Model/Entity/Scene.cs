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

            ObjectHelper.CreateComponent<ABComponent>(this, false);
            ObjectHelper.CreateComponent<PrefabAssociateComponent>(this, false);
            ObjectHelper.CreateComponent<TextManageComponent>(this, false);
            //ObjectHelper.CreateComponent<NetComponent>(this, false);
        }

        public override void Dispose()
        {
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }
            componentDic = null;
            componentView = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
        }
    }
}