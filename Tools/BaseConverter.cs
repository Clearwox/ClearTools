using System;
using System.Linq;
using System.Text;

namespace Clear.Tools
{
    public static class BaseConverter
    {
        private static readonly char[] Base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public static long ConvertToDecimal(string value, int baseValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            value = value.ToUpperInvariant();

            ValidateBaseValue(baseValue);

            long result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[value.Length - 1 - i];
                int index = Array.IndexOf(Base36, c);

                if (index == -1)
                    throw new ArgumentException($"Invalid character '{c}' in the input string.", nameof(value));

                result += index * (long)Math.Pow(baseValue, i);
            }

            return result;
        }

        public static string ConvertFromDecimal(long value, int baseValue)
        {
            ValidateBaseValue(baseValue);

            char[] baseChars = Base36.Take(baseValue).ToArray();
            StringBuilder result = new StringBuilder();

            while (value > 0)
            {
                value = Math.DivRem(value, baseValue, out long remainder);
                result.Insert(0, baseChars[(int)remainder]);
            }

            return result.ToString();
        }

        public static string ConvertToAlpha(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative.");

            const int baseValue = 26;
            const string Base26 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder();

            if (value == 0)
                return Base26[0].ToString();

            while (value > 0)
            {
                long remainder = value % baseValue;
                value /= baseValue;
                result.Insert(0, Base26[(int)remainder]);
            }

            return result.ToString();
        }

        private static void ValidateBaseValue(int baseValue)
        {
            if (baseValue < 2 || baseValue > 36)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baseValue), "Base is out of range. Please enter a value between 2 and 36."
                );
            }
        }
    }
}