using Clear.Tools.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Clear.Tools
{
    public static class OtpUtility
    {
        private static string Generate(string text, int secretKey, DateTime expiryTime, int codeLength)
        {
            var expiryTimeBinary = expiryTime.ToBinary();
            var data = $"{text}|{expiryTimeBinary}";

            using (var hmac = new HMACSHA256(BitConverter.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                var code = BitConverter.ToUInt32(hash, 0) % (uint)Math.Pow(10, codeLength);
                return code.ToString($"D{codeLength}");
            }
        }

        private static void ValidateCodeLength(int codeLength)
        {
            if (codeLength <= 0 || codeLength > 10)
            {
                throw new ArgumentException("Code length must be between 1 and 10.");
            }
        }

        public static ValidationCodeResult GenerateCode(string text, int secretKey, TimeSpan expiryDuration, int codeLength = 6)
        {
            ValidateCodeLength(codeLength);

            var expiryTime = DateTime.UtcNow.Add(expiryDuration);
            var code = Generate(text, secretKey, expiryTime, codeLength);

            return new ValidationCodeResult(code, expiryTime);
        }

        public static ValidationCodeResult GenerateCode(string text, int secretKey, int codeLength = 6)
        {
            ValidateCodeLength(codeLength);

            var now = DateTime.UtcNow;
            var expiryTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
            expiryTime = (expiryTime - now).TotalMinutes < 10 ? expiryTime.AddHours(1) : expiryTime;

            var code = Generate(text, secretKey, expiryTime, codeLength);

            return new ValidationCodeResult(code, expiryTime);
        }

        public static bool ValidateCode(string text, int secretKey, string code, DateTime expiryTime, int codeLength = 6)
        {
            if (DateTime.UtcNow > expiryTime)
            {
                return false;
            }

            var generatedCode = Generate(text, secretKey, expiryTime, codeLength);
            return generatedCode == code;
        }

        public static bool ValidateCode(string text, int secretKey, string code, int codeLength = 6)
        {
            var now = DateTime.UtcNow;
            var nextHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
            var expiryTime = (nextHour - now).TotalMinutes < 10 ? nextHour.AddHours(1) : nextHour;

            var generatedCode = Generate(text, secretKey, expiryTime, codeLength);
            return generatedCode == code;
        }
    }
}