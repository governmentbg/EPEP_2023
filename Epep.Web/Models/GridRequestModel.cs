using Epep.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Epep.Web.Controllers
{
    public class GridRequestModel
    {
        public int page { get; set; }
        public int size { get; set; }
        public string filter { get; set; }
        public string exportFormat { get; set; }
        public string data { get; set; }
    }

    public class GridViewConstants
    {
        public class ExportFormats
        {
            public const string Excel = "xls";
        }
    }

    public static class GridViewExtensions
    {
        public static GridResponseModel BuildResponse<T>(this GridRequestModel request, IQueryable<T> source) where T : class
        {
            var response = new GridResponseModel()
            {
                page = request.page,
                size = request.size,
            };
            if (source != null && request.size > 0)
            {
                response.total_rows = getCount(source);
                response.total_pages = (int)Math.Ceiling((decimal)response.total_rows / request.size);
                var dataPage = source.Skip((request.page - 1) * request.size).Take(request.size).ToList();
                response.data = dataPage;
            }
            return response;
        }

        public static IActionResult GetResponse<T>(this GridRequestModel request, IQueryable<T> source) where T : class
        {
            var response = BuildResponse(request, source);
            return new JsonResult(response);
        }

        private static int getCount<T>(IQueryable<T> source) where T : class
        {
            try
            {
                return source.Select(x => new { i = 0 }).Count();
            }
            catch
            {
                return source.Count();
            }
        }

        public static T GetData<T>(this GridRequestModel request)
        {
            if (request.data == null)
            {
                return (T)Activator.CreateInstance(typeof(T), false);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(request.data, new IsoDateTimeConverter { DateTimeFormat = NomenclatureConstants.NormalDateFormat });
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }

    public class GridResponseModel
    {
        public int page { get; set; }
        public int size { get; set; }
        public int total_rows { get; set; }
        public int total_pages { get; set; }
        public object data { get; set; }

        //private object _data;
        //public object data
        //{
        //    get
        //    {
        //        return _data ?? pagedData;
        //    }
        //    set
        //    {
        //        _data = value;
        //    }
        //}
        //public IEnumerable<T> pagedData { get; set; }
        //public GridResponseModel(GridRequestModel request, IQueryable<T> source, bool storeToPagedData = false)
        //{
        //    this.page = request.page;
        //    this.size = request.size;

        //    if (source != null && request.size > 0)
        //    {
        //        total_pages = (int)Math.Ceiling((decimal)source.Count() / size);
        //    }

        //    if (storeToPagedData)
        //    {
        //        pagedData = source.Skip((page - 1) * size).Take(size).ToArray();
        //    }
        //    else
        //    {
        //        data = source.Skip((page - 1) * size).Take(size);
        //    }
        //}

    }
}
