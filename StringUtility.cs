using System;
using System.Collections.Generic;
using System.Linq;
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
        string ParseEditorJS(EditorJS.Content content);
        string StripHTML(string htmlstring);
        string StripSymbols(string xstring);
        string StripSymbolsAndHTML(string xstring);
        string TruncateString(string id);
        string GenerateValidationCode(string input, DateTime expiryDate, int secretKey);
        bool ValidationCode(string code, string input, DateTime expiryDate, int secretKey);
        string SQLSerialize(string rawString);
        string SQLSerialize(bool value);
        string SQLSerialize(DateTime? value);
        string SQLSerialize(DateTime value);
        string TimeSince(DateTime value);
        string TimeAgo(DateTime dateTime);
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

        public string StripHTML(string htmlstring) =>
            Regex.Replace(htmlstring, "<[^>]*>", string.Empty).Replace("&nbsp;", string.Empty).Trim();

        public string StripSymbols(string xstring) =>
            new Regex("[;\\\\\\\\/:*?\"<>|&']")
                .Replace(xstring, string.Empty)
                .Replace("+", string.Empty)
                .Replace(".", string.Empty)
                .Replace("`", string.Empty)
                .Replace("'", string.Empty)
                .Replace(",", string.Empty)
                .Replace("/", string.Empty)
                .Replace("'", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("{", string.Empty)
                .Replace("}", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("\"", string.Empty)
                .Replace("#", string.Empty)
                .Replace("*", string.Empty);

        public string StripSymbolsAndHTML(string htmlstring) =>
            StripSymbols(StripHTML(htmlstring));

        public string GetSubstring(string text, int startIndex) => text.Substring(startIndex);

        public string GetSubstring(string text, int startIndex, int count) => text.Substring(startIndex, count);

        public string GenerateFileName(string title, string fileExtension) =>
            GenerateFileName(title, fileExtension, string.Empty);

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
        //public string ParseEditorJS(string content) => 
        //    ParseEditorJS(JsonConvert.DeserializeObject<EditorJS.Content>(content));
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

        public string SQLSerialize(string rawString) => rawString?.Replace("'", "''");
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
                return string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                return timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                return timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                return timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                return timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                return timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }
        }
    }
}