public class Singleton<T> where T : Singleton<T>, new()
{
    protected static T _instance;
    private static readonly object padlock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        var t = new T();
                        t.init();
                        System.Threading.Thread.MemoryBarrier();
                        _instance = t;
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void init()
    {
    }

    public virtual void Dispose()
    {
        _instance = null;
    }
}