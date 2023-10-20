using System;

namespace Model
{
    public class GuidHelper
    {
        private static byte[] Buffer = new byte[16];

        /// <summary>
        /// 由连字符分隔的32位数字
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            System.Guid guid = Guid.NewGuid();

            return guid.ToString();
        }

        /// <summary>
        /// 根据GUID获取16位的唯一字符串
        /// </summary>
        /// <param name=\"guid\"></param>
        /// <returns></returns>
        public static string GuidTo16String()
        {
            long i = 1;
            Buffer = Guid.NewGuid().ToByteArray();

            for (int j = 0; j < Buffer.Length; j++)
            {
                i *= (Buffer[j] + 1);
            }

            return $"{i - DateTime.Now.Ticks:x}";
        }

        /// <summary>
        /// 根据GUID获取16位的唯一数字序列
        /// </summary>
        /// <returns></returns>
        public static long GuidToLongID()
        {
            Buffer = Guid.NewGuid().ToByteArray();

            return BitConverter.ToInt64(Buffer, 0);
        }
    }
}