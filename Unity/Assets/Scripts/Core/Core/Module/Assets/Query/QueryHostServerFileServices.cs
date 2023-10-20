using YooAsset;

namespace Model
{
    // 远端文件查询服务类
    public class QueryHostServerFileServices : IRemoteServices
    {
        public string GetRemoteMainURL(string fileName)
        {
            return $"{Game.Instance.Scene.GetComponent<AssetsComponent>().HostServerURL}/{fileName}";
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return $"{Game.Instance.Scene.GetComponent<AssetsComponent>().HostServerURL}/{fileName}";
        }
    }
}