public class ABMgr : Singleton<ABMgr>
{
    public int a = 10;

    protected override void init()
    {
        base.init();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}