using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Model
{
    /// <summary>
    /// DES工具
    /// </summary>
    public class DESHelper
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="bytes">明文</param>
        public static byte[] Encrypt(byte[] bytes, string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = keyBytes;
            provider.IV = keyBytes;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="bytes">密文</param>
        public static byte[] Decrypt(byte[] bytes, string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = keyBytes;
            provider.IV = keyBytes;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}