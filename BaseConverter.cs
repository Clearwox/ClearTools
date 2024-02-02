using System;
using System.Linq;
using System.Text;

namespace Clear
{
    public interface IBaseConverter
    {
        string ConvertFromDecimal(long value, int baseValue);
        string ConvertToAlpa(long IDouble);
        long ConvertToDecimal(string value, int baseValue);
    }
    public class BaseConverter : IBaseConverter
    {
        private readonly char[] Base36 = ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ").ToCharArray();

        public long ConvertToDecimal(string value, int originalBase)
        {
            value = value.ToUpper();

            if (originalBase < 2 || originalBase > Base36.Length)
                throw new ArgumentOutOfRangeException(nameof(originalBase), "Base is out of range. Please enter a value between 2 and 36.");

            return value.Reverse().Select((c, i) => Base36.Contains(c) ?
                Array.IndexOf(Base36, c) * (long)Math.Pow(originalBase, i) :
                throw new ArgumentException($"Invalid character '{c}' in the input string.", nameof(value)))
                .Sum();
        }


        public string ConvertFromDecimal(long value, int baseValue)
        {
            ValidateBaseValue(baseValue);

            char[] baseChars = Base36.Take(baseValue).ToArray();
            StringBuilder result = new StringBuilder();

            while (value >= 1)
            {
                value = Math.DivRem(value, baseValue, out long remainder);
                result.Insert(0, baseChars[(int)remainder]);
            }

            return result.ToString();
        }


        public string ConvertToAlpa(long IDouble)
        {
            const int baseValue = 26;
            const string Base26 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder();

            while (IDouble >= 1)
            {
                IDouble = Math.DivRem(IDouble, baseValue, out long remainder);
                result.Insert(0, Base26[(int)remainder]);
            }

            return result.ToString();
        }


        private void ValidateBaseValue(int value)
        {
            if (value < 2 || value > 36)
            {
                throw new InvalidOperationException("Base is out of range. Please enter a value between 2 and 36.");
            }
        }
    }
}