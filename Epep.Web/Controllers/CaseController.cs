using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using IO.Timestamp.Client.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMART.Extensions;

namespace Epep.Web.Controllers
{
    public class CaseController : BaseController
    {
        private readonly ICaseService caseService;
        private readonly INomenclatureService nomService;
        private readonly IDocumentService docService;
        private readonly IUserService userService;
        private readonly IUserContext userContext;
        private readonly DateTime UpgradeEpepDateStart;


        public CaseController(
            ICaseService _caseService,
            INomenclatureService _nomService,
            IDocumentService _docService,
            IUserService _userService,
            IConfiguration config,
            IUserContext _userContext)
        {
            caseService = _caseService;
            nomService = _nomService;
            docService = _docService;
            userService = _userService;
            userContext = _userContext;

            UpgradeEpepDateStart = config.GetValue<DateTime>("UpgradeEpepDateStart", DateTime.Now.AddMonths(-1));
        }
        public async Task<IActionResult> Index()
        {
            var filter = new FilterCaseVM()
            {
                MyCasesOnly = false
            };
            await SetViewBagIndex();
            return View(filter);
        }


        [Authorize]
        public async Task<IActionResult> MyCases()
        {
            await SetViewBagIndex();
            var filter = new FilterCaseVM()
            {
                MyCasesOnly = true
            };
            return View(nameof(Index), filter);
        }



        [HttpPost]
        public async Task<IActionResult> LoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterCaseVM>();
            filter.MyCasesOnly = false;
            filter.NoOrgUserCases = false;
            var data = await caseService.SelectCase(filter);
            var responce = request.BuildResponse(data);
            postProcessNamesCase(true, responce);
            return Json(responce);
        }

        private void postProcessNamesCase(bool isPublic, GridResponseModel responce)
        {
            if (responce == null || responce.data == null)
            {
                return;
            }
            if (NomenclatureConstants.UserTypes.GlobalAdmin == userContext.UserType)
            {
                return;
            }
            var cases = (List<CaseVM>)responce.data;
            foreach (var item in cases)
            {
                caseService.ProcessCaseNames(item, UpgradeEpepDateStart, isPublic, userContext.UserType, userContext.CourtId);
            }
            responce.data = cases;
        }
        
        

        private async Task SetViewBagIndex(bool forHearings = false)
        {
            ViewBag.CourtId_ddl = (await nomService.GetDDL_Courts()).PrependAllItem();
            ViewBag.CaseKindId_ddl = (await nomService.GetDDL_CaseKind()).PrependAllItem();
            if (!forHearings)
            {
                ViewBag.ActKindId_ddl = (await nomService.GetDDL_ActKinds()).PrependAllItem();
            }
            var years = nomService.GetDDL_CaseYears().PrependAllItem();
            ViewBag.RegYear_ddl = years;
            ViewBag.PrevYear_ddl = years;
            ViewBag.ActYear_ddl = years;
        }


