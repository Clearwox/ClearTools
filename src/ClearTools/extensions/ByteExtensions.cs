using System;

namespace ClearTools.extensions
{
    public static class ByteExtensions
    {
        public static string ToBase64String(this byte[] value)
        => Convert.ToBase64String(value);
    }
}