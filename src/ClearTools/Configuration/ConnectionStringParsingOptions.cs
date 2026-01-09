using System;

namespace ClearTools.Configuration
{
    /// <summary>
    /// Configuration options for parsing and serializing connection strings.
    /// </summary>
    public class ConnectionStringParsingOptions
    {
        /// <summary>
        /// Gets or sets the delimiter used to separate key-value pairs in the connection string.
        /// Default is semicolon (;).
        /// </summary>
        public string Delimiter { get; set; } = ";";

        /// <summary>
        /// Gets or sets whether escaping is enabled for the delimiter character.
        /// When enabled, a backslash before the delimiter (\;) will be treated as a literal delimiter character.
        /// Default is true.
        /// </summary>
        public bool EnableEscaping { get; set; } = true;

        /// <summary>
        /// Gets or sets the string comparison type used when matching property keys.
        /// Default is <see cref="StringComparison.OrdinalIgnoreCase"/> (case-insensitive).
        /// </summary>
        public StringComparison KeyComparison { get; set; } = StringComparison.OrdinalIgnoreCase;
    }
}
