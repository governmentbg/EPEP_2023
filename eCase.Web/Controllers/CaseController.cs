using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using eCase.Common.Captcha;
using eCase.Common.Crypto;
using eCase.Common.Enums;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Case;

using PagedList;
using eCase.Data.Core;
using System.Data.Entity;

namespace eCase.Web.Controllers
{
    public partial class CaseController : BaseController
    {
        public CaseController(
                IUnitOfWork unitOfWork,
                ICaseRepository caseRepository,
                IHearingRepository hearingRepository,
                IActRepository actRepository,
                IAppealRepository appealRepository,
                ILawyerRepository lawyerRepository,
                IAssignmentRepository assignmentRepository,
                ICaseRulingRepository caseRulingRepository,
                IIncomingDocumentRepository incomingDocumentRepository,
                IOutgoingDocumentRepository outgoingDocumentRepository,
                IScannedFileRepository scannedFileRepository,
                IAttachedDocumentRepository attachedDocumentRepository,
                IEntityCodeNomsRepository<CaseKind, EntityCodeNomVO> caseKindRepository,
                IEntityCodeNomsRepository<ActKind, EntityCodeNomVO> actKindRepository,
                IUserRepository userRepository,
                ISummonRepository summonRepository)
            : base(summonRepository)
        {
            _unitOfWork = unitOfWork;
            _caseRepository = caseRepository;
            _hearingRepository = hearingRepository;
            _actRepository = actRepository;
            _appealRepository = appealRepository;
            _lawyerRepository = lawyerRepository;
            _assignmentRepository = assignmentRepository;
            _caseRulingRepository = caseRulingRepository;
            _incomingDocumentRepository = incomingDocumentRepository;
            _outgoingDocumentRepository = outgoingDocumentRepository;
            _scannedFileRepository = scannedFileRepository;
            _caseKindRepository = caseKindRepository;
            _actKindRepository = actKindRepository;
            _userRepository = userRepository;
            _attachedDocumentRepository = attachedDocumentRepository;
        }

        #region Search

