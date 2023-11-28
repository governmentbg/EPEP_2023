using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace eCase.Web.Helpers
{
    public class ValidPasswordAttribute : StringLengthAttribute
    {
        public ValidPasswordAttribute(int maximumLength)
            : base(maximumLength)
        {
        }

        public override bool IsValid(object value)
        {
            string strValue = (value as string) ?? string.Empty;

            if (strValue.Length < MinimumLength)
            {
                return false;
            }

            int smallLettersCount = 0, capitalLettersCount = 0, digitsCount = 0, symbolsCount = 0;

            foreach (char c in strValue)
            {
                if (Char.IsLower(c))
                    smallLettersCount++;
                else if (Char.IsUpper(c))
                    capitalLettersCount++;
                else if (Char.IsDigit(c))
                    digitsCount++;
                else if (!Char.IsWhiteSpace(c))
                    symbolsCount++;
            }

            int[] groups = new int[] { smallLettersCount, capitalLettersCount, digitsCount, symbolsCount };

            int validGroupsCount = groups.Count(g => g >= 2);

            if (validGroupsCount < 2)
            {
                return false;
            }

            return true;
        }
    }
}