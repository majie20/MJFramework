namespace Hotfix
{
    public class Game : Singleton<Game>
    {
        public LifecycleSystem LifecycleSystem { private set; get; }

        public override void Init()
        {
            LifecycleSystem = new LifecycleSystem();
        }

        public override void Dispose()
        {
            LifecycleSystem?.Dispose();
            LifecycleSystem = null;
        }
    }
}