using System;

namespace Clear.Tools.Models
{
    /// <summary>
    /// Represents a validation code result.
    /// </summary>
    public class ValidationCodeResult
    {
        public ValidationCodeResult(string code, DateTime expiryTime)
        {
            Code = code;
            ExpiryTime = expiryTime;
        }

        /// <summary>
        /// The validation code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// The expiry time of the validation code.
        /// </summary>
        public DateTime ExpiryTime { get; }
    }
}
