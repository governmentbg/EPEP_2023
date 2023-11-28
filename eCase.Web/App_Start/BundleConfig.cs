using System.Web;
using System.Web.Optimization;

namespace eCase.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            

            bundles.Add(new ScriptBundle("~/bundles/upxjs").Include(
                    "~/Scripts/uxp/jquery-1.11.2.min.js",
                    "~/Scripts/uxp/bootstrap.min.js",
                    "~/Scripts/uxp/select2.min.js",
                    "~/Scripts/uxp/select2_locale_bg.js",
                    "~/Scripts/uxp/jquery.easing.1.3.js",
                    "~/Scripts/uxp/bootstrap-datepicker.min.js",
                    "~/Scripts/uxp/bootstrap-datepicker.bg.min.js",

                    // will change
                    "~/Scripts/scripts.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                    "~/Scripts/custom/jquery.custom-extensions.js",
                    "~/Scripts/custom/jquery.cascadingDropDown.js",
                    "~/Scripts/custom/html.extensions.js",
                    "~/Scripts/custom/jquery.autogrow-textarea.js",
                    "~/Scripts/custom/switchFunctions.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/angularCore").Include(
            //        "~/Scripts/angular/core/angular.js",
            //        "~/Scripts/angular/core/angular-resource.js",
            //        "~/Scripts/angular/angular-animate.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundles/angularDirectives").Include(
            //    // external
            //        "~/Scripts/angular/directives/external/jq.js",
            //        "~/Scripts/angular/directives/external/select2.js",
            //        "~/Scripts/angular/directives/external/ui-bootstrap-tpls-0.10.0.js",
            //    // eumis
            //        "~/Scripts/angular/directives/scaffolding.js",
            //        "~/Scripts/angular/directives/autoGrow.js",
            //        "~/Scripts/angular/directives/bootstrapSwitch.js",
            //        "~/Scripts/angular/directives/datepicker.js",
            //        "~/Scripts/angular/directives/disabled.js",
            //        "~/Scripts/angular/directives/enter.js",
            //        "~/Scripts/angular/directives/historyBtn.js",
            //        "~/Scripts/angular/directives/infoIcon.js",
            //        "~/Scripts/angular/directives/number.js",
            //        "~/Scripts/angular/directives/money.js",
            //        "~/Scripts/angular/directives/symbolsCount.js",
            //        "~/Scripts/angular/directives/eumisAddress/eumisAddress.js",
            //        "~/Scripts/angular/directives/nomenclature.js",
            //        "~/Scripts/angular/directives/eumisCompany/eumisCompany.js",
            //        "~/Scripts/angular/directives/confirmClick.js",
            //        "~/Scripts/angular/directives/file/fileDirective.js",
            //        "~/Scripts/angular/directives/booleanRadio/booleanRadio.js",
            //        "~/Scripts/angular/directives/nutsAddress/nutsAddress.js",
            //        "~/Scripts/angular/directives/privateNomRadio/privateNomRadio.js",
            //        "~/Scripts/angular/directives/nomRadio/nomRadio.js",
            //        "~/Scripts/angular/directives/validationPopover.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundles/angularModules").Include(

            //        "~/Scripts/angular/modules/_moduleUtils.js",
            //        "~/Scripts/angular/modules/moduleEumisNomenclature.js",
            //        "~/Scripts/angular/modules/moduleTriggers.js",
            //        "~/Scripts/angular/modules/modulePartners.js",
            //        "~/Scripts/angular/modules/moduleContract.js",
            //        "~/Scripts/angular/modules/moduleAttachedDocuments.js",
            //        "~/Scripts/angular/modules/moduleIndicators.js",
            //        "~/Scripts/angular/modules/moduleContractTeamCollection.js",
            //        "~/Scripts/angular/modules/moduleProgrammeContractActivities.js",
            //        "~/Scripts/angular/modules/moduleProjectErrandCollection.js",
            //        "~/Scripts/angular/modules/moduleDimensionsBudgetContract.js",
            //        "~/Scripts/angular/modules/modulePaperAttachedDocuments.js",
            //        "~/Scripts/angular/modules/moduleNutsAddress.js",
            //        "~/Scripts/angular/modules/moduleCandidate.js",
            //        "~/Scripts/angular/modules/moduleEvalTableGroups.js",
            //        "~/Scripts/angular/modules/moduleEvalSheetGroups.js",
            //        "~/Scripts/angular/modules/moduleProjectSpecFields.js",
            //        "~/Scripts/angular/modules/moduleProjectBasicData.js",
            //        "~/Scripts/angular/modules/moduleAttachedSignatures.js",
            //        "~/Scripts/angular/modules/modulePreliminaryContract.js",
            //        "~/Scripts/angular/modules/modulePreliminaryContractActivities.js",

            //        "~/Scripts/angular/modules/moduleBFPContract.js",

            //        "~/Scripts/angular/modules/moduleProcurementsInitial.js",
            //        "~/Scripts/angular/modules/moduleProcurementsChange.js"
            //    ));

            

            var upxcss = new StyleBundle("~/bundles/upxcss")
                .Include("~/Content/css/bootstrap.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/select2/select2.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/bootstrap-datepicker3.standalone.min.css", new CssRewriteUrlTransform())
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
