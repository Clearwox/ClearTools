using System;

namespace Clear.Models
{
    /// <summary>
    /// Represents a response from a CAPTCHA validation.
    /// </summary>
    public class CaptcherResponse
    {
        /// <summary>
        /// Indicates whether the CAPTCHA validation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The timestamp of the challenge.
        /// </summary>
        public DateTime? Challenge_ts { get; set; }

        /// <summary>
        /// The hostname of the site where the CAPTCHA was solved.
        /// </summary>
        public string Hostname { get; set; } = default!;

        /// <summary>
        /// The error codes returned by the CAPTCHA validation.
        /// </summary>
        public string[] Errorcodes { get; set; } = default!;
    }
}