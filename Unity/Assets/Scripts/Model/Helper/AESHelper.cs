using System;
using System.Security.Cryptography;
using System.Text;

namespace Model
{
    /// <summary>
    /// AES工具
    /// </summary>
    public class AESHelper
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="bytes">明文</param>
        public static byte[] Encrypt(byte[] bytes, string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateEncryptor();
            byte[] resultBytes = ict.TransformFinalBlock(bytes, 0, bytes.Length);
            return resultBytes;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="bytes">密文</param>
        public static byte[] Decrypt(byte[] bytes, string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateDecryptor();
            byte[] resultBytes = ict.TransformFinalBlock(bytes, 0, bytes.Length);
            return resultBytes;
        }
    }
}