using System.Web;
using System.Web.Optimization;

namespace VSS.Portal
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/upxjs").Include(
                "~/Scripts/jquery-1.11.2.min.js",
                "~/Scripts/select2.min.js",
                "~/Scripts/select2_locale_bg.js",
  	            "~/Scripts/jquery.easing.1.3.js",
  	            "~/Scripts/scripts.js"
               ));

            //bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
            //        "~/Scripts/custom/jquery.custom-extensions.js"
            //    ));

            var upxcss = new StyleBundle("~/bundles/upxcss")
                .Include("~/Content/css/bootstrap.min.css", new CssRewriteUrlTransform())
	            .Include("~/Content/css/select2.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/style.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/custom-style.css", new CssRewriteUrlTransform());

            upxcss.Orderer = new AsDefinedBundleOrderer();

            bundles.Add(upxcss);
        }

        public class AsDefinedBundleOrderer : IBundleOrderer
        {
            #region IBundleOrderer Members

            public System.Collections.Generic.IEnumerable<BundleFile> OrderFiles(BundleContext context, System.Collections.Generic.IEnumerable<BundleFile> files)
            {
                return files;
            }

            #endregion
        }
    }
}
