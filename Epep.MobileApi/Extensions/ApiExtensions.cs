using Epep.MobileApi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.MobileApi.Extensions
{
    public static class ApiExtensions
    {
        public static List<NomenclatureItemVM> ToSimpleNomenclature(this List<SelectListItem> list)
        {
            return list.Select(x => new NomenclatureItemVM
            {
                Text = x.Text,
                Value = x.Value,
            }).ToList();
        }

        public static int SafeCount<T>(this IQueryable<T> items) where T : class
        {
            try
            {
                return items.Select(x => new { id = 1 }).Count();
            }
            catch (Exception ex)
            {
                return items.Count();
            }
        }
    }
}
