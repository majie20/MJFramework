namespace Model
{
    public class Unit : Entity
    {
        private EventSystem eventSystem;

        public EventSystem EventSystem
        {
            protected set
            {
                eventSystem = value;
            }
            get
            {
                return eventSystem;
            }
        }

        public Unit()
        {
            this.EventSystem = new EventSystem(true);
        }

        public override void Dispose()
        {
            this.EventSystem.Dispose();
            base.Dispose();
        }
    }
}