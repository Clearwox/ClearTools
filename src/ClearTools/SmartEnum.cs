using System;
using System.Collections.Generic;

namespace Clear
{
    public abstract class SmartEnum<TEnum> where TEnum : SmartEnum<TEnum>
    {
        public string Name { get; }
        public int Value { get; }

        protected SmartEnum(string name, int value)
        {
            Name = name;
            Value = value;
            Register((TEnum)this);
        }

        private static readonly Dictionary<int, TEnum> _byValue = new Dictionary<int, TEnum>();
        private static readonly Dictionary<string, TEnum> _byName = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        private static void Register(TEnum instance)
        {
            _byValue[instance.Value] = instance;
            _byName[instance.Name] = instance;
        }

        public static IEnumerable<TEnum> List() => _byValue.Values;

        public static TEnum FromValue(int value) =>
            _byValue.TryGetValue(value, out var result) ? result : throw new ArgumentException($"No SmartEnum with value {value}");

        public static TEnum FromName(string name) =>
            _byName.TryGetValue(name, out var result) ? result : throw new ArgumentException($"No SmartEnum with name '{name}'");

        public override string ToString() => Name;

        public override bool Equals(object obj) =>
            obj is SmartEnum<TEnum> other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}