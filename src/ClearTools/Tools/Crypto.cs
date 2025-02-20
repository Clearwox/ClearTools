using System;
using System.Security.Cryptography;
using System.Text;

namespace Clear.Tools
{
    public static class Crypto
    {
        public static string CreateSalt(int size = 128)
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string EncodeSHA512(string input, string salt = "")
        {
            using var sha = SHA512.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + salt);
            byte[] hashBytes = sha.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string EncodeSHA256(string input, string salt = "")
        {
            using var sha = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + salt);
            byte[] hashBytes = sha.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string EncodeSHA1(string input)
        {
            using var sha = SHA1.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string EncodeBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes);
        }

        public static string DecodeBase64(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}