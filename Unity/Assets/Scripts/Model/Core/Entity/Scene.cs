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
            ObjectHelper.CreateComponent<AssetsComponent>(this, false);
            ObjectHelper.CreateComponent<HotComponent>(this, false);
            ObjectHelper.CreateComponent<NPContextComponent>(this, false);
            ObjectHelper.CreateComponent<GameManagerComponent>(this, false);
            ObjectHelper.CreateComponent<MusicManager>(this, false);
            //ObjectHelper.CreateComponent<PostProcessingComponent>(this, false);
            //ObjectHelper.CreateComponent<PostProcessAssetComponent>(this, false);
            ObjectHelper.CreateComponent<TimerComponent>(this, false);
        }

        public override void Dispose()
        {
            IsDispose = true;
            if (childDic.Count > 0)
            {
                foreach (var child in childDic.Values)
                {
                    child.Dispose();
                }
            }

            if (componentDic.Count > 0)
            {
                foreach (var value in componentDic.Values)
                {
                    value.Dispose();
                }
            }

            componentDic = null;
            componentView = null;
            childDic = null;

            UnityEngine.Object.Destroy(GameObject);
            Transform = null;
            GameObject = null;
            IsDispose = false;
        }
    }
}