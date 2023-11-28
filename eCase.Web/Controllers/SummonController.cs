using eCase.Common.Crypto;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using eCase.Common.Helpers;
using eCase.Data.Core;
using eCase.Common.Enums;
using eCase.Components.SummonReportGenerator;

namespace eCase.Web.Controllers
{
    [Authorize]
    public partial class SummonController : BaseController
    {
        public SummonController(IUnitOfWork unitOfWork, IEntityCodeNomsRepository<Court, EntityCodeNomVO> courtRepository
            , ISummonRepository summonRepository, ICaseRepository caseRepository, IUserRepository userRepository, IBlobStorageRepository blobStorageRepository)
            : base(summonRepository)
        {
            _unitOfWork = unitOfWork;
            _courtRepository = courtRepository;
            _caseRepository = caseRepository;
            _userRepository = userRepository;
            _blobStorageRepository = blobStorageRepository;
        }

        #region Search

        [HttpGet]
        [DecryptParametersAttribute(IdsParamName =
            new string[]
            {
                "courtCode",
                "dateFrom",
                "dateTo",
                "caseNumber",
                "caseYear",
                "type",
                "page"
            })]
        public virtual ActionResult Search(
                string courtCode = "",

                string dateFrom = "",
                string dateTo = "",

                string caseNumber = "",
                string caseYear = "",

                bool isOnlyUnread = false,

                string type = "",

                SummonsOrder order = SummonsOrder.Date,
                bool isAsc = false,

                string page = ""
            )
        {
            ModelState.Clear();

            SummonSearchVM vm = new SummonSearchVM()
            {
                CourtCode = courtCode,
                DateFrom = dateFrom,
                DateTo = dateTo,
                CaseNumber = caseNumber,
                CaseYear = caseYear,
                IsOnlyUnread = isOnlyUnread,
                Type = type,
                Order = order,
                IsAsc = isAsc
            };

            FillSelectListItems(ref vm);

            var summons = _summonRepository.GetSummonsByUserId(CurrentUser.UserID);

            if (!string.IsNullOrWhiteSpace(courtCode))
            {
                summons = summons.Where(e => e.Side.Case.Court.Code == courtCode);
            }

            if (!string.IsNullOrWhiteSpace(dateFrom))
            {
                DateTime pDateFrom = DateTime.MinValue;

                if (DateTime.TryParse(dateFrom, out pDateFrom))
                {
                    summons = summons.Where(e => e.DateCreated >= pDateFrom);
                }
            }

            if (!string.IsNullOrWhiteSpace(dateTo))
            {
                DateTime pDateTo = DateTime.MaxValue;

                if (DateTime.TryParse(dateTo, out pDateTo))
                {
                    summons = summons.Where(e => e.DateCreated <= pDateTo);
                }
            }

            if (isOnlyUnread)
            {
                summons = summons.Where(e => !e.IsRead);
            }

            var selectedType = MessageSummonTypeNomenclature.Values.FirstOrDefault(e => e.Code.Equals(type));
            if (selectedType != null)
            {
                if (selectedType.Code == MessageSummonTypeNomenclature.Summon.Code)
                {
                    summons = summons.Where(e => e.SummonType.Code == SummonTypeNomenclature.Hearing.Code);
                }
                else if (selectedType.Code == MessageSummonTypeNomenclature.Message.Code)
                {
                    summons = summons.Where(e => e.SummonType.Code != SummonTypeNomenclature.Hearing.Code);
                }
            }

            #region Cases

            summons = summons.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(caseNumber) || !string.IsNullOrWhiteSpace(caseYear))
            {

                var cases = _caseRepository.SetWithoutIncludes();

                if (!string.IsNullOrWhiteSpace(caseNumber))
                {
                    int pNumber = 0;

                    if (int.TryParse(caseNumber, out pNumber))
                    {
                        cases = cases.Where(e => e.Number == pNumber);
                    }
                }

                if (!string.IsNullOrWhiteSpace(caseYear))
                {
                    int pYear = 0;

                    if (int.TryParse(caseYear, out pYear))
                    {
                        cases = cases.Where(e => e.CaseYear == pYear);
                    }
                }

                var user = _userRepository.Find(CurrentUser.UserID);

                if (CurrentUser.IsPerson)
                {
                    cases = cases.Where(e => e.Sides.Any(s => s.PersonAssignments.Any(pa => pa.PersonRegistrationId == CurrentUser.UserID && (pa.IsActive ?? true))));
                }
                else
                {
                    cases = cases.Where(e => e.Sides.Any(s => s.LawyerAssignments.Any(la => la.LawyerId == user.LawyerRegistration.LawyerId && la.IsActive == true)));
                }

                var casesCollection = cases.AsEnumerable();

                var caseIds = casesCollection.Select(e => e.CaseId);
                var actIds = casesCollection.SelectMany(e => e.Acts.Select(act => act.ActId));
                var appealIds = casesCollection.SelectMany(e => e.Acts.SelectMany(act => act.Appeals).Select(appeal => appeal.AppealId));
                var hearingIds = casesCollection.SelectMany(e => e.Hearings.Select(hearing => hearing.HearingId));

                summons = summons.Where(e =>
                    (e.SummonType.Code == SummonTypeNomenclature.Case.Code && caseIds.Contains(e.CaseId.Value))
                    || (e.SummonType.Code == SummonTypeNomenclature.Act.Code && actIds.Contains(e.ActId.Value))
                    || (e.SummonType.Code == SummonTypeNomenclature.Appeal.Code && appealIds.Contains(e.AppealId.Value))
                    || (e.SummonType.Code == SummonTypeNomenclature.Hearing.Code && hearingIds.Contains(e.HearingId.Value))
                );

            }

