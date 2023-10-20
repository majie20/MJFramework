using System.IO;
using YooAsset;

namespace Model
{
    public class OffsetDecrypt : IDecryptionServices
    {
        private ulong offset;

        public OffsetDecrypt()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            offset = component.ABSettings.EncryptOffsetVolume;
        }

        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return offset;
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