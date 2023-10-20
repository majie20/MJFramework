namespace Hotfix
{
    public class Game : Singleton<Game>
    {
#if ILRuntime
        public LifecycleSystem LifecycleSystem { private set; get; }
#endif

        public override void Init()
        {
#if ILRuntime
            LifecycleSystem = new LifecycleSystem();
#endif
        }

        public override void Dispose()
        {
#if ILRuntime
            LifecycleSystem?.Dispose();
            LifecycleSystem = null;
#endif
        }
    }
}