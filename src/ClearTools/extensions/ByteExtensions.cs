using System;

namespace ClearTools.extensions
{
    /// <summary>
    /// Extension methods for byte arrays.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Converts the byte array to a Base64 string.
        /// </summary>
        public static string ToBase64String(this byte[] value)
        => Convert.ToBase64String(value);
    }
}