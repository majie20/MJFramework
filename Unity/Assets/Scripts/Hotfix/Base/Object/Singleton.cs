namespace Hotfix
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance;
        private static readonly object padlock = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            var t = new T();
                            System.Threading.Thread.MemoryBarrier();
                            instance = t;
                        }
                    }
                }
                return instance;
            }
        }

        public virtual void Init()
        {
        }

        public virtual void Dispose()
        {
            instance = null;
        }
    }
}