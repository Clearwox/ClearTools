using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Clear
{
    public interface IStringUtility
    {
        string AddUpDate();
        string AddUpDate(int padding);
        string CreateParagraphsFromReturns(string text);
        string CreateReturnsFromParagraphs(string text);
        string GenerateFileName(string title, string fileExtension);
        string GenerateFileName(string title, string fileExtension, string siteName);
        string GenerateTags(params string[] keys);
        string GenerateUrlKey(string txt);
        string GetDateCode();
        string GetSubstring(string text, int startIndex);
        string GetSubstring(string text, int startIndex, int count);
        string ParseEditorJS(string content);
        string ParseEditorJS(EditorJS.Content content);
        string StripHTML(string htmlString);
        string StripSymbols(string text);
        string StripSymbolsAndHTML(string htmlString);
        string TruncateString(string id);
        string GenerateValidationCode(string input, DateTime expiryDate, int secretKey);
        bool ValidationCode(string code, string input, DateTime expiryDate, int secretKey);
        string SQLSerialize(string rawString);
        string SQLSerialize(bool value);
        string SQLSerialize(DateTime? value);
        string SQLSerialize(DateTime value);
        string TimeSince(DateTime value);
        string TimeAgo(DateTime dateTime);
        string ExtractInitialsFromName(string fullName);
        string GenerateToken(int length, bool includeSmallLetters, bool includeCapitalLetters, bool includeNumbers, bool includeSpecialCharacters);
    }
    public class StringUtility : IStringUtility
    {
        public string AddUpDate() => AddUpDate(0);

        public string AddUpDate(int padding)
        {
            var now = DateTime.Now;
            return (
                now.Year + now.Month + now.Day + now.Hour +
                now.Minute + now.Second + now.Millisecond + padding).ToString();
        }

        public string GetDateCode() => TruncateString(DateTime.Now.ToFileTime().ToString());

        public string GenerateUrlKey(string txt) =>
            StripHTML(StripSymbols(txt)).Replace(" ", "-").Replace("--", "-").ToLower();

        public string GenerateTags(params string[] keys) => string.Join(",", keys);

        public string StripHTML(string htmlString) =>
            Regex.Replace(htmlString, "<[^>]*>", string.Empty).Replace("&nbsp;", string.Empty).Trim();

        public string StripSymbols(string text)
        {
            string pattern = "[;\\\\/:*?\"<>|&'+`',/\\(\\)\\[\\]{}\\\"#*]";
            return Regex.Replace(text, pattern, string.Empty);
        }

        public string StripSymbolsAndHTML(string htmlString) =>
            StripSymbols(StripHTML(htmlString));

        public string GetSubstring(string text, int startIndex)
        {
            ReadOnlySpan<char> span = text.AsSpan(startIndex);
            return new string(span);
        }


        public string GetSubstring(string text, int startIndex, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (startIndex < 0 || startIndex >= text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is out of range.");
            }

            if (count < 0)
            {
                count = 0;
            }

            if (startIndex + count > text.Length)
            {
                count = text.Length - startIndex;
            }

            ReadOnlySpan<char> span = text.AsSpan(startIndex, count);
            return new string(span);
        }


        public string GenerateFileName(string title, string fileExtension) =>
            GenerateFileName(title, fileExtension.Trim('.'), string.Empty);

        public string GenerateFileName(string title, string fileExtension, string siteName)
        {
            var ts = StripSymbols(StripHTML(title)).Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Take(5).ToList();
            ts.Add(Guid.NewGuid().ToString().Substring(0, 5));
            return $"{siteName}-{string.Join("-", ts)}.{fileExtension}".Trim('-').Trim();
        }

        public string TruncateString(string id)
        {
            int b = id.Length / 2;
            return (Convert.ToInt64(id.Substring(0, b)) + Convert.ToInt64(id.Substring(b + 1))).ToString();
        }

        public string CreateParagraphsFromReturns(string text)
        {
            XElement xe = new XElement("div");

            IEnumerable<string> blocks = text.Split('\n');

            foreach (string block in blocks)
            {
                if (!string.IsNullOrEmpty(block))
                    xe.Add(new XElement("p", block));
            }

            return xe.ToString();
        }

        public string CreateReturnsFromParagraphs(string text)
        {
            text = text.Replace("<div>", string.Empty)
                .Replace("</div>", string.Empty)
                .Replace("<p>", string.Empty)
                .Replace("</p>", string.Empty);

            IEnumerable<string> blocks = text.Split('\n');

            text = string.Empty;

            foreach (string block in blocks)
            {
                if (!string.IsNullOrEmpty(block.Trim()))
                    text += (text == string.Empty ? string.Empty : "\n") + block.Trim();
            }

            return StripHTML(text);
        }

        public string ParseEditorJSHeading(EditorJS.Data data)
        {
            return data.level switch
            {
                1 => $"<h{data.level}>{data.text}</h{data.level}>",
                2 => $"<h{data.level}>{data.text}</h{data.level}>",
                3 => $"<h{data.level}>{data.text}</h{data.level}>",
                4 => $"<h{data.level}>{data.text}</h{data.level}>",
                5 => $"<h{data.level}>{data.text}</h{data.level}>",
                _ => ""
            };
        }

        public string ParseEditorJS(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be null or empty.");

            var editorContent = JsonConvert.DeserializeObject<EditorJS.Content>(content) ??
                throw new JsonException("Deserialization returned null.");

            return ParseEditorJS(editorContent);
        }

        public string ParseEditorJS(EditorJS.Content content)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var itm in content.blocks)
            {
                builder.Append(itm.type switch
                {
                    "header" => ParseEditorJSHeading(itm.data),
                    "paragraph" => $"<p>{itm.data.text}</p>",
                    "list" => $"<{(itm.data.style == "unordered" ? "ul" : "ol")}>{string.Join("", itm.data.items.Select(x => $"<li>{x}</li>"))}</{(itm.data.style == "unordered" ? "ul" : "ol")}>",
                    "image" => $"<img style=\"max-width: 100%;\" src=\"{itm.data.url}\" /><p>{itm.data.caption}</p>",
                    "embed" => itm.data.service switch
                    {
                        "youtube" => @$"<div><div style=""padding-bottom: 56.25%; position: relative;""><iframe width=""100%"" height=""100%"" src=""{itm.data.embed}?modestbranding=1&rel=0"" frameborder=""0"" allow=""accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture; fullscreen""  style=""position: absolute; top: 0px; left: 0px; width: 100%; height: 100%;""></iframe></div></div>",
                        "vimeo" => @$"<div><div style=""padding:60.16% 0 0 0;position:relative;""><iframe src=""{itm.data.embed}?badge=0&amp;autopause=0&amp;player_id=0&amp;app_id=58479"" frameborder=""0"" allow=""autoplay; fullscreen; picture-in-picture; clipboard-write"" style=""position:absolute;top:0;left:0;width:100%;height:100%;"" title=""StoreApp - Loyal sales and Coupon""></iframe></div><script src=""https://player.vimeo.com/api/player.js""></script></div>",
                        _ => ""
                    },
                    _ => ""
                });
            }

            return builder.ToString();
        }

        public string GenerateValidationCode(string input, DateTime expiryDate, int secretKey)
        {
            var code = input.ToCharArray().Sum(x => x) + expiryDate.ToFileTimeUtc() + secretKey;
            code /= code.ToString().ToCharArray().Sum(x => x);
            code /= code.ToString().ToCharArray().Sum(x => x);
            code /= code.ToString().ToCharArray().Sum(x => x);
            code /= code.ToString().ToCharArray().Sum(x => x);
            return code.ToString();
        }

        public bool ValidationCode(string code, string input, DateTime expiryDate, int secretKey) =>
            code == GenerateValidationCode(input, expiryDate, secretKey);

        public string SQLSerialize(string rawString) => rawString.Replace("'", "''");
        public string SQLSerialize(bool value) => (value ? 1 : 0).ToString();
        public string SQLSerialize(DateTime? value) => value == null ? "" : SQLSerialize((DateTime)value);
        public string SQLSerialize(DateTime value) => value.ToString("dd/MMM/yyy HH:mm:ss");

        public string TimeSince(DateTime value)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - value.Ticks);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (seconds < 60 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (seconds < 120 * MINUTE)
                return "an hour ago";

            if (seconds < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (seconds < 48 * HOUR)
                return "yesterday";

            if (seconds < 30 * DAY)
                return ts.Days + " days ago";

            if (seconds < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        public string TimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                return $"{timeSpan.Seconds} seconds ago";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                return timeSpan.Minutes > 1 ?
                    $"about {timeSpan.Minutes} minutes ago" :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                return timeSpan.Hours > 1 ?
                    $"about {timeSpan.Hours} hours ago" :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                return timeSpan.Days > 1 ?
                    $"about {timeSpan.Days} days ago" :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                return timeSpan.Days > 30 ?
                    $"about {timeSpan.Days / 30} months ago" :
                    "about a month ago";
            }
            else
            {
                return timeSpan.Days > 365 ?
                    $"about {timeSpan.Days / 365} years ago" :
                    "about a year ago";
            }
        }

        public string ExtractInitialsFromName(string fullName)
        {
            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return (names[0][0].ToString() + (names.Length > 1 ? names[^1][0].ToString() : "")).ToUpper();
        }

        public string GenerateToken(int length, bool includeSmallLetters,
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