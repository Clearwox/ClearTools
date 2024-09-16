using System;

namespace Clear.Exceptions
{
    public class DataDeserializationException : Exception
    {
        public DataDeserializationException(Type type) : base($"Could not deserialize string into {type.Name}") { }
    }
}