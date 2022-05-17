namespace Model
{
    public class MObjectComponent : Component
    {
        private ObjectType type;

        public ObjectType Type
        {
            protected set
            {
                type = value;
            }
            get
            {
                return type;
            }
        }
    }
}