        [Authorize]
        public async Task<IActionResult> Desktop()
        {
            ViewBag.nextHearings = await caseService.SelectNextHearingsByUser().Take(3).ToListAsync();
            ViewBag.newSummons = await caseService.SelectLastSummonsByUser().Where(x => x.IsRead == false).Select(x => x.IsRead).CountAsync();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> OnlineHearings()
        {
            var filter = new FilterHearingVM();
            await SetViewBagIndex(true);
            return View(filter);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> OnlineHearingsLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterHearingVM>();
            var data = await caseService.SelectHearingOnline(filter);
            return request.GetResponse(data);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DesktopSummonsLoadData([FromBody] GridRequestModel request)
        {
            var data = caseService.SelectLastSummonsByUser();
            return request.GetResponse(data);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DesktopLastCasesLoadData([FromBody] GridRequestModel request)
        {
            var data = caseService.SelectLastViewedCases();
            return request.GetResponse(data);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FocusCasesLoadData([FromBody] GridRequestModel request)
        {
            var data = await caseService.SelectCase(new FilterCaseVM()
            {
                MyCasesOnly = true,
                FocusCasesOnly = true
            });
            return request.GetResponse(data.AsQueryable());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ArchiveCasesLoadData([FromBody] GridRequestModel request)
        {
            var data = await caseService.SelectCase(new FilterCaseVM()
            {
                MyCasesOnly = true,
                ArchiveCasesOnly = true,
                NoOrgUserCases = false
            });
            return request.GetResponse(data.AsQueryable());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MyCasesLoadData([FromBody] GridRequestModel request)
        {
            var filter = request.GetData<FilterCaseVM>();
            filter.MyCasesOnly = true;
            var data = await caseService.SelectCase(filter);
            return request.GetResponse(data.AsQueryable());
        }

        public async Task<IActionResult> CaseDetail(Guid gid)
        {
            var model = await (await caseService.SelectCase(new FilterCaseVM()
            {
                CaseGid = gid
            })).FirstOrDefaultAsync();

            if (model == null)
            {
                return RedirectToAction(nameof(HomeController.NotFound), "Home");
            }
            ViewBag.isUserAccess = (await caseService.CheckCaseAccess(gid));

            bool isGlobalAccess = IsGlobalAccess(model.CourtId, model.RestrictedAccess ?? true);

            ViewBag.isGlobalAccess = isGlobalAccess;
            ViewBag.UpgradeEpepDateStart = UpgradeEpepDateStart;

            if ((bool)ViewBag.isUserAccess || isGlobalAccess)
            {
                ViewBag.hasScannedFiles = await caseService.HasScannedFiles(gid);
            }

            caseService.ProcessCaseNames(model, UpgradeEpepDateStart, !ViewBag.isUserAccess, userContext.UserType, userContext.CourtId);

            if (NomenclatureConstants.UserTypes.LogCaseViewTypes.Contains(userContext.UserType) && model != null)
            {
                int viewMode = NomenclatureConstants.FocusTypes.View;
                if (ViewBag.isUserAccess == false && userContext.UserType == NomenclatureConstants.UserTypes.Lawyer)
                {
                    viewMode = NomenclatureConstants.FocusTypes.LawyerView;
                }
                await caseService.SaveCaseView(gid, viewMode);
            }
            return View(model);
        }

        private async Task<bool> IsGlobalAccess(Guid caseGid)
        {
            var model = await caseService.GetByGidAsync<Core.Models.Case>(caseGid);
            if (model == null) { return false; }
            return IsGlobalAccess(model.CourtId, model.RestrictedAccess ?? true);
        }
        private bool IsGlobalAccess(long courtId, bool restrictedAccess)
        {
            bool isGlobalAccess = false;
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.GlobalAdmin:
                    isGlobalAccess = true;
                    break;
                case NomenclatureConstants.UserTypes.Lawyer:
                    isGlobalAccess = !restrictedAccess;
                    break;
                case NomenclatureConstants.UserTypes.CourtAdmin:
                    isGlobalAccess = courtId == userContext.CourtId;
                    break;
            }
            return isGlobalAccess;
        }

        [Authorize]
        public async Task<IActionResult> ToggleFocus(Guid gid)
        {
            var toggleResult = await caseService.ToggleFocusCase(gid, NomenclatureConstants.FocusTypes.Focus);
            if (toggleResult.Result)
            {
                SetSuccessMessage(SaveResultVM.MessageSaveOk);
            }

            return RedirectToAction(nameof(CaseDetail), new { gid });
        }

        [Authorize]
        public async Task<IActionResult> ToggleArchive(Guid gid)
        {
            var toggleResult = await caseService.ToggleFocusCase(gid, NomenclatureConstants.FocusTypes.Archive);
            if (toggleResult.Result)
            {
                SetSuccessMessage(SaveResultVM.MessageSaveOk);
            }

            return RedirectToAction(nameof(CaseDetail), new { gid });
        }

        [HttpPost]
        public async Task<IActionResult> DocumentsLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectDocumentsByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }
        [HttpPost]
        public async Task<IActionResult> SidesLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectSidesByCaseGid(caseGid);
            var responce = request.BuildResponse(data);
            responce.data = await caseService.ProcessSideNames((List<SideVM>)responce.data, UpgradeEpepDateStart, caseGid.Gid, userContext.UserType, userContext.CourtId);
            //var isPublic = !(await caseService.CheckCaseAccess(caseGid.Gid));
            //bool isGlobalAccess = await IsGlobalAccess(caseGid.Gid);


            //if (isPublic && !isGlobalAccess)
            //{
            //    postProcessNamesSide(responce);
            //}
            return Json(responce);
        }
        [HttpPost]
        public async Task<IActionResult> AssignmentsLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectAssignmentsByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }
        [HttpPost]
        public async Task<IActionResult> HearingsLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectHearingsByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }

        [HttpPost]
        public async Task<IActionResult> ActsLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectActsByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }

