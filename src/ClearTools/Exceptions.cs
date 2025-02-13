using System;

namespace Clear.Exceptions
{
    public class DataDeserializationException : Exception
    {
        public DataDeserializationException(Type type, string data) 
            : base($"Could not deserialize string into {type.Name}\n\ndata:\n{data}") { }
    }
}