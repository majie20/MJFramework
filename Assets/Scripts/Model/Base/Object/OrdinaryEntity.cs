namespace MGame.Model
{
    public class OrdinaryEntity : Entity
    {
        public OrdinaryEntity()
        {
        }

        public override void Dispose()
        {
            foreach (var component in GetComponents())
            {
                RemoveComponent(component);
            }

            base.Dispose();
        }
    }
}