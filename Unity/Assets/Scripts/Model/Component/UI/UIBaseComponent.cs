namespace Model
{
    [LifeCycle]
    public class UIBaseComponent : Component, IAwake
    {
        public void Awake()
        {
            UIValue.Add();
        }
    }
}