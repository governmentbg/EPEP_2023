using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace eCase.Common.Helpers
{
    public static class DataUtils
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public static string ConvertNomIdToName(string nomId)
        {
            if (nomId.Length > 2)
                return nomId.Substring(0, nomId.Length - 2) + "Name";

            throw new Exception("ConvertNomIdToName failure");
        }

        public static string ConvertNomIdToCode(string nomId)
        {
            if (nomId.Length > 2)
                return nomId.Substring(0, nomId.Length - 2) + "Code";

            throw new Exception("ConvertNomIdToCode failure");
        }

        public static bool IsExtensionMethod(MethodInfo method)
        {
            return method.IsDefined(typeof(ExtensionAttribute), false);
        }

        public static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static Type GetUnderlyingType(Type theType)
        {
            if (theType.IsArray)
            {
                return theType.GetElementType();
            }

            if (IsNullableType(theType))
            {
                NullableConverter nullableConverter = new NullableConverter(theType);
                return nullableConverter.UnderlyingType;
            }
            else
            {
                return theType;
            }
        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            if (IsNullableType(conversionType))
            {
                if (value == null)
                {
                    return null;
                }

                // It's a nullable type, and not null, so that means it can be converted to its underlying type
                conversionType = GetUnderlyingType(conversionType);
            }

            return Convert.ChangeType(value, conversionType);
        }

        public static bool TryParseDateTime(string keyword, out DateTime dateTime)
        {
            string format1 = "MM.yyyy";
            string format2 = "dd.MM.yyyy";
            if (DateTime.TryParseExact(keyword, format1, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime)
                || DateTime.TryParseExact(keyword, format2, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int? Len(string obj)
        {
            if (!String.IsNullOrEmpty(obj))
            {
                return obj.Length;
            }

            return 0;
        }

        public static int? Len<T>(Nullable<T> obj) where T : struct
        {
            if (obj != null)
            {
                return Len(obj.ToString());
            }

            return 0;
        }

        public static int? LenNoWhiteSpace(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return Len(obj.Trim());
            }

            return 0;
        }

        public static int? LenNoWhiteSpace<T>(Nullable<T> obj) where T : struct
        {
            if (obj != null)
            {
                return LenNoWhiteSpace(obj.ToString().Trim());
            }

            return 0;
        }

        public static int? GetNullInt32()
        {
            return null;
        }

        public static DateTime? GetNullDatetime()
        {
            return null;
        }

        public static string GetNullString()
        {
            return null;
        }

        public static double? GetNullDouble()
        {
            return null;
        }

        public static bool? GetNullBool()
        {
            return null;
        }

        public static bool IsEmptyInternal<T>(T value)
        {
            return String.IsNullOrWhiteSpace(Convert.ToString(value));
        }

        public static bool? IsEmpty<T>(T value)
        {
            return IsEmptyInternal(value);
        }

        public static bool? NotEmpty<T>(T value)
        {
            return !IsEmpty(value);
        }

        public static bool DateIsValid(int day, int month, int year)
        {
            try
            {
                DateTime date = new DateTime(year, month, day);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool EgnIsValid(string egn)
        {
            return new EGN(egn).IsValid();
        }

        public static bool LnOrLnchIsValid(string uin)
        {
            if (uin.Length != 10) return false;
            else
            {
                foreach (char c in uin.ToCharArray())
                {
                    if (!Char.IsDigit(c)) return false;
                }
            }

            return true;
        }

        public static bool EgnBirthDateIs(string egn, DateTime date)
        {
            EGN e = new EGN(egn);
            if (e.IsValid())
                return false;
            else
                return DateTime.Equals(date, e.BirthDate);
        }

        //public static int ParseDay(string dateString)
        //{
        //    return Formatter.Parse<DateTime>(dateString).Day;
        //}

        //public static int ParseMonth(string dateString)
        //{
        //    return Formatter.Parse<DateTime>(dateString).Month;
        //}

        //public static int ParseYear(string dateString)
        //{
        //    return Formatter.Parse<DateTime>(dateString).Year;
        //}

        public static string ToCamelCase(string s)
        {
            return Char.ToLower(s[0]) + s.Substring(1);
        }

        public static string LimitLength(this string s, int length)
        {
            return s.Length > length ? s.Substring(0, length) : s;
        }

        public static string ConcatenateNames(string firstName, string secondName, string lastName)
        {
            string name = String.Empty;

            if (!String.IsNullOrWhiteSpace(firstName))
                name += firstName;

            if (!String.IsNullOrWhiteSpace(secondName))
                name += " " + secondName;

            if (!String.IsNullOrWhiteSpace(lastName))
                name += " " + lastName;

            return name.Trim();
        }

        public static string[] SplitControlSectorString(string value)
        {
            return SplitString(value, 3);
        }

        public static string[] SplitCensusSectorString(string value)
        {
            return SplitString(value, 1);
        }

        public static string[] SplitString(string value, int digitsNum)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return new string[] { };
            }
            else
            {
                string modifiedVal = value;
                while (true)
                {
                    int dashIndex = modifiedVal.IndexOf('-');
                    if (dashIndex != -1)
                    {
                        int firstVal = int.Parse(modifiedVal.Substring(dashIndex - digitsNum, digitsNum));
                        int secondVal = int.Parse(modifiedVal.Substring(dashIndex + 1, digitsNum));

                        int smallerVal = firstVal <= secondVal ? firstVal : secondVal;
                        int biggerVal = secondVal >= firstVal ? secondVal : firstVal;

                        StringBuilder newVal = new StringBuilder();
                        for (int i = smallerVal; i <= biggerVal; i++)
                        {
                            newVal.Append(i.ToString().PadLeft(digitsNum, '0'));
                            if (i != biggerVal)
                            {
                                newVal.Append(",");
                            }
                        }

                        modifiedVal = modifiedVal.Remove(dashIndex - digitsNum, 2 * digitsNum + 1);
                        modifiedVal = modifiedVal.Insert(dashIndex - digitsNum, newVal.ToString());
                    }
                    else
                    {
                        break;
                    }
                }

                return modifiedVal.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static object Pow(object left, object right)
        {
            double leftDouble;
            double rightDouble;
            if (TryConvert<double>(left, out leftDouble) && TryConvert<double>(right, out rightDouble))
            {
                double result = Math.Pow(leftDouble, rightDouble);
                return result;
            }
            else
            {
                throw new Exception(String.Format("Cannot use Pow operation on non-numeric values {0} and {1}.", left, right));
            }
        }

        public static bool TryConvert<T>(object value, out T valueOfT)
        {
            try
            {
                valueOfT = (T)DataUtils.ChangeType(value, typeof(T));
                return true;
            }
            catch
            {
                valueOfT = default(T);
                return false;
            }
        }

        public static bool IsNumeric(Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static string StrConcat(string[] stringArr)
        {
            return String.Concat(stringArr);
        }

        public static int Switch(object[] objArr)
        {
            return 1;
        }

        public static object WrapNullableValue(object value)
        {
            Type valueType = value.GetType();
            if (DataUtils.IsNullableType(valueType))
            {
                return value;
            }

            if (valueType == typeof(string))
            {
                return value;
            }
            else if (valueType == typeof(decimal))
            {
                return (Nullable<decimal>)value;
            }
            else if (valueType == typeof(int))
            {
                return (Nullable<int>)value;
            }
            else if (valueType == typeof(long))
            {
                return (Nullable<long>)value;
            }
            else if (valueType == typeof(DateTime))
            {
                return (Nullable<DateTime>)value;
            }
            else if (valueType == typeof(bool))
            {
                return (Nullable<bool>)value;
            }
            else if (valueType == typeof(double))
            {
                return (Nullable<double>)value;
            }
            else
            {
                throw new NotSupportedException(String.Format("Type '{0}' is not supported.", valueType.FullName));
            }
        }

        public static Type GetNullableType(Type underlyingType)
        {
            if (DataUtils.IsNullableType(underlyingType))
            {
                return underlyingType;
            }

            if (underlyingType == typeof(string))
            {
                return underlyingType;
            }
            else if (underlyingType == typeof(decimal))
            {
                return typeof(Nullable<decimal>);
            }
            else if (underlyingType == typeof(int))
            {
                return typeof(Nullable<int>);
            }
            else if (underlyingType == typeof(long))
            {
                return typeof(Nullable<long>);
            }
            else if (underlyingType == typeof(DateTime))
            {
                return typeof(Nullable<DateTime>);
            }
            else if (underlyingType == typeof(bool))
            {
                return typeof(Nullable<bool>);
            }
            else if (underlyingType == typeof(double))
            {
                return typeof(Nullable<double>);
            }
            else
            {
                throw new NotSupportedException(String.Format("Type '{0}' is not supported.", underlyingType.FullName));
            }
        }

        public static decimal Percent(decimal value, decimal percentage)
        {
            if (percentage > 100)
            {
                throw new ArgumentException("Percentage must be below 100.");
            }

            return (decimal)((value / (decimal)100) * percentage);
        }

        public static bool? IsValidEGN(string egn)
        {
            return IsValidEGNInternal(egn);
        }

        internal static bool IsValidEGNInternal(string egn)
        {
            return new EGN(egn).IsValid();
        }

        public static DateTime? DateFromEGN(string egn)
        {
            return DateFromEGNInternal(egn);
        }

        internal static DateTime? DateFromEGNInternal(string egn)
        {
            EGN e = new EGN(egn);
            if (!e.IsValid())
            {
                return null;
            }
            else
            {
                return e.BirthDate;
            }
        }

        public static DateTime? DateNull()
        {
            return null;
        }

        public static T Iif<T>(bool? condition, T value1, T value2)
        {
            if (condition.HasValue)
            {
                return (condition.Value ? value1 : value2);
            }
            else
            {
                throw new ArgumentNullException("condition");
            }
        }

        public static T IsNull<T>(T value, T replaceValue)
        {
            bool? isEmpty = IsEmpty<T>(value);
            if (isEmpty.HasValue)
            {
                return !isEmpty.Value ? value : replaceValue;
            }
            else
            {
                return replaceValue;
            }
        }

        public static string IsNull(string value, string replaceValue)
        {
            return (!String.IsNullOrEmpty(value) ? value : replaceValue);
        }

        public static bool NullableTypesMatching(Type type1, Type type2)
        {
            if (DataUtils.IsNullableType(type1) && !DataUtils.IsNullableType(type2) && DataUtils.GetUnderlyingType(type1) == type2)
            {
                return true;
            }

            if (!DataUtils.IsNullableType(type1) && DataUtils.IsNullableType(type2) && type1 == DataUtils.GetUnderlyingType(type2))
            {
                return true;
            }

            return false;
        }

        public static bool TypeEqualOrLarger(Type type1, Type type2)
        {
            if (type1 == type2)
            {
                return true;
            }
            else
            {
                return (DataUtils.IsNullableType(type1) && DataUtils.GetUnderlyingType(type1) == type2);
            }
        }

        public static Type MakeNullableType(Type type)
        {
            if (DataUtils.IsNullableType(type))
            {
                return type;
            }

            Type nullableType = typeof(Nullable<>).MakeGenericType(type);
            return nullableType;
        }

        public static bool? In<T>(T valueObject, params T[] args)
        {
            string valueObjectString = Convert.ToString(valueObject, CultureInfo.InvariantCulture);
            foreach (T arg in args)
            {
                if (valueObjectString == Convert.ToString(arg, CultureInfo.InvariantCulture))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool? NotIn<T>(T valueObject, params T[] args)
        {
            return !DataUtils.In<T>(valueObject, args);
        }

        public static bool? Between<T>(T valueObject, T lowValue, T highValue)
        {
            return Comparer<T>.Default.Compare(valueObject, lowValue) >= 0 && Comparer<T>.Default.Compare(valueObject, highValue) <= 0;
        }

        public static bool? NotBetween<T>(T valueObject, T lowValue, T highValue)
        {
            return !Between<T>(valueObject, lowValue, highValue);
        }

        public static DateTime? DateDate(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }
            else
            {
                return date.Value.Date;
            }
        }

        public static int? DateYear(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }
            else
            {
                return date.Value.Year;
            }
        }

        public static DateTime? Now()
        {
            return DateTime.Now;
        }

        public static DateTime? Today()
        {
            return DateTime.Today;
        }

        // Не се използва (вместо това - fnAge())
        public static int? Age(DateTime? birthDate, DateTime? processDate)
        {
            if (!birthDate.HasValue || !processDate.HasValue || birthDate > processDate)
            {
                return null;
            }
            else
            {
                DateTime today = processDate.Value.Date;
                int age = today.Year - birthDate.Value.Year;
                if (birthDate.Value > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }

        public static string GenderFromEGN(string egn)
        {
            EGN e = new EGN(egn);
            if (!e.IsValid())
            {
                return null;
            }
            else
            {
                switch (e.Sex)
                {
                    case EGN.Gender.None:
                        return null;
                    case EGN.Gender.Male:
                        return "1";
                    case EGN.Gender.Female:
                        return "2";
                }
            }

            return null;
        }

        public static string TrimLeft(string source, string ch)
        {
            if (!string.IsNullOrEmpty(source))
            {
                return source.TrimStart(new char[] { ch[0] });
            }

            return source;
        }

        //public static DateTime? ToDate(string val, string format)
        //{
        //    DateTime valDate;

        //    if (string.IsNullOrEmpty(val))
        //        return null;

        //    if (!string.IsNullOrEmpty(format))
        //    {

        //        MatchDateTimeAndFormatLength(ref val, ref format);

        //        if (Formatter.TryParseDatetime(val, format, out valDate))
        //        {
        //            return valDate;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        if (DateTime.TryParse(val, out valDate))
        //        {
        //            return valDate;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public static int? ToInt(string val)
        {
            int valResult;

            if (Int32.TryParse(val, out valResult))
            {
                return valResult;
            }
            else
            {
                return null;
            }
        }

        public static void MatchDateTimeAndFormatLength(ref string val, ref string format)
        {
            if (val == null)
            {
                throw new Exception("MatchDateTimeAndFormatLength does not work with NULL string for value.");
            }

            if (val.Length < format.Length)
            {
                format = format.Substring(0, val.Length);
            }
            else if (val.Length > format.Length)
            {
                val = val.Substring(0, format.Length);
            }
        }

        public static string IntegerToStringSpace(int result)
        {
            return String.Format("{0:n0}", result).Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, " ");
        }

        public static string DoubleToStringDecimalPoint(double result)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            return Math.Round(result, 2).ToString(nfi);
        }

        public static string DecimalToStringDecimalPoint(decimal result)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            return Math.Round(result, 2).ToString(nfi);
        }

        public static string DoubleStringToStringDecimalPointSpace(string result)
        {
            if (!String.IsNullOrWhiteSpace(result))
            {
                double value;
                if (double.TryParse(result, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberDecimalSeparator = ".";
                    nfi.NumberGroupSeparator = " ";

                    return Math.Round(value, 2).ToString("N", nfi);
                }
            }

            return result;
        }

        public static string DecimalToStringDecimalPointSpace(decimal result)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = " ";

            return Math.Round(result, 2).ToString("N", nfi);
        }

        public static string Romanize(string number)
        {
            return Romanize(int.Parse(number));
        }

        public static string Romanize(int number)
        {
            var retVal = new StringBuilder(5);
            var valueMap = new SortedDictionary<int, string>
                               {
                                   { 1, "I" },
                                   { 4, "IV" },
                                   { 5, "V" },
                                   { 9, "IX" },
                                   { 10, "X" },
                                   { 40, "XL" },
                                   { 50, "L" },
                                   { 90, "XC" },
                                   { 100, "C" },
                                   { 400, "CD" },
                                   { 500, "D" },
                                   { 900, "CM" },
                                   { 1000, "M" },
                               };

            foreach (var kvp in valueMap.Reverse())
            {
                while (number >= kvp.Key)
                {
                    number -= kvp.Key;
                    retVal.Append(kvp.Value);
                }
            }

            return retVal.ToString();
        }
    }

    public class EGN
    {
        public enum Gender
        {
            /// <summary>
            /// Стойност по подразбиране: пола на лицето не е инициализиран
            /// </summary>
            None,
            /// <summary>
            /// Лицето е мъж
            /// </summary>
            Male,
            /// <summary>
            /// Лицето е жена
            /// </summary>
            Female
        }

        private int[] coef = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
        private DateTime birthDate;
        private Gender sex;

        public string EGNError;
        public bool IsValid()
        {
            return string.IsNullOrEmpty(EGNError);
        }

        public EGN(string egn)
        {
            if (string.IsNullOrWhiteSpace(egn) || egn.Length != 10)
            {
                EGNError = "Invalid EGN length";
                return;
            }

            for (int i = 0; i < 10; i++)
                if (!Char.IsDigit(egn, i))
                {
                    EGNError = "EGN must contain digits only";
                    return;
                }

            int yy = Int32.Parse(egn.Substring(0, 2));
            int mm = Int32.Parse(egn.Substring(2, 2));
            int dd = Int32.Parse(egn.Substring(4, 2));

            if (mm >= 21 && mm <= 32)
            {
                mm -= 20;
                yy += 1800;
            }
            else if (mm >= 41 && mm <= 52)
            {
                mm -= 40;
                yy += 2000;
            }
            else
                yy += 1900;

            try
            {
                birthDate = new DateTime(yy, mm, dd);
            }
            catch
            {
                EGNError = "Invalid date in EGN";
                return;
            }

            if (Convert.ToInt32(egn.Substring(8, 1)) % 2 == 0)
                sex = Gender.Male;
            else
                sex = Gender.Female;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (Int32.Parse(egn.Substring(i, 1)) * coef[i]);

            int rem = sum % 11;
            if (rem == 10)
                rem = 0;

            if (rem != Int32.Parse(egn.Substring(9, 1)))
            {
                EGNError = "Invalid EGN checksum";
                return;
            }
        }

        public static DateTime DateFromEGNInternal(string egn)
        {
            EGN e = new EGN(egn);
            if (!e.IsValid())
            {
                return DateTime.MinValue;
            }
            else
            {
                return e.BirthDate;
            }
        }

        /// <summary>
        /// Датата на раждане на лицето със съответното ЕГН.
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                return birthDate;
            }
        }

        /// <summary>
        /// Полът на лицето със съответното ЕГН.
        /// </summary>
        public Gender Sex
        {
            get
            {
                return sex;
            }
        }
    }
}
