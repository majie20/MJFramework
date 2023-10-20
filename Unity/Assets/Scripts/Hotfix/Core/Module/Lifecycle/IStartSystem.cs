namespace Hotfix
{
#if HybridCLR
    public interface IStartSystem : Model.IStartSystem
    {
    }
#else
    public interface IStartSystem
    {
        void Start();
    }
#endif
}