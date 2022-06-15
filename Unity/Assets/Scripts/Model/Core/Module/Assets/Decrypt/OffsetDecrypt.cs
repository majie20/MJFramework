using YooAsset;

namespace Model
{
    public class OffsetDecrypt : IDecryptionServices
    {
        private int offset;

        public OffsetDecrypt()
        {
            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            offset = component.ABSettings.EncryptOffsetVolume;
        }

        public ulong GetFileOffset()
        {
            return (ulong)offset;
        }
    }
}