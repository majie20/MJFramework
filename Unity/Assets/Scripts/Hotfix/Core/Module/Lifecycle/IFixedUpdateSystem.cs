namespace Hotfix
{
#if HybridCLR
    public interface IFixedUpdateSystem : Model.IFixedUpdateSystem
    {
    }
#else
    public interface IFixedUpdateSystem
    {
        void OnFixedUpdate(float tick);
    }
#endif
}