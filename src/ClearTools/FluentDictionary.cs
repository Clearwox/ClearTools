using System.Collections.Generic;

namespace Clear
{
    public class FluentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new FluentDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            base.Add(key, value);
            return this;
        }

        public new FluentDictionary<TKey, TValue> Remove(TKey key)
        {
            base.Remove(key);
            return this;
        }

        public static FluentDictionary<TKey, TValue> Create(TKey key, TValue value) 
        => new FluentDictionary<TKey, TValue>().Add(key, value);
    }
}