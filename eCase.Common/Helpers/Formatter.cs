using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace eCase.Common.Helpers
{
    /// <summary>
    /// Клас, който се грижи за правилното форматиране на данни
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        /// Скъсавя даден текст до определена дължина
        /// </summary>
        /// <param name="s">подаден текст</param>
        /// <param name="count">дължина на новия текст</param>
        /// <returns></returns>
        public static string Cut(this string s, int count)
        {
            return !string.IsNullOrEmpty(s) ? (s.Length > count ? Regex.Replace(s.Substring(0, count) + "...", "<(.|\n)*?>", "") : s) : string.Empty;
        }

        /// <summary>
        /// Конвертиране на число от тип decimal
        /// </summary>
        /// <param name="s">подаден текст</param>
        /// <param name="result">резултат от тип decimal</param>
        /// <returns></returns>
        public static bool TryParseDecimal(string s, out decimal result)
        {
            NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
            result = 0;

            if (!Decimal.TryParse(s, numberStyles, new NumberFormatInfo { NumberDecimalSeparator = "." }, out result))
            {
                return Decimal.TryParse(s, numberStyles, new NumberFormatInfo { NumberDecimalSeparator = "," }, out result);
            }

            return true;
        }
    }
}