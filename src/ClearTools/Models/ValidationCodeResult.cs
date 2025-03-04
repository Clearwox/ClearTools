using System;

namespace Clear.Tools.Models
{
    /// <summary>
    /// Represents a validation code result.
    /// </summary>
    public class ValidationCodeResult
    {
        /// <summary>
        /// The validation code.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// The expiry time of the validation code.
        /// </summary>
        public DateTime ExpiryTime { get; set; }
    }
}
