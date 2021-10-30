using UnityEngine;

namespace MGame.Hotfix
{
    public class Scene : Entity
    {
        public Scene()
        {
            GameObject = new GameObject("Scene");
            Transform = GameObject.transform;
            Transform.SetParent(Game.Instance.Transform);

            AddComponentView();
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