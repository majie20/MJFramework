namespace MGame
{
    public class SingletonEntity : Entity
    {
        public SingletonEntity()
        {
        }

        public override void Dispose()
        {
            foreach (var component in GetComponents())
            {
                component.Dispose();
            }

            base.Dispose();
        }
    }
}