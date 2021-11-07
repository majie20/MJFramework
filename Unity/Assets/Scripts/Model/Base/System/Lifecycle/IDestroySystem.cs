namespace Model
{
    public interface IDestroySystem
    {
    }

    public interface IDestroy : IDestroySystem
    {
        void Destroy();
    }

    public interface IDestroy<A> : IDestroySystem
    {
        void Destroy(A a);
    }

    public interface IDestroy<A, B> : IDestroySystem
    {
        void Destroy(A a, B b);
    }

    public interface IDestroy<A, B, C> : IDestroySystem
    {
        void Destroy(A a, B b, C c);
    }
}