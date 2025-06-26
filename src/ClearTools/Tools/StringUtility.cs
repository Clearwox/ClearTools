using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Clear.Tools
{
    public static class StringUtility
    {
        public static string AddUpDate() => AddUpDate(0);

        public static string AddUpDate(int padding)
        {
            var now = DateTime.Now;
            return (
                now.Year + now.Month + now.Day + now.Hour +
                now.Minute + now.Second + now.Millisecond + padding).ToString();
        }

        public static string GetDateCode() => TruncateString(DateTime.Now.ToFileTime().ToString());

        public static string GenerateUrlKey(string txt)
        => StripHTML(StripSymbols(txt)).Replace(" ", "-").Replace("--", "-").ToLower();

        public static string GenerateTags(params string[] keys) => string.Join(",", keys);

        public static string StripHTML(string htmlString)
        => Regex.Replace(htmlString, "<[^>]*>", string.Empty)
                .Replace("&nbsp;", string.Empty)
                .Trim();

        public static string StripSymbols(string text)
        => Regex.Replace(text, @"[^a-zA-Z0-9]", string.Empty);

        public static string StripSymbolsAndHTML(string htmlString)
        => StripSymbols(StripHTML(htmlString));

        public static string GetSubstring(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text) || startIndex < 0 || startIndex >= text.Length)
            {
                return string.Empty;
            }

            return text[startIndex..];
        }

        public static string GetSubstring(string text, int startIndex, int count)
        {
            if (string.IsNullOrEmpty(text) || startIndex < 0 || count < 0 || startIndex >= text.Length)
            {
                return string.Empty;
            }

            if (startIndex + count > text.Length)
            {
                count = text.Length - startIndex;
            }

            return new string(text.AsSpan(startIndex, count));
        }

        public static string GenerateFileName(string title, string fileExtension)
        => GenerateFileName(title, fileExtension.Trim('.'), string.Empty);

        public static string GenerateFileName(string title, string fileExtension, string siteName)
        {
            var cleanTitle = StripSymbols(StripHTML(title));
            var words = cleanTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(5);
            var uniqueId = Guid.NewGuid().ToString("N")[..5];

            return $"{siteName}-{string.Join("-", words)}-{uniqueId}.{fileExtension}".Trim('-').Trim();
        }

        public static string TruncateString(string id)
        {
            int b = id.Length / 2;
            return (Convert.ToInt64(id[..b]) + Convert.ToInt64(id[b..])).ToString();
        }

        public static string CreateParagraphsFromReturns(string text)
        {
            var divElement = new XElement("div");

            var paragraphs = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => new XElement("p", line));

            divElement.Add(paragraphs);

            return divElement.ToString();
        }

        public static string CreateReturnsFromParagraphs(string html)
        {
            var xElement = XElement.Parse(html);

            var paragraphs = xElement.Elements("p")
                                     .Select(p => p.Value.Trim())
                                     .Where(p => !string.IsNullOrEmpty(p));

            return string.Join('\n', paragraphs);
        }


        public static string GenerateValidationCode(string input, DateTime expiryDate, int secretKey)
        {
            var charSum = input.Sum(c => (int)c);
            var code = charSum + expiryDate.ToFileTimeUtc() + secretKey;

            for (int i = 0; i < 4; i++)
            {
                code /= code.ToString().Sum(c => c);
            }

            return code.ToString();
        }

        public static bool ValidationCode(string code, string input, DateTime expiryDate, int secretKey)
        => code == GenerateValidationCode(input, expiryDate, secretKey);

        public static string SQLSerialize(string rawString) => rawString.Replace("'", "''");
        public static string SQLSerialize(bool value) => (value ? 1 : 0).ToString();
        public static string SQLSerialize(DateTime? value) => value == null ? "" : SQLSerialize((DateTime)value);
        public static string SQLSerialize(DateTime value) => value.ToString("dd/MMM/yyy HH:mm:ss");

        public static string TimeSince(DateTime value, DateTime currentTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = currentTime - value;
            double seconds = ts.TotalSeconds;

            if (seconds < MINUTE)
                return ts.Seconds == 1 ? "one second ago" : $"{ts.Seconds} seconds ago";
            if (seconds < HOUR)
                return $"{ts.Minutes} minutes ago";
            if (seconds < 2 * HOUR)
                return "an hour ago";
            if (seconds < DAY)
                return $"{ts.Hours} hours ago";
            if (seconds < 2 * DAY)
                return "yesterday";
            if (seconds < MONTH)
                return $"{ts.Days} days ago";
            if (seconds < 12 * MONTH)
            {
                int months = (int)(ts.Days / 30.0);
                return months <= 1 ? "one month ago" : $"{months} months ago";
            }

            int years = (int)(ts.Days / 365.0);
            return years <= 1 ? "one year ago" : $"{years} years ago";
        }

        public static string TimeAgo(DateTime dateTime, DateTime currentTime)
        {
            var timeSpan = currentTime.Subtract(dateTime);

            if (timeSpan.TotalSeconds <= 60)
            {
                return $"{timeSpan.Seconds} seconds ago";
            }
            else if (timeSpan.TotalMinutes <= 60)
            {
                return timeSpan.Minutes > 1 ? $"about {timeSpan.Minutes} minutes ago" : "about a minute ago";
            }
            else if (timeSpan.TotalHours <= 24)
            {
                return timeSpan.Hours > 1 ? $"about {timeSpan.Hours} hours ago" : "about an hour ago";
            }
            else if (timeSpan.TotalDays <= 30)
            {
                return timeSpan.Days > 1 ? $"about {timeSpan.Days} days ago" : "yesterday";
            }
            else if (timeSpan.TotalDays <= 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return months > 1 ? $"about {months} months ago" : "about a month ago";
            }
            else
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return years > 1 ? $"about {years} years ago" : "about a year ago";
            }
        }

        public static string ExtractInitialsFromName(string fullName)
        {
            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return (names[0][0] + (names.Length > 1 ? names[^1][0].ToString() : string.Empty)).ToUpper();
        }

        public static string GenerateToken(int length, bool includeSmallLetters,
            bool includeCapitalLetters, bool includeNumbers, bool includeSpecialCharacters)
        {
            if (length <= 0)
                throw new ArgumentException("Token length must be greater than zero.");

            // Initialize the character pool
            var characterPool = new StringBuilder();

            if (includeCapitalLetters)
            {
                characterPool.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }

            if (includeSmallLetters)
            {
                characterPool.Append("abcdefghijklmnopqrstuvwxyz");
            }

            if (includeNumbers)
            {
                characterPool.Append("0123456789");
            }

            if (includeSpecialCharacters)
            {
                characterPool.Append("!@#$%^&*()-_=+[]{}|;:,.<>?/"); // Special characters set
            }

            if (characterPool.Length == 0)
            {
                throw new ArgumentException("At least one character type must be included.");
            }

            var token = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomNumber);
                    int randomIndex = (int)(BitConverter.ToUInt32(randomNumber, 0) % characterPool.Length);
                    token.Append(characterPool[randomIndex]);
                }
            }

            return token.ToString();
        }
    }
}