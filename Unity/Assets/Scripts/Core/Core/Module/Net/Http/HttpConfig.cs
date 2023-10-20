using System;

namespace Model
{
    public class HttpConfig
    {
        //URL
        public static Uri TestURL1 = new Uri("http://127.0.0.1:8080/");

        //方法
        public static string HOT_GET_NAME = "Hot/GetName";

        public static string HOT_GET_TEXTURE = "Hot/GetTexture";
        public static string HOT_GET_AB_PACK = "Hot/GetABPack";
        public static string HOT_VERIFY_AB_PACK = "Hot/VerifyABPack";
    }
}