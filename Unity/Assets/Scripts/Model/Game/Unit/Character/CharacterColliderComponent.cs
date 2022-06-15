namespace Model
{
    [LifeCycle]
    public class CharacterColliderComponent : Component, IAwake
    {
        private PlayerCollider playerCollider;

        public void Awake()
        {
            playerCollider = this.Entity.GameObject.GetComponent<PlayerCollider>();
            playerCollider.CharacterColliderEventSign = "playerCollider";
            Game.Instance.EventSystem.AddListener<TargetType, string>(playerCollider.CharacterColliderEventSign, this, OnCollider);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void OnCollider(TargetType targetType, string name)
        {
            if (targetType == TargetType.Item)
            {
                GetItem(name);
            }
        }

        public void GetItem(string name)
        {
        }
    }
}