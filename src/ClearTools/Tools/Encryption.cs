using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Clear.Tools
{
    public static class Encryption
    {
        private const int MinKeyLength = 32;

        private static void ValidateKey(string key)
        {
            if (key.Length < MinKeyLength)
            {
                throw new ArgumentException($"Key length must be at least {MinKeyLength} characters.");
            }
        }

        public static string Encrypt(string text, string key)
        {
            ValidateKey(key);

            using (Aes aesAlg = Aes.Create())
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                Array.Resize(ref keyBytes, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;

                var iv = aesAlg.IV;
                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            ValidateKey(key);

            var fullCipher = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                var iv = new byte[aesAlg.BlockSize / 8];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, iv, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                var keyBytes = Encoding.UTF8.GetBytes(key);
                Array.Resize(ref keyBytes, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (var msDecrypt = new MemoryStream(cipher))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}