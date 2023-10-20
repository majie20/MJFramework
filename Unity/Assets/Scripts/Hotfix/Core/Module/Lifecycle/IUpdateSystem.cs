namespace Hotfix
{
#if HybridCLR
    public interface IUpdateSystem : Model.IUpdateSystem
    {
    }
#else
    public interface IUpdateSystem
    {
        void OnUpdate(float tick);
    }
#endif
}