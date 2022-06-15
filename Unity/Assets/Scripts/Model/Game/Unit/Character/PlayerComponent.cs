namespace Model
{
    [ComponentOf(typeof(CharacterComponent), typeof(PlayerCtrlComponent))]
    public class PlayerComponent : Component, IAwake
    {
        public void Awake()
        {
            this.Entity.GameObject.name = "player";
        }
    }
}