using Epep.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.RegularExpressions;

namespace Epep.Core.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Маркира определен елемент, като избран
        /// </summary>
        /// <typeparam name="TSource">Тип на колекцията</typeparam>
        /// <param name="source">Изходна колекция</param>
        /// <param name="selected">Елемент, който да бъде избран</param>
        /// <returns></returns>
        public static SelectList SetSelected<TSource>(
        this IEnumerable<TSource> source, object selected)
        {
            if (source == null)
            {
                return new SelectList(new List<SelectListItem>());
            }
            var result = new SelectList(source, "Value", "Text", selected);
            foreach (var item in result)
            {
                if (item.Text != null)
                {
                    item.Text = System.Web.HttpUtility.HtmlDecode(item.Text);
                }
            }
            return result;
        }

        public static List<SelectListItem> PrependAllItem(this List<SelectListItem> items, string itemText = "Всички")
        {
            return items.Prepend(new SelectListItem(itemText, "-1")).ToList();
        }
        public static List<SelectListItem> SingleOrSelect(this List<SelectListItem> items)
        {
            if(items.Count(x=>x.Value != "-1" && x.Value != "-2") == 1 && items.Count(x => x.Value == "-1" || x.Value == "-2") > 0)
            {
                items.RemoveAt(0);
            }
            return items;
        }

        public static string GetInitials(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var names = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(0, 1)
                .ToUpper()).Take(2).ToArray();
            if (names.Length > 1)
            {
                return names[0] + names[1];
            }
            else
            {
                return names[0];
            }
        }

        public static DateTime StrToDateFormat(this string value, string formatDate)
        {
            if (value.Trim().Length == 0)
                return DateTime.MinValue;

            DateTime _dt = DateTime.Now;
            try
            {
                DateTime.TryParseExact(value, formatDate, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out _dt);
                return _dt;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static string EmptyToNull(this string model, string nullVal = "")
        {
            if (model == null || model?.Trim() == nullVal)
            {
                return null;
            }
            return model.Trim();
        }
        public static string SaveTrim(this string model, string nullVal = "")
        {
            if (model == null)
            {
                return nullVal;
            }
            return model.Trim();
        }

        public static int? EmptyToNull(this int? model, int nullVal = -1)
        {
            if (model == null || model == nullVal)
            {
                return null;
            }
            return model;
        }
        public static long? EmptyToNull(this long? model, long nullVal = -1)
        {
            if (model == null || model == nullVal)
            {
                return null;
            }
            return model;
        }

        public static DateTime? MakeEndDate(this DateTime? model)
        {
            if (model.HasValue && model.Value.Hour == 0 && model.Value.Minute == 0)
            {
                return model.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            return model;
        }

        public static DateTime MakeEndDate(this DateTime model)
        {
            if (model.Hour == 0 && model.Minute == 0)
            {
                return model.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            return model;
        }

        public static string ToPaternSearch(this string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                return "%";
            }
            return $"%{model.Trim().Replace(" ", "%")}%";
        }

        /// <summary>
        /// Кодира текст в шестнайсетичен код
        /// </summary>
        /// <param name="bytes">Текста за кодиране,
        /// като масив от байтове</param>
        /// <returns>текст в шестнайсетичен код</returns>
        public static string ToHexString(this byte[] bytes, bool lowercase = true)
        {
            var sb = new StringBuilder();
            foreach (var t in bytes)
            {
                sb.Append(t.ToString(lowercase ? "x2" : "X2"));
            }
            return sb.ToString();
        }

        public static string GetSplitedName(this string fullname, int nameIndex, string defaultNullVal = "")
        {
            if (string.IsNullOrWhiteSpace(fullname))
            {
                return fullname;
            }
            var items = fullname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0)
            {
                return string.Empty;
            }
            var firstName = items[0];
            var middleName = defaultNullVal;
            var lastName = string.Empty;
            if (items.Length > 1)
            {
                middleName = items[1];
            }
            if (items.Length > 2)
            {
                for (int i = 2; i < items.Length; i++)
                {
                    lastName += items[i] + " ";
                }
                lastName = lastName.Trim();
            }
            else
            {
                lastName = middleName;
                middleName = string.Empty;
            }

            switch (nameIndex)
            {
                case 1: return firstName;
                case 2: return middleName;
                case 3: return lastName;
                default: return string.Empty;
            }
        }

        public static long[] ToLongArray(this string values)
        {
            if (string.IsNullOrEmpty(values))
            {
                return new List<long>().ToArray();
            }
            return values.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
        }

        public static string ToInitials(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return fullName;
            }

            var splitNames = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = "";
            foreach (var splitName in splitNames)
            {
                result += splitName[0] + ".";
            }
            return result;
        }
    }
}
