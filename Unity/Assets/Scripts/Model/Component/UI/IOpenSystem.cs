namespace Model
{
    public interface IOpen
    {
        void Open();
    }

    public interface IOpen<A> 
    {
        void Open(A a);
    }

    public interface IOpen<A, B> 
    {
        void Open(A a, B b);
    }

    public interface IOpen<A, B, C>
    {
        void Open(A a, B b, C c);
    }
}