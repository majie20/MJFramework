namespace Model
{
    public class UnitComponent : Component
    {
        private UnitType type;

        public UnitType Type
        {
            set { type = value; }
            get { return type; }
        }

        public int TypeCode;
    }
}