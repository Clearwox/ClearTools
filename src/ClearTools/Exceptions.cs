using System;

namespace Clear.Exceptions
{
    /// <summary>
    /// Exception thrown when data deserialization fails.
    /// </summary>
    public class DataDeserializationException : Exception
    {
        public DataDeserializationException(Type type, string data) 
            : base($"Could not deserialize string into {type.Name}\n\ndata:\n{data}") { }
    }
}