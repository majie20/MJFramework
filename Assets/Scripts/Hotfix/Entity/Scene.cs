using UnityEngine;

namespace MGame.Hotfix
{
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