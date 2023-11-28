
namespace eCase.Common
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string TruncateWithEllipsis(this string value, int maxLength)
        {
            var truncatedString = Truncate(value, maxLength);
            if (truncatedString != value)
            {
                truncatedString += "...";
            }

            return truncatedString;
        }
    }
}
