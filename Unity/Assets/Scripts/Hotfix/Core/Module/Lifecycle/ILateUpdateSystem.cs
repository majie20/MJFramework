namespace Hotfix
{
#if HybridCLR
    public interface ILateUpdateSystem : Model.ILateUpdateSystem
    {
    }
#else
    public interface ILateUpdateSystem
    {
        void OnLateUpdate();
    }
#endif
}