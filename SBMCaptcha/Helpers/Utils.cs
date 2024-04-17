using System;
using System.IO;
using System.Text.RegularExpressions;
namespace SBM_Captcha_ASP
{
    public class Utils
    {
        public static readonly Random Random = new Random();
        public static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
        public static string SanitizeString(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format( @"[{0}]+", invalidChars );
            return Regex.Replace(name, invalidReStr, "_" );
        }
        public static int ConvertToInt(string value)
        {
            int v = -1;
            int.TryParse(value, out v);

            return v;
        }
    }
}
