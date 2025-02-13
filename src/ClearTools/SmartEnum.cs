using System;

namespace Clear
{
    public interface ISmartEnum<out TEnum> where TEnum : Enum
    {
        string Name { get; }
        TEnum Value { get; }
    }

    public abstract class SmartEnum<TEnum> where TEnum : Enum
    {
        public string Name { get; private set; }
        public TEnum Value { get; private set; }

        protected SmartEnum(string name, TEnum value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is SmartEnum<TEnum> other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class SmartEnums<TEnum> where TEnum : Enum
    {
        public string Name { get; private set; }
        public TEnum Value { get; private set; }

        protected SmartEnums(string name, TEnum value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is SmartEnum<TEnum> other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}