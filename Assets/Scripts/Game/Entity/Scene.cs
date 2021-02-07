namespace MGame
{
    //[HideInHierarchy]
    public class Scene : SingletonEntity
    {
        public Scene() 
        {
  
        }

        public override Entity Init()
        {
            base.Init();
            AddComponent(new PrefabAssociateComponent().Init(this));
            AddComponent(new ABComponent().Init(this));
            return this;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}