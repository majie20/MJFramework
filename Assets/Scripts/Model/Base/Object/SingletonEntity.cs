namespace MGame.Model
{
    public class SingletonEntity<T> where T : Entity, new()
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

        public virtual SingletonEntity<T> Init()
        {
            instance.Init();
            return this;
        }

        public virtual void Dispose()
        {
            instance.Dispose();
            instance = null;
        }
    }
}