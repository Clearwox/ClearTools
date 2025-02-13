using System;

namespace Clear.Models
{
    public class CaptcherResponse
    {
        public bool Success { get; set; }
        public DateTime? Challenge_ts { get; set; }
        public string Hostname { get; set; } = default!;
        public string[] Errorcodes { get; set; } = default!;
    }
}