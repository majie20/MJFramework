namespace Model
{
    public enum Camp
    {
        Self,
        Enemy,
        Neutral
    }

    public enum ObjectType
    {
        Item,
        Props,
        Character,
    }

    public enum StateMachineType
    {
        Idle,
        Die,
        Walk,
        Run,
        Jump,
    }

    public enum TargetType
    {
        Item,
        Enemy,
        Wall,
    }

    public class CharacterBaseData
    {
        public float moveSpeed;
        public float runSpeed;
        public Camp camp;

        public CharacterBaseData()
        {
        }

        public CharacterBaseData(float moveSpeed, float runSpeed, Camp camp)
        {
            this.moveSpeed = moveSpeed;
            this.runSpeed = runSpeed;
            this.camp = camp;
        }
    }
}