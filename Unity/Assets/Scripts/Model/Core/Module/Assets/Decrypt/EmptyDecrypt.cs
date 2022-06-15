using YooAsset;

namespace Model
{
    public class EmptyDecrypt : IDecryptionServices
    {
        public ulong GetFileOffset()
        {
            return 0;
        }
    }
}