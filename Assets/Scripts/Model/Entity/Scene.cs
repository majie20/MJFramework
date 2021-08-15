using UnityEngine;

namespace MGame.Model
{
    //[HideInHierarchy]
    public class Scene : Entity
    {
        public Scene()
        {
        }

        public Entity Init(string name, Transform parent)
        {
            Init();

            GameObject = new GameObject(name);
            Transform = GameObject.transform;
            Transform.SetParent(parent);

            AddComponentView();

            AddComponent(new ABComponent().Init(this));
            AddComponent(new PrefabAssociateComponent().Init(this));
            AddComponent(new TextManageComponent().Init(this));

            return this;
        }

        public override void Dispose()
        {
            foreach (var value in componentDic.Values)
            {
                value.Dispose();
            }
            componentDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
        }
    }
}