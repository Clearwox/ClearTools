using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clear.EditorJS
{
    // "version" : "2.19.3"

    public class Content
    {
        public long time { get; set; }
        public Block[] blocks { get; set; }
        public string version { get; set; }
    }

    public class Block
    {
        public string type { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string text { get; set; }
        public int level { get; set; }
        public string style { get; set; }
        public string[] items { get; set; }
        public File file { get; set; }
        public string caption { get; set; }
        public bool withBorder { get; set; }
        public bool stretched { get; set; }
        public bool withBackground { get; set; }
        public string html { get; set; }
        public string alignment { get; set; }
        public string link { get; set; }
        public Meta meta { get; set; }
        public string[][] content { get; set; }
        public string code { get; set; }
        public string service { get; set; }
        public string source { get; set; }
        public string embed { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class File
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Meta
    {
        public string url { get; set; }
        public string domain { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Image image { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }
}