using System.IO;
using YooAsset;

namespace Model
{
    public class AESDecrypt : IDecryptionServices
    {
        private string encryptPassword;

        public AESDecrypt()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            encryptPassword = component.ABSettings.EncryptPassword;
        }

        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException();
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            //return AESHelper.Decrypt(fileInfo., password)
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