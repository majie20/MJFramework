namespace Hotfix
{
    public interface IAwakeSystem
    {
    }

    public interface IAwake : IAwakeSystem
    {
        void Awake();
    }

    public interface IAwake<A> : IAwakeSystem
    {
        void Awake(A a);
    }

    public interface IAwake<A, B> : IAwakeSystem
    {
        void Awake(A a, B b);
    }

    public interface IAwake<A, B, C> : IAwakeSystem
    {
        void Awake(A a, B b, C c);
    }
}