        [HttpGet]
        [DecryptParametersAttribute(IdsParamName =
            new string[]
            {
                    "incomingNumber",
                    "number",
                    "year",
                    "predecessorNumber",
                    "predecessorYear",
                    "caseKindId",
                    "sideName",
                    "courtCode",
                    "lawyerId",
                    "actKindId",
                    "actNumber",
                    "actYear",
                    "page"
            })]
        public virtual ActionResult Search(
                string incomingNumber = "",
                string number = "",
                string year = "",
                string predecessorNumber = "",
                string predecessorYear = "",
                string caseKindId = "",
                string sideName = "",
                string courtCode = "",
                string lawyerId = "",
                string actKindId = "",
                string actNumber = "",
                string actYear = "",

                CasesOrder order = CasesOrder.Case,
                bool isAsc = false,

                bool showResults = false,
                string page = ""
                )
        {
            ModelState.Clear();

            CaseSearchVM vm = new CaseSearchVM()
            {
                IncomingNumber = incomingNumber,
                Number = number,
                Year = year,
                PredecessorNumber = predecessorNumber,
                PredecessorYear = predecessorYear,
                CaseKindId = caseKindId,
                SideName = sideName,
                CourtCode = courtCode,
                LawyerId = lawyerId,
                ActKindId = actKindId,
                ActNumber = actNumber,
                ActYear = actYear,

                ShowResults = showResults,

                Order = order,
                IsAsc = isAsc
            };

            FillSelectListItems(ref vm);

            #region show results

            // filter only assigned cases
            // auto search for lawyer and person
            if (Request.IsAuthenticated && (CurrentUser.IsLawyer || CurrentUser.IsPerson))
            {
                vm.ShowResults = true;
                vm.AreOnlyPersonalCases = true;
                vm.HasPersonalCases = true;
            }

            if (vm.ShowResults)
            {
                var cases = _caseRepository.SetWithoutIncludes();

                //if (vm.RestrictedAccess && !Request.IsAuthenticated && !(CurrentUser.IsSuperAdmin || CurrentUser.IsCourtAdmin)) //comment for PROD
                //if (!Request.IsAuthenticated && !(CurrentUser.IsSuperAdmin || CurrentUser.IsCourtAdmin))
                //{
                //    vm.ShowResults = true;
                //    vm.AreOnlyPersonalCases = true;
                //    vm.HasPersonalCases = true;
                //}

                if (!string.IsNullOrWhiteSpace(courtCode))
                {
                    cases = cases.Where(e => e.Court.Code == courtCode);
                }
                else
                {
                    var courtCodes = vm.Courts.Select(e => e.Value);
                    cases = cases.Where(e => courtCodes.Contains(e.Court.Code));
                }

                if (!string.IsNullOrWhiteSpace(caseKindId))
                {
                    int pCaseKindId;

                    if (int.TryParse(caseKindId, out pCaseKindId))
                    {
                        cases = cases.Where(e => e.CaseKindId == pCaseKindId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(incomingNumber))
                {
                    int pIncomingNumber = 0;

                    if (int.TryParse(incomingNumber, out pIncomingNumber))
                    {
                        cases = cases.Where(e => e.IncomingDocument.IncomingNumber == pIncomingNumber);
                    }
                }

                if (!string.IsNullOrWhiteSpace(number))
                {
                    int pNumber = 0;

                    if (int.TryParse(number, out pNumber))
                    {
                        cases = cases.Where(e => e.Number == pNumber);
                    }
                }

                if (!string.IsNullOrWhiteSpace(year))
                {
                    int pYear = 0;

                    if (int.TryParse(year, out pYear))
                    {
                        cases = cases.Where(e => e.CaseYear == pYear);
                    }
                }

                #region Predecessors logic

                int pPredecessorNumber = 0;
                bool hasPredecessorNumber = !string.IsNullOrWhiteSpace(predecessorNumber) && int.TryParse(predecessorNumber, out pPredecessorNumber);

                int pPredecessorYear = 0;
                bool hasPredecessorYear = !string.IsNullOrWhiteSpace(predecessorYear) && int.TryParse(predecessorYear, out pPredecessorYear);

                if (hasPredecessorNumber || hasPredecessorYear)
                {
                    var predecessorCases = _caseRepository.GetAllPredecessorCases();

                    if (hasPredecessorNumber)
                    {
                        predecessorCases = predecessorCases.Where(e => e.Number == pPredecessorNumber);
                    }
                    if (hasPredecessorYear)
                    {
                        predecessorCases = predecessorCases.Where(e => e.CaseYear == pPredecessorYear);
                    }

                    _caseRepository.UpdateCasesWithPredecessors(predecessorCases, ref cases);
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(actKindId))
                {
                    int pActKindId;

                    if (int.TryParse(actKindId, out pActKindId))
                    {
                        cases = cases.Where(e => e.Acts.Any(a => a.ActKindId == pActKindId));
                    }
                }

                if (!string.IsNullOrWhiteSpace(actNumber))
                {
                    int pActNumber = 0;

                    if (int.TryParse(actNumber, out pActNumber))
                    {
                        cases = cases.Where(e => e.Acts.Any(a => a.Number == pActNumber));
                    }
                }

                if (!string.IsNullOrWhiteSpace(actYear))
                {
                    int pActYear = 0;

                    if (int.TryParse(actYear, out pActYear))
                    {
                        cases = cases.Where(e => e.Acts.Any(a => a.DateSigned.Year == pActYear));
                    }
                }
                if (vm.ShowSides && !string.IsNullOrEmpty(vm.SideName))
                {
                    string[] words = vm.SideName.Split(' ');

                    for (int i = 0; i < words.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(words[i]))
                        {
                            string term = words[i];
                            cases = cases.Where(e => e.Sides.Any(s => s.Subject.Name.ToLower().Contains(term.ToLower())
                                                            || s.Subject.Uin.ToLower().Contains(term.ToLower())));
                        }
                    }
                }

                if (vm.ShowLawyers && !string.IsNullOrEmpty(vm.LawyerId))
                {
                    long pLawyerId = 0;

                    if (long.TryParse(vm.LawyerId, out pLawyerId))
                    {
                        var lawyer = _lawyerRepository.Find(pLawyerId);
                        if (lawyer != null)
                        {
                            cases = cases.Where(e => e.Sides.Any(s => s.LawyerAssignments.Any(la => la.LawyerId == lawyer.LawyerId && la.IsActive == true)));
                        }
                    }
                }

                if (vm.HasPersonalCases && vm.AreOnlyPersonalCases)
                {
                    var user = _userRepository.SetWithoutIncludes()
                        .Where(e => e.UserId == CurrentUser.UserID)
                        .Include(e => e.LawyerRegistration)
                        .Single();

                    if (CurrentUser.IsSuperAdmin || CurrentUser.IsSystemAdmin)
                    {
                        // all cases
                    }
                    else if (CurrentUser.IsCourtAdmin && !String.IsNullOrWhiteSpace(CurrentUser.CourtId))
                    {
                        long pCourtId;

                        if (long.TryParse(CurrentUser.CourtId, out pCourtId))
                        {
                            cases = cases.Where(e => e.CourtId == pCourtId);
                        }
                    }
                    else if (CurrentUser.IsPerson && CurrentUser.IsLawyer)
                    {
                        cases = cases.Where(e => e.Sides.Any(s => s.PersonAssignments.Any(pa => pa.PersonRegistrationId == CurrentUser.UserID && (pa.IsActive ?? true)) || s.LawyerAssignments.Any(la => la.LawyerId == user.LawyerRegistration.LawyerId && la.IsActive == true)));
                    }
                    else if (CurrentUser.IsPerson)
                    {
                        cases = cases.Where(e => e.Sides.Any(s => s.PersonAssignments.Any(pa => pa.PersonRegistrationId == CurrentUser.UserID && (pa.IsActive ?? true))));
                    }

                    else if (CurrentUser.IsLawyer)
                    {
                        cases = cases.Where(e => e.Sides.Any(s => s.LawyerAssignments.Any(la => la.LawyerId == user.LawyerRegistration.LawyerId && la.IsActive == true)));
                    }
                }

                int innerPage = string.IsNullOrEmpty(page) ? 1 : int.Parse(page);

                var enumerableCases = cases
                    .Include(e => e.Court)
                    .Include(e => e.CaseKind)
                    .Take(Statics.MaxCaseItems + 1).ToList();

                if (enumerableCases.Count > Statics.MaxCaseItems)
                {
                    ModelState.AddModelError("_FORM", "Броят на намерените дела надвишава " + Statics.MaxCaseItems + ". Моля, въведете допълнителни критерии.");
                    vm.SearchResults = null;
                }
                else
                {
                    #region Order

                    if (order == CasesOrder.Court)
                        enumerableCases = isAsc ? enumerableCases.OrderBy(e => e.Court.Name).ToList()
                                        : enumerableCases.OrderByDescending(e => e.Court.Name).ToList();
                    else if (order == CasesOrder.Case)
                        enumerableCases = isAsc ? enumerableCases.OrderBy(e => e.Number).ThenBy(e => e.CaseYear).ToList()
                                        : enumerableCases.OrderByDescending(e => e.Number).ThenByDescending(e => e.CaseYear).ToList();
                    else if (order == CasesOrder.Kind)
                        enumerableCases = isAsc ? enumerableCases.OrderBy(e => e.CaseKind.Name).ToList()
                                        : enumerableCases.OrderByDescending(e => e.CaseKind.Name).ToList();
                    else if (order == CasesOrder.Department)
                        enumerableCases = isAsc ? enumerableCases.OrderBy(e => e.DepartmentName).ToList()
                                        : enumerableCases.OrderByDescending(e => e.DepartmentName).ToList();

                    #endregion

                    vm.SearchResults = enumerableCases.ToPagedList(innerPage, Statics.MaxCaseItemsPerPage);
                }
            }

            #endregion

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("Captcha")]
        public virtual ActionResult Search(CaseSearchVM vm, bool? captchaValid)
        {
            if (!Request.IsAuthenticated && captchaValid.HasValue && !captchaValid.Value)
            {
                ModelState.AddModelError("Captcha", CaseSearchVM.MessageCaptcha);
            }

            if (!ModelState.IsValid)
            {
                FillSelectListItems(ref vm);

                return View(vm);
            }

            vm.ShowResults = true;

            CaseSearchVM.EncryptProperties(vm);

            return RedirectToAction(ActionNames.Search, vm);
        }

        #endregion

        #region Details

        [HttpGet]
        public virtual ActionResult Details(Guid gid, int? sumPage, int? actSumPage, int? appSumPage, int? hearSumPage, int? hPage, int? aPage, int? asPage, int? ccPage, int? idPage, int? odPage, int? sdPage)
        {
            CaseDetailsVM vm = new CaseDetailsVM();

            vm.sumPage = sumPage;
            vm.actSumPage = actSumPage;
            vm.appSumPage = appSumPage;
            vm.hearSumPage = hearSumPage;
            vm.hPage = hPage;
            vm.aPage = aPage;
            vm.asPage = asPage;
            vm.ccPage = ccPage;
            vm.idPage = idPage;
            vm.odPage = odPage;
            vm.sdPage = sdPage;

            vm.Case = _caseRepository.SetWithoutIncludes().Where(e => e.Gid == gid)
                .Include(e => e.Court)
                .Include(e => e.CaseKind)
                .Include(e => e.IncomingDocument)
                .Include(e => e.CaseCode)
                .Include(e => e.Sides)
                .Include(e => e.Sides.Select(s => s.SideInvolvementKind))
                .Include(e => e.Sides.Select(s => s.Subject))
                .Include(e => e.Sides.Select(s => s.LawyerAssignments))
                .Include(e => e.Sides.Select(s => s.LawyerAssignments.Select(l => l.Lawyer)))
                .Single();

            long caseId = vm.Case.CaseId;

            vm.Hearings = _hearingRepository.SetWithoutIncludes().Where(e => e.CaseId == caseId).ToList().OrderByDescending(e => e.Date).ToPagedList(vm.hPage ?? 1, Statics.SmallPageSize);
            vm.Acts = _actRepository.SetWithoutIncludes()
                .Include(e => e.ActKind)
                .Include(e => e.ActPreparators)
                .Where(e => e.CaseId == caseId).ToList().OrderByDescending(e => e.DateSigned).ToPagedList(vm.aPage ?? 1, Statics.SmallPageSize);

            List<Appeal> appealsList = new List<Appeal>();
            vm.Acts.ToList().ForEach(a => appealsList.AddRange(
                _appealRepository.SetWithoutIncludes()
               .Include(e => e.AppealKind)
               .Include(e => e.Side)
               .Where(e => e.ActId == a.ActId)
               )
            );
            appealsList.RemoveAll(a => a == null);

            vm.Appeals = appealsList.AsQueryable().OrderByDescending(e => e.DateFiled).ToPagedList(vm.aPage ?? 1, Statics.SmallPageSize);

            vm.HasPermissions = Request.IsAuthenticated && _caseRepository.CheckPermission(gid, CurrentUser.UserID);

            if (vm.HasPermissions)
            {
                vm.Assignments = _assignmentRepository.SetWithoutIncludes().Where(e => e.CaseId == caseId).ToList().OrderByDescending(e => e.Date).ToPagedList(vm.asPage ?? 1, Statics.SmallPageSize);

                vm.CaseRulings = _caseRulingRepository.GetCaseRulings(caseId)
                    .Include(e => e.CaseRulingKind)
                    .ToList().ToPagedList(vm.sdPage ?? 1, Statics.SmallPageSize);

                var initDocId = _caseRepository.Find(caseId).IncomingDocumentId;

                List<IncomingDocument> _IncomingDocuments = _incomingDocumentRepository.GetIncomingDocuments(caseId)
                    .Include(e => e.Subject)
                    .Include(e => e.IncomingDocumentType)
                    .ToList();

                if (initDocId != null)
                {
                    IncomingDocument initDoc = _incomingDocumentRepository.GetInitIncomingDocument((long)initDocId);
                    _IncomingDocuments.Add(initDoc);
                    vm.IncomingDocuments = _IncomingDocuments.Distinct().OrderByDescending(e => e.IncomingDate).ToPagedList(vm.idPage ?? 1, Statics.SmallPageSize);
                }
                else
                {
                    vm.IncomingDocuments = _IncomingDocuments.OrderByDescending(e => e.IncomingDate).ToPagedList(vm.idPage ?? 1, Statics.SmallPageSize);
                }



                vm.OutgoingDocuments = _outgoingDocumentRepository.GetOutgoingDocuments(caseId)
                    .Include(e => e.Subject)
                    .Include(e => e.OutgoingDocumentType)
                    .ToList().ToPagedList(vm.odPage ?? 1, Statics.SmallPageSize);

                vm.ScannedFiles = _scannedFileRepository.GetScannedFiles(caseId).ToList().ToPagedList(vm.sdPage ?? 1, Statics.MaxNomItems);
            }

            if (ShowSummons)
            {
                vm.Summons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Case, caseId).ToList().ToPagedList(vm.sumPage ?? 1, Statics.SmallPageSize);
                vm.ActSummons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Act, _actRepository.SetWithoutIncludes().Where(e => e.CaseId == caseId).Select(e => e.ActId).ToArray()).ToList().ToPagedList(vm.actSumPage ?? 1, Statics.SmallPageSize);
                vm.AppealSummons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Appeal, _actRepository.SetWithoutIncludes().Where(e => e.CaseId == caseId).SelectMany(act => act.Appeals).Select(e => e.AppealId).ToArray()).ToList().ToPagedList(vm.appSumPage ?? 1, Statics.SmallPageSize);
                vm.HearingSummons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Hearing, _hearingRepository.SetWithoutIncludes().Where(e => e.CaseId == caseId).Select(e => e.HearingId).ToArray()).ToList().ToPagedList(vm.hearSumPage ?? 1, Statics.SmallPageSize);
            }

            var connectedCases = _caseRepository.GetConnectedCases(caseId);

            vm.ConnectedCases = connectedCases.OrderByDescending(e => e.FormationDate).ThenBy(e => e.Number).ToPagedList(vm.ccPage ?? 1, Statics.SmallPageSize);

            #region AttachedDocuments
            vm.AttachedDocument = new List<AttachedDocument>();
            if (vm.IncomingDocuments != null && vm.IncomingDocuments.Any())
            {
                vm.AttachedDocument.AddRange(_attachedDocumentRepository.GetAttachedDocuments(AttachedTypes.IncommingDocument, vm.IncomingDocuments.Select(x => x.IncomingDocumentId).ToArray()));
            }
            if (vm.OutgoingDocuments != null && vm.OutgoingDocuments.Any())
            {
                vm.AttachedDocument.AddRange(_attachedDocumentRepository.GetAttachedDocuments(AttachedTypes.OutgoingDocument, vm.OutgoingDocuments.Select(x => x.OutgoingDocumentId).ToArray()));
            }
            #endregion
            return View(vm);
        }

        #endregion

        #region Files

        [HttpGet]
        public virtual RedirectResult GetAssignmentFile(Guid assignmentGid)
        {
            Assignment assignment = _assignmentRepository.FindByGid(assignmentGid);

            if (assignment == null || !assignment.BlobKey.HasValue)
                return null;

            // Check permissions
            if (!Request.IsAuthenticated || !_caseRepository.CheckPermission(assignment.CaseId, CurrentUser.UserID))
                return null;

            Guid blobKey = assignment.BlobKey.Value;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetScannedFile(Guid scannedFileGid)
        {
            ScannedFile scannedFile = _scannedFileRepository.FindByGid(scannedFileGid);

            if (scannedFile == null)
                return null;

            // Check permissions
            if (!Request.IsAuthenticated || !_caseRepository.CheckPermission(scannedFile.CaseId, CurrentUser.UserID))
                return null;

            Guid blobKey = scannedFile.BlobKey;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetIncomingDocumentFile(Guid incomingDocumentGid)
        {
            IncomingDocument document = _incomingDocumentRepository.FindByGid(incomingDocumentGid);

            if (document == null || !document.BlobKey.HasValue)
                return null;


            long caseId = document.CaseId.HasValue ? document.CaseId.Value : _caseRepository.GetCaseByInitIncomingDocument(document.IncomingDocumentId).CaseId;
            // Check permissions
            if (!Request.IsAuthenticated || !_caseRepository.CheckPermission(caseId, CurrentUser.UserID))
                return null;

            Guid blobKey = document.BlobKey.Value;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetAttachedDocumentFile(Guid gid)
        {
            AttachedDocument document = _attachedDocumentRepository.FindByGid(gid);

            if (document == null)
                return null;


            //long caseId = document.CaseId.HasValue ? document.CaseId.Value : _caseRepository.GetCaseByInitIncomingDocument(document.IncomingDocumentId).CaseId;
            // Check permissions
            if (!Request.IsAuthenticated
                //|| !_caseRepository.CheckPermission(caseId, CurrentUser.UserID)
                )
                return null;

            Guid blobKey = document.BlobKey;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetOutgoingDocumentFile(Guid outgoingDocumentGid)
        {
            OutgoingDocument document = _outgoingDocumentRepository.FindByGid(outgoingDocumentGid);

            if (document == null || !document.BlobKey.HasValue)
                return null;

            // Check permissions
            if (!Request.IsAuthenticated || !_caseRepository.CheckPermission(document.CaseId.Value, CurrentUser.UserID))
                return null;

            Guid blobKey = document.BlobKey.Value;

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        #endregion

        #region Lawyers

        [HttpPost]
        public virtual JsonResult GetLawyer(long id)
        {
            var lawyer = _lawyerRepository.Find(id);

            if (lawyer != null)
                return Json(new { id = lawyer.LawyerId, text = lawyer.Number + " " + lawyer.Name });
            else
                return Json(new { id = "", text = "" });
        }

        [HttpPost]
        public virtual JsonResult GetLawyers(string term)
        {
            var lawyers = _lawyerRepository.FindLawyers(term, Statics.MaxNomItems);

            return Json(lawyers.Select(e => new { id = e.LawyerId, text = String.Format("{0} {1}", e.Number, e.Name) }));
        }

        #endregion

        #region Private

        private void FillSelectListItems(ref CaseSearchVM vm)
        {
            if (vm == null)
                vm = new CaseSearchVM();

            vm.ShowLawyers = Request.IsAuthenticated && (CurrentUser.IsCourtAdmin || CurrentUser.IsSystemAdmin || CurrentUser.IsSuperAdmin);
            vm.ShowSides = Request.IsAuthenticated;

            vm.HasPersonalCases = Request.IsAuthenticated && (CurrentUser.IsPerson || CurrentUser.IsLawyer);

            if (Request.IsAuthenticated && CurrentUser.IsPerson)
            {
                vm.Courts =
                (from pa in ((UnitOfWork)_unitOfWork).DbContext.Set<PersonAssignment>().Where(e => e.PersonRegistrationId == CurrentUser.UserID && (e.IsActive ?? true))
                 join s in ((UnitOfWork)_unitOfWork).DbContext.Set<Side>() on pa.SideId equals s.SideId
                 join c in ((UnitOfWork)_unitOfWork).DbContext.Set<Case>() on s.CaseId equals c.CaseId
                 join court in ((UnitOfWork)_unitOfWork).DbContext.Set<Court>() on c.CourtId equals court.CourtId
                 select court).Distinct().OrderBy(e => e.Name).ToList().Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
            }
            else if (Request.IsAuthenticated && CurrentUser.IsLawyer)
            {
                vm.Courts =
                (from lr in ((UnitOfWork)_unitOfWork).DbContext.Set<LawyerRegistration>().Where(e => e.LawyerRegistrationId == CurrentUser.UserID)
                 join la in ((UnitOfWork)_unitOfWork).DbContext.Set<LawyerAssignment>() on lr.LawyerId equals la.LawyerId
                 join s in ((UnitOfWork)_unitOfWork).DbContext.Set<Side>() on la.SideId equals s.SideId
                 join c in ((UnitOfWork)_unitOfWork).DbContext.Set<Case>() on s.CaseId equals c.CaseId
                 join court in ((UnitOfWork)_unitOfWork).DbContext.Set<Court>() on c.CourtId equals court.CourtId
                 where la.IsActive == true
                 select court).Distinct().OrderBy(e => e.Name).ToList().Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
            }
            else
            if (Request.IsAuthenticated && CurrentUser.IsCourtAdmin)
            {
                vm.Courts = ((UnitOfWork)_unitOfWork).DbContext.Set<Court>().Where(e => e.CourtId.ToString() == CurrentUser.CourtId).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
            }
            else
            {
                vm.Courts = ((UnitOfWork)_unitOfWork).DbContext.Set<Court>().Where(e => e.IsIntegrated).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.Code.ToString(), Text = e.Name });
            }

            vm.CaseKinds = _caseKindRepository.GetNoms(String.Empty).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.NomValueId.ToString(), Text = e.Name });

            vm.CaseYears = _years;
            vm.PredecessorYears = _years;
            vm.ActYears = _years;

            vm.ActKinds = _actKindRepository.GetNoms(String.Empty).OrderBy(e => e.Name).Select(e => new SelectListItem() { Value = e.NomValueId.ToString(), Text = e.Name });
        }

        private IEnumerable<SelectListItem> _years = Enumerable.Range(DateTime.Now.Year - 30, 31).OrderByDescending(e => e).Select(e => new SelectListItem() { Value = e.ToString(), Text = e.ToString() });

        private IUnitOfWork _unitOfWork;
        private ICaseRepository _caseRepository;
        private IHearingRepository _hearingRepository;
        private IActRepository _actRepository;
        private IAppealRepository _appealRepository;
        private ILawyerRepository _lawyerRepository;
        private IAssignmentRepository _assignmentRepository;
        private ICaseRulingRepository _caseRulingRepository;
        private IIncomingDocumentRepository _incomingDocumentRepository;
        private IOutgoingDocumentRepository _outgoingDocumentRepository;
        private IScannedFileRepository _scannedFileRepository;
        private IEntityCodeNomsRepository<CaseKind, EntityCodeNomVO> _caseKindRepository;
        private IEntityCodeNomsRepository<ActKind, EntityCodeNomVO> _actKindRepository;
        private IUserRepository _userRepository;
        private IAttachedDocumentRepository _attachedDocumentRepository;

        #endregion
    }
}