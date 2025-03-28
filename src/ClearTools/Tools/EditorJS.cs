using Clear.Models.EditorJS;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace Clear.Tools
{
    public static class EditorJS
    {
        private static string ParseHeading(Data data)
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

        public static string Parse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be null or empty.");

            try
            {
                var editorContent = JsonConvert.DeserializeObject<Content>(content) ??
                    throw new JsonException("Deserialization returned null.");

                return Parse(editorContent);
            }
            catch (JsonReaderException)
            {
                throw new ArgumentException("Content is not a valid json");
            }
        }

        public static string Parse(Content content)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var itm in content.blocks)
            {
                builder.Append(itm.type switch
                {
                    "header" => ParseHeading(itm.data),
                    "paragraph" => $"<p>{itm.data.text}</p>",
                    "list" => ParseList(itm.data.items, itm.data.style),
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

        private static string ParseList(Item[] items, string style)
        {
            var tag = style == "unordered" ? "ul" : "ol";
            var builder = new StringBuilder();
            builder.Append($"<{tag}>");

            foreach (var item in items)
            {
                builder.Append("<li>");
                builder.Append(item.content);
                if (item.items != null && item.items.Any())
                {
                    builder.Append(ParseList(item.items, style));
                }
                builder.Append("</li>");
            }

            builder.Append($"</{tag}>");
            return builder.ToString();
        }
    }
}