using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace xChatLib
{
    internal static class Security
    {
        public static byte[] GeneratePBKDF1(string Key, bool IV)
        {
            PasswordDeriveBytes Bytes = new PasswordDeriveBytes(Constants.Ansi.GetBytes(Key), Constants.Ansi.GetBytes("i2923jdj293jd982ejkslwdw903k"));
            if (IV)
                return Bytes.GetBytes(16);
            return Bytes.GetBytes(32);
        }
    }
}