        [HttpPost]
        public async Task<IActionResult> UsersWithAccessLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectUsersWithAccessByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }


        [HttpPost]
        public async Task<IActionResult> ConnectedCaseLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            var data = await caseService.SelectConnectedCaseByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SummonsCaseLoadData([FromBody] GridRequestModel request)
        {
            var caseGid = request.GetData<GidLoaderVM>();
            if (!(await caseService.CheckCaseAccess(caseGid.Gid)))
            {
                return null;
            }
            var data = await caseService.SelectSummonsByCaseGid(caseGid);
            return request.GetResponse(data.AsQueryable());
        }

        public async Task<IActionResult> ChronologyTimelineByCase(GidLoaderVM loader)
        {
            var data = await caseService.SelectChronologyByCase(loader);
            return PartialView("_ChronologyTimeline", data.ToList());
        }

        public async Task<IActionResult> Preview(int type, Guid gid, string backUrl = null, string backCanvasUrl = null)
        {
            switch (type)
            {
                case NomenclatureConstants.SourceTypes.Hearing:
                    return await previewHearing(gid);
                case NomenclatureConstants.SourceTypes.Summon:
                    return await previewSummon(gid, backUrl, backCanvasUrl);
                case NomenclatureConstants.SourceTypes.Act:
                    {
                        ViewBag.participants = (await caseService.SelectActPreparatorsByAct(new GidLoaderVM()
                        {
                            Gid = gid
                        }));
                    }
                    break;
                default:
                    break;
            }

            var model = await caseService.LoadFilesForObject(type, gid);
            if (model == null)
            {
                return Content("null");
            }
            model.BackUrl = backUrl;
            model.BackCanvasUrl = backCanvasUrl;
            return PartialView("_PreviewDetails", model);
        }

        [Authorize]
        private async Task<IActionResult> previewSummon(Guid gid, string backUrl = null, string backCanvasUrl = null)
        {
            var model = await caseService.LoadFilesForObject(NomenclatureConstants.SourceTypes.Summon, gid);
            if (model == null)
            {
                return Content("null");
            }

            if (model.ForAction)
            {
                var summonService = HttpContext.RequestServices.GetService<ISummonService>();
                var timestampClient = HttpContext.RequestServices.GetService<ITimestampClient>();

                DateTime ReadTime = DateTime.Now;
                var reportModel = await summonService.GetSummonDocumentInfo(model.ObjectGid);
                byte[] tsr = null;
                try
                {
                    if (reportModel.SummonContent != null)
                    {
                        (tsr, ReadTime) = await timestampClient.GetTimestampAsync(reportModel.SummonContent);
                    }
                }
                catch (Exception ex)
                {
                }
                reportModel.ReadTime = ReadTime.ToLocalTime();
                reportModel.UserNameRead = userContext.FullName;
                reportModel.UserEmailRead = userContext.Email;
                var bytes = await (new ViewAsPdfByteWriter("_SummonDocument", reportModel, true)).GetByte(this.ControllerContext);

                await summonService.SetSummonAsRead(model.ObjectGid, reportModel.ReadTime, bytes, tsr);
                model = await caseService.LoadFilesForObject(NomenclatureConstants.SourceTypes.Summon, gid);
                model.ActionMode = "reload-summon";
            }
            model.BackUrl = backUrl;
            model.BackCanvasUrl = backCanvasUrl;
            return PartialView("_PreviewDetails", model);
        }

        private async Task<IActionResult> previewHearing(Guid gid)
        {
            var data = (await caseService.SelectHearingsByCaseGid(new GidLoaderVM()
            {
                ObjectGid = gid
            })).FirstOrDefault();
            ViewBag.participants = (await caseService.SelectHearingParticipantByHearing(new GidLoaderVM()
            {
                Gid = gid
            }));
            ViewBag.items = (await caseService.SelectHearingItemsByHearing(new GidLoaderVM()
            {
                Gid = gid
            }));
            ViewBag.protocolFiles = await caseService.SelectHearingProtocolByHearingGid(data.Gid);
            return PartialView("_HearingDetails", data);
        }

        [Authorize]
        public async Task<IActionResult> Calendar()
        {
            ViewBag.todayItems = await caseService.SelectCalendarByUser(DateTime.Now.Date, DateTime.Now.Date);
            if (userContext.UserType == NomenclatureConstants.UserTypes.Lawyer)
            {
                ViewBag.nextVacations = await userService.UserVacation_Select().Where(x => x.DateTo > DateTime.Now).OrderBy(x => x.DateFrom).Take(3).ToArrayAsync();
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> LoadCalendar(DateTime start, DateTime end)
        {
            var events = await caseService.SelectCalendarByUser(start, end);
            return Json(events);
        }

        [Authorize]
        public async Task<IActionResult> GetCalendarDate(DateTime date)
        {
            var model = await caseService.GetCalendarDateEvents(date);
            return Json(model);
        }

        [Authorize]
        public async Task<IActionResult> UserAccess(Guid gid)
        {
            var model = await userService.ManageAccessForSide(gid);
            return PartialView("~/Views/User/_UserAccess.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserAccess(UserAccessVM model)
        {
            var result = new SaveResultVM();
            if (!model.HasAccess && model.RequestAccess == 0)
            {
                result.AddError("Моля, изберете", nameof(model.RequestAccess));
            }

            var description = await userService.ManageAccessGetDescription(model);
            if (description != null)
            {
                var docResult = await docService.InitDocument(model.CaseGid, model.SideGid, description);
                if (docResult.Result)
                {
                    result.ObjectId = docResult.ObjectId;
                }
                else
                {
                    result.AddError(docResult.Message);
                }
            }

            return Json(result);
        }
    }
}
