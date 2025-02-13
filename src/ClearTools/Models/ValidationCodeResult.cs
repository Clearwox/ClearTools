using System;

namespace Clear.Tools.Models
{
    public class ValidationCodeResult
    {
        public string Code { get; set; } = null!;
        public DateTime ExpiryTime { get; set; }
    }
}
