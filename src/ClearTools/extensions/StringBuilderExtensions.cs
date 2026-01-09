using System.Text;

namespace ClearTools.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIfTrue(this StringBuilder sb, bool condition, string value)
        {
            if (condition)
            {
                sb.Append(value);
            }
            return sb;
        }
        public static StringBuilder AppendLineIfTrue(this StringBuilder sb, bool condition, string value)
        {
            if (condition)
            {
                sb.AppendLine(value);
            }
            return sb;
        }
    }
}