            #endregion

            #region Order

            if (order == SummonsOrder.Date)
                summons = isAsc ? summons.OrderBy(e => e.DateCreated)
                                : summons.OrderByDescending(e => e.DateCreated);
            else if (order == SummonsOrder.Type)
                summons = isAsc ? summons.OrderBy(e => e.SummonType.Name)
                                : summons.OrderByDescending(e => e.SummonType.Name);
            else if (order == SummonsOrder.Side)
                summons = isAsc ? summons.OrderBy(e => e.Addressee)
                                : summons.OrderByDescending(e => e.Addressee);
            else if (order == SummonsOrder.Court)
                summons = isAsc ? summons.OrderBy(e => e.Side.Case.Court.Abbreviation)
                                : summons.OrderByDescending(e => e.Side.Case.Court.Abbreviation);
            else if (order == SummonsOrder.Case)
                summons = isAsc ? summons.OrderBy(e => e.Side.Case.Abbreviation)
                                : summons.OrderByDescending(e => e.Side.Case.Abbreviation);

            #endregion

            int innerPage = string.IsNullOrEmpty(page) ? 1 : int.Parse(page);

            vm.SearchResults = summons.ToPagedList(innerPage, Statics.MaxSummonItemsPerPage);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Search(SummonSearchVM vm)
        {
            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);

                return View(vm);
            }

            SummonSearchVM.EncryptProperties(vm);

            return RedirectToAction(ActionNames.Search, vm);
        }

        #endregion

        #region Details

        [HttpGet]
        public virtual ActionResult Details(Guid gid)
        {
            SummonDetailsVM vm = new SummonDetailsVM();

            Summon summon = _summonRepository.GetSummonByGid(gid);
            if (!summon.IsRead)
            {
                using (var transaction = _unitOfWork.BeginTransaction())
                {
                    summon.IsRead = true;
                    summon.ReadTime = DateTime.Now;

                    var docVm = new SummonDocumentVM();

                    docVm.CourtName = summon.Side.Case.Court.Name;
                    docVm.CaseKind = summon.Side.Case.CaseKind.Name;
                    docVm.CaseNumber = summon.Side.Case.Number;
                    docVm.CaseYear = summon.Side.Case.CaseYear;

                    docVm.SummonKind = summon.SummonKind;
                    docVm.DateCreated = summon.DateCreated;
                    docVm.ReadTime = summon.ReadTime.Value;
                    docVm.Addressee = summon.Addressee;

                    var content = SummonDocumentGenerator.GenerateSummonDocument(docVm);
                    Guid blobKey = _blobStorageRepository.UploadFileToBlobStorage(Guid.NewGuid(), content, MimeTypeHelper.MIME_APPLICATION_PDF, FileType.SummonReport, summon.ReadTime.Value);

                    summon.ReportBlobKey = blobKey;
                    summon.ModifyDate = DateTime.Now;

                    _unitOfWork.Save();
                    transaction.Commit();
                }
            }

            vm.Summon = summon;

            vm.HasPermissions = Request.IsAuthenticated && _summonRepository.CheckPermission(gid, CurrentUser.UserID);

            vm.HasSummonFile = summon.SummonBlobKey != null;

            return View(vm);
        }

        #endregion

        #region Files

        [HttpGet]
        public virtual RedirectResult GetSummonFile(Guid summonGid)
        {
            var summon = _summonRepository.FindByGid(summonGid);

            var summonBlob = summon.SummonBlobKey;

            if (summonBlob == null)
                return null;

            // Check permissions
            if (!Request.IsAuthenticated || !_summonRepository.CheckPermission(summonGid, CurrentUser.UserID))
                return null;

            Guid blobKey = summonBlob.Value;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetSummonReportFile(Guid summonGid)
        {
            var summon = _summonRepository.FindByGid(summonGid);

            var reportBlob = summon.ReportBlobKey;

            if (reportBlob == null)
                return null;

            // Check permissions
            if (!Request.IsAuthenticated || !_summonRepository.CheckPermission(summonGid, CurrentUser.UserID))
                return null;

            Guid blobKey = reportBlob.Value;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        #endregion

        #region Private

        private void FillSelectListItems(ref SummonSearchVM vm)
        {
            if (vm == null)
                vm = new SummonSearchVM();

            vm.Courts = ((UnitOfWork)_unitOfWork).DbContext.Set<Court>().Where(e => e.IsIntegrated).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });

            vm.CaseYears = _years;

            vm.Types = MessageSummonTypeNomenclature.Values.Select(e => new SelectListItem() { Value = e.Code, Text = e.Text });
        }

        private IEnumerable<SelectListItem> _years = Enumerable.Range(DateTime.Now.Year - 30, 31).OrderByDescending(e => e).Select(e => new SelectListItem() { Value = e.ToString(), Text = e.ToString() });

        private IUnitOfWork _unitOfWork;
        private IEntityCodeNomsRepository<Court, EntityCodeNomVO> _courtRepository;
        private ICaseRepository _caseRepository;
        private IUserRepository _userRepository;
        private IBlobStorageRepository _blobStorageRepository;

        #endregion
    }
}