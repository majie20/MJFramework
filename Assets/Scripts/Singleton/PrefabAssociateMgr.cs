using Game.Event;

namespace Game.Singleton
{
    public class PrefabAssociateMgr : Singleton<ABMgr>
    {

        public override void Init()
        {
            base.Init();
            EventSystem.Instance.Add<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
        }

        public override void Dispose()
        {
            base.Dispose();
            EventSystem.Instance.Remove<AssetBundleLoadComplete>(OnAssetBundleLoadComplete);
        }

        private void OnAssetBundleLoadComplete()
        {

        }
    }
}
