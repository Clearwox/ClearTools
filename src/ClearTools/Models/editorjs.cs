namespace Clear.Models.EditorJS
{
    // "version" : "2.19.3"

    public class Content
    {
        public long time { get; set; }
        public Block[] blocks { get; set; } = null!;
        public string version { get; set; } = null!;
    }

    public class Block
    {
        public string type { get; set; } = null!;
        public Data data { get; set; } = null!;
    }

    public class Data
    {
        public string text { get; set; } = null!;
        public int level { get; set; }
        public string style { get; set; } = null!;
        public string[] items { get; set; } = null!;
        public File file { get; set; } = null!;
        public string caption { get; set; } = null!;
        public bool withBorder { get; set; }
        public bool stretched { get; set; }
        public bool withBackground { get; set; }
        public string html { get; set; } = null!;
        public string alignment { get; set; } = null!;
        public string link { get; set; } = null!;
        public Meta meta { get; set; } = null!;
        public string[][] content { get; set; } = null!;
        public string code { get; set; } = null!;
        public string service { get; set; } = null!;
        public string source { get; set; } = null!;
        public string embed { get; set; } = null!;
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; } = null!;
    }

    public class File
    {
        public string url { get; set; } = null!;
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Meta
    {
        public string url { get; set; } = null!;
        public string domain { get; set; } = null!;
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public Image image { get; set; } = null!;
    }

    public class Image
    {
        public string url { get; set; } = null!;
    }
}