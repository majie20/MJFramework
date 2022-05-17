using UnityEngine;

namespace Model
{
    public class CharacterComponent : MObjectComponent, IAwake
    {
        public Animator CharacterAnimator;

        private CharacterBaseData m_characterData;

        public void Awake()
        {
            Type = ObjectType.Character;

            CharacterAnimator = this.Entity.Transform.GetComponent<Animator>();

            this.m_characterData = new CharacterBaseData(5f, 10f, Camp.Self);
        }

        public override void Dispose()
        {
            m_characterData = null;
            CharacterAnimator = null;
            base.Dispose();
        }

        public CharacterBaseData GetCharacterBaseData()
        {
            return m_characterData;
        }
    }
}