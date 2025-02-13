using Newtonsoft.Json;
using System.Text;
using System;
using System.Linq;

namespace Clear.Tools
{
    public static class EditorJS
    {
        private static string ParseHeading(Clear.Models.EditorJS.Data data)
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

            var editorContent = JsonConvert.DeserializeObject<Clear.Models.EditorJS.Content>(content) ??
                throw new JsonException("Deserialization returned null.");

            return Parse(editorContent);
        }

        public static string Parse(Clear.Models.EditorJS.Content content)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var itm in content.blocks)
            {
                builder.Append(itm.type switch
                {
                    "header" => ParseHeading(itm.data),
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
    }
}