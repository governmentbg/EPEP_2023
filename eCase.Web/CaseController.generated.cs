// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace eCase.Web.Controllers
{
    public partial class CaseController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CaseController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Details()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Details);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.RedirectResult GetAssignmentFile()
        {
            return new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetAssignmentFile);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.RedirectResult GetScannedFile()
        {
            return new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetScannedFile);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.RedirectResult GetIncomingDocumentFile()
        {
            return new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetIncomingDocumentFile);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.RedirectResult GetOutgoingDocumentFile()
        {
            return new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetOutgoingDocumentFile);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.JsonResult GetLawyer()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetLawyer);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.JsonResult GetLawyers()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetLawyers);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CaseController Actions { get { return MVC.Case; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Case";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Case";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Search = "Search";
            public readonly string Details = "Details";
            public readonly string GetAssignmentFile = "GetAssignmentFile";
            public readonly string GetScannedFile = "GetScannedFile";
            public readonly string GetIncomingDocumentFile = "GetIncomingDocumentFile";
            public readonly string GetOutgoingDocumentFile = "GetOutgoingDocumentFile";
            public readonly string GetLawyer = "GetLawyer";
            public readonly string GetLawyers = "GetLawyers";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Search = "Search";
            public const string Details = "Details";
            public const string GetAssignmentFile = "GetAssignmentFile";
            public const string GetScannedFile = "GetScannedFile";
            public const string GetIncomingDocumentFile = "GetIncomingDocumentFile";
            public const string GetOutgoingDocumentFile = "GetOutgoingDocumentFile";
            public const string GetLawyer = "GetLawyer";
            public const string GetLawyers = "GetLawyers";
        }


        static readonly ActionParamsClass_Search s_params_Search = new ActionParamsClass_Search();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Search SearchParams { get { return s_params_Search; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Search
        {
            public readonly string incomingNumber = "incomingNumber";
            public readonly string number = "number";
            public readonly string year = "year";
            public readonly string predecessorNumber = "predecessorNumber";
            public readonly string predecessorYear = "predecessorYear";
            public readonly string caseKindId = "caseKindId";
            public readonly string sideName = "sideName";
            public readonly string courtCode = "courtCode";
            public readonly string lawyerId = "lawyerId";
            public readonly string actKindId = "actKindId";
            public readonly string actNumber = "actNumber";
            public readonly string actYear = "actYear";
            public readonly string order = "order";
            public readonly string isAsc = "isAsc";
            public readonly string showResults = "showResults";
            public readonly string page = "page";
            public readonly string vm = "vm";
            public readonly string captchaValid = "captchaValid";
        }
        static readonly ActionParamsClass_Details s_params_Details = new ActionParamsClass_Details();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Details DetailsParams { get { return s_params_Details; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Details
        {
            public readonly string gid = "gid";
            public readonly string sumPage = "sumPage";
            public readonly string actSumPage = "actSumPage";
            public readonly string appSumPage = "appSumPage";
            public readonly string hearSumPage = "hearSumPage";
            public readonly string hPage = "hPage";
            public readonly string aPage = "aPage";
            public readonly string asPage = "asPage";
            public readonly string ccPage = "ccPage";
            public readonly string idPage = "idPage";
            public readonly string odPage = "odPage";
            public readonly string sdPage = "sdPage";
        }
        static readonly ActionParamsClass_GetAssignmentFile s_params_GetAssignmentFile = new ActionParamsClass_GetAssignmentFile();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetAssignmentFile GetAssignmentFileParams { get { return s_params_GetAssignmentFile; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetAssignmentFile
        {
            public readonly string assignmentGid = "assignmentGid";
        }
        static readonly ActionParamsClass_GetScannedFile s_params_GetScannedFile = new ActionParamsClass_GetScannedFile();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetScannedFile GetScannedFileParams { get { return s_params_GetScannedFile; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetScannedFile
        {
            public readonly string scannedFileGid = "scannedFileGid";
        }
        static readonly ActionParamsClass_GetIncomingDocumentFile s_params_GetIncomingDocumentFile = new ActionParamsClass_GetIncomingDocumentFile();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetIncomingDocumentFile GetIncomingDocumentFileParams { get { return s_params_GetIncomingDocumentFile; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetIncomingDocumentFile
        {
            public readonly string incomingDocumentGid = "incomingDocumentGid";
        }
        static readonly ActionParamsClass_GetOutgoingDocumentFile s_params_GetOutgoingDocumentFile = new ActionParamsClass_GetOutgoingDocumentFile();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetOutgoingDocumentFile GetOutgoingDocumentFileParams { get { return s_params_GetOutgoingDocumentFile; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetOutgoingDocumentFile
        {
            public readonly string outgoingDocumentGid = "outgoingDocumentGid";
        }
        static readonly ActionParamsClass_GetLawyer s_params_GetLawyer = new ActionParamsClass_GetLawyer();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetLawyer GetLawyerParams { get { return s_params_GetLawyer; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetLawyer
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_GetLawyers s_params_GetLawyers = new ActionParamsClass_GetLawyers();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetLawyers GetLawyersParams { get { return s_params_GetLawyers; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetLawyers
        {
            public readonly string term = "term";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string Details = "Details";
                public readonly string Search = "Search";
            }
            public readonly string Details = "~/Views/Case/Details.cshtml";
            public readonly string Search = "~/Views/Case/Search.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_CaseController : eCase.Web.Controllers.CaseController
    {
        public T4MVC_CaseController() : base(Dummy.Instance) { }

        [NonAction]
        partial void SearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string incomingNumber, string number, string year, string predecessorNumber, string predecessorYear, string caseKindId, string sideName, string courtCode, string lawyerId, string actKindId, string actNumber, string actYear, eCase.Web.Helpers.CasesOrder order, bool isAsc, bool showResults, string page);

        [NonAction]
        public override System.Web.Mvc.ActionResult Search(string incomingNumber, string number, string year, string predecessorNumber, string predecessorYear, string caseKindId, string sideName, string courtCode, string lawyerId, string actKindId, string actNumber, string actYear, eCase.Web.Helpers.CasesOrder order, bool isAsc, bool showResults, string page)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Search);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "incomingNumber", incomingNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "number", number);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "year", year);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "predecessorNumber", predecessorNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "predecessorYear", predecessorYear);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "caseKindId", caseKindId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sideName", sideName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "courtCode", courtCode);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "lawyerId", lawyerId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "actKindId", actKindId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "actNumber", actNumber);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "actYear", actYear);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "order", order);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "isAsc", isAsc);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "showResults", showResults);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "page", page);
            SearchOverride(callInfo, incomingNumber, number, year, predecessorNumber, predecessorYear, caseKindId, sideName, courtCode, lawyerId, actKindId, actNumber, actYear, order, isAsc, showResults, page);
            return callInfo;
        }

        [NonAction]
        partial void SearchOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, eCase.Web.Models.Case.CaseSearchVM vm, bool? captchaValid);

        [NonAction]
        public override System.Web.Mvc.ActionResult Search(eCase.Web.Models.Case.CaseSearchVM vm, bool? captchaValid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Search);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "vm", vm);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "captchaValid", captchaValid);
            SearchOverride(callInfo, vm, captchaValid);
            return callInfo;
        }

        [NonAction]
        partial void DetailsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.Guid gid, int? sumPage, int? actSumPage, int? appSumPage, int? hearSumPage, int? hPage, int? aPage, int? asPage, int? ccPage, int? idPage, int? odPage, int? sdPage);

        [NonAction]
        public override System.Web.Mvc.ActionResult Details(System.Guid gid, int? sumPage, int? actSumPage, int? appSumPage, int? hearSumPage, int? hPage, int? aPage, int? asPage, int? ccPage, int? idPage, int? odPage, int? sdPage)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Details);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "gid", gid);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sumPage", sumPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "actSumPage", actSumPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "appSumPage", appSumPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "hearSumPage", hearSumPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "hPage", hPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "aPage", aPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "asPage", asPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ccPage", ccPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "idPage", idPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "odPage", odPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sdPage", sdPage);
            DetailsOverride(callInfo, gid, sumPage, actSumPage, appSumPage, hearSumPage, hPage, aPage, asPage, ccPage, idPage, odPage, sdPage);
            return callInfo;
        }

        [NonAction]
        partial void GetAssignmentFileOverride(T4MVC_System_Web_Mvc_RedirectResult callInfo, System.Guid assignmentGid);

        [NonAction]
        public override System.Web.Mvc.RedirectResult GetAssignmentFile(System.Guid assignmentGid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetAssignmentFile);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "assignmentGid", assignmentGid);
            GetAssignmentFileOverride(callInfo, assignmentGid);
            return callInfo;
        }

        [NonAction]
        partial void GetScannedFileOverride(T4MVC_System_Web_Mvc_RedirectResult callInfo, System.Guid scannedFileGid);

        [NonAction]
        public override System.Web.Mvc.RedirectResult GetScannedFile(System.Guid scannedFileGid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetScannedFile);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "scannedFileGid", scannedFileGid);
            GetScannedFileOverride(callInfo, scannedFileGid);
            return callInfo;
        }

        [NonAction]
        partial void GetIncomingDocumentFileOverride(T4MVC_System_Web_Mvc_RedirectResult callInfo, System.Guid incomingDocumentGid);

        [NonAction]
        public override System.Web.Mvc.RedirectResult GetIncomingDocumentFile(System.Guid incomingDocumentGid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetIncomingDocumentFile);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "incomingDocumentGid", incomingDocumentGid);
            GetIncomingDocumentFileOverride(callInfo, incomingDocumentGid);
            return callInfo;
        }

        [NonAction]
        partial void GetOutgoingDocumentFileOverride(T4MVC_System_Web_Mvc_RedirectResult callInfo, System.Guid outgoingDocumentGid);

        [NonAction]
        public override System.Web.Mvc.RedirectResult GetOutgoingDocumentFile(System.Guid outgoingDocumentGid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_RedirectResult(Area, Name, ActionNames.GetOutgoingDocumentFile);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "outgoingDocumentGid", outgoingDocumentGid);
            GetOutgoingDocumentFileOverride(callInfo, outgoingDocumentGid);
            return callInfo;
        }

        [NonAction]
        partial void GetLawyerOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, long id);

        [NonAction]
        public override System.Web.Mvc.JsonResult GetLawyer(long id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetLawyer);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            GetLawyerOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void GetLawyersOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, string term);

        [NonAction]
        public override System.Web.Mvc.JsonResult GetLawyers(string term)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetLawyers);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "term", term);
            GetLawyersOverride(callInfo, term);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
