using YooAsset;

namespace Model
{
    public class EmptyDecrypt : IDecryptionServices
    {
        public ulong GetFileOffset(BundleInfo bundleInfo)
        {
            return 0;
        }
    }
}