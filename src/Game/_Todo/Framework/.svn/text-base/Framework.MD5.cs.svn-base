///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Framework
{
    public class cMD5
    {
        public static string ConvertStringToMD5(string ClearText)
        {
            MD5 oMd5 = MD5.Create();

            byte[] HashData = oMd5.ComputeHash(Encoding.ASCII.GetBytes(ClearText));

            StringBuilder oSb = new StringBuilder();
            foreach (byte b in HashData)
            {
                oSb.Append(b.ToString("X2"));
            }
            return oSb.ToString();
        }
        public static bool Equals(string one, string two)
        {
            bool ret = false;
            if (ConvertStringToMD5(one).ToLower() == two.ToLower()) return true;
            return ret;
        }
    }
}
