namespace Model
{
    [ComponentOf(typeof(MObjectComponent))]
    public class CharacterComponent : Component, IAwake
    {
        private CharacterBaseData m_characterData;

        public void Awake()
        {
            MObjectComponent mObjectComponent = this.Entity.GetComponent<MObjectComponent>();
            mObjectComponent.Type = ObjectType.Character;

            this.m_characterData = new CharacterBaseData(5f, 10f, Camp.Self);
        }

        public override void Dispose()
        {
            m_characterData = null;

            base.Dispose();
        }

        public CharacterBaseData GetCharacterBaseData()
        {
            return m_characterData;
        }
    }
}