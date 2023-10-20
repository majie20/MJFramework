namespace Model
{
    public class EventEntity : Entity
    {
        public EventEntity()
        {
            EventSystem = new EventSystem(true);
        }

        public override void Dispose()
        {
            IsDispose = true;
            this.EventSystem.Dispose();
            base.Dispose();
        }
    }
}