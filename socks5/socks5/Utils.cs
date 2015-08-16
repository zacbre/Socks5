using System;
using System.Collections.Generic;
using System.Text;

namespace socks5
{
    class Utils
    {
        private static Random r = new Random(Environment.TickCount);
        public static string RandStr(int count)
        {
            string abc = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string ret = "";
            for (int i = 0; i < count; i++)
            {
                ret += abc[r.Next(0, abc.Length - 1)];
            }
            return ret;
        }
    }
}
