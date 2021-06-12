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
        //string ParseEditorJS(string content);
        string ParseEditorJS(EditorJS.Content content);
        string StripHTML(string htmlstring);
        string StripSymbols(string xstring);
        string StripSymbolsAndHTML(string xstring);
        string TruncateString(string id);
        string GenerateValidationCode(string input, DateTime expiryDate, int secretKey);
        bool ValidationCode(string code, string input, DateTime expiryDate, int secretKey);
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
    }
}