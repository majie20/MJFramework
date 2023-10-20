using System.IO;
using YooAsset;

namespace Model
{
    public class EmptyDecrypt : IDecryptionServices
    {
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 0;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException();
        }

        public uint GetManagedReadBufferSize()
        {
            throw new System.NotImplementedException();
        }

        Stream IDecryptionServices.LoadFromStream(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}