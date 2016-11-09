using System;
using System.Security.Cryptography;
using System.Text;

namespace product1
{
    public static class HashExtensions
    {
        public static string Sha256(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            using (SHA256 shA256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(shA256.ComputeHash(bytes));
            }
        }

        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
                return (byte[])null;
            using (SHA256 shA256 = SHA256.Create())
                return shA256.ComputeHash(input);
        }

        public static string Sha512(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            using (SHA512 shA512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(shA512.ComputeHash(bytes));
            }
        }

    }
}