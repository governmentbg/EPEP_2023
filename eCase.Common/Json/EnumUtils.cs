using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace eCase.Common.Json
{
    public static class EnumUtils
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<Enum, string>> cache = new ConcurrentDictionary<Type, Dictionary<Enum, string>>();

        public static string GetCamelCaseEnumValue(Enum e)
        {
            return ToCamelCase(Enum.GetName(e.GetType(), e));
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetEnumDescription(this Enum element)
        {
            if (element == null)
            {
                return null;
            }

            Type type = element.GetType();

            Dictionary<Enum, string> cachedDescriptions;
            if (!cache.TryGetValue(type, out cachedDescriptions))
            {
                cachedDescriptions = new Dictionary<Enum, string>();
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    FieldInfo fi = type.GetField(enumValue.ToString());
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    string description = null;
                    if (attributes != null && attributes.Length > 0)
                    {
                        description = attributes[0].Description;
                    }
                    else
                    {
                        description = element.ToString();
                    }

                    cachedDescriptions.Add(enumValue, description);
                }

                cache.TryAdd(type, cachedDescriptions);
            }

            return cachedDescriptions[element];
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    break;
                }

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }
    }
}
