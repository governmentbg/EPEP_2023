using System;
using System.Collections.Generic;
using System.Web;

namespace eCase.Web
{
    public static class Constants
    {
        public const string CaptchaModelName = "Captcha";
        public const string CookieName = "__eCase__identity__";
        public const string ProjectName = "eCase";
        public const string IsFromLoginKey = "ComesFromLoginKey";
        //public const string DownloadUrl = "/api/file/download/";
        public static string DownloadUrl
        {
            get
            {
                string fileUrl = System.Configuration.ConfigurationManager.AppSettings["eCase.Web:FileDownloadUrl"];
                if (string.IsNullOrEmpty(fileUrl))
                {
                    fileUrl = "/api/file/download/";
                }
                return fileUrl;
            }
        }

        public static string[] CriminalCaseKindCodes =
        {
            "2001",
            "2002",
            "2003",
            "2004",
            "2005",
            "2006",
            "2007",
            "2008",
            "2009"
        };

        public static int PAGE_ITEMS_COUNT
        {
            get
            {
                int result;

                string count = System.Configuration.ConfigurationManager.AppSettings["Eumis.Portal.Web:PageItemsCount"];
                if (Int32.TryParse(count, out result))
                    return result;
                else
                    return 5;
            }
        }

        public static PagedList.Mvc.PagedListRenderOptions PagedListRenderOptions
        {
            get
            {
                return new PagedList.Mvc.PagedListRenderOptions()
                {
                    ContainerDivClasses = new List<string>() { "txt-align-right" },
                    Display = PagedList.Mvc.PagedListDisplayMode.IfNeeded,
                    DisplayPageCountAndCurrentLocation = true,
                    PageCountAndCurrentLocationFormat = "Страница {0} от {1}",
                    UlElementClasses = new List<string>() { "pagination", "pagination-sm" }
                };
            }
        }
    }
}