using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using IOWebApplication.Infrastructure.Models.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class CaseService : BaseService, ICaseService
    {
        private readonly IUserContext userContext;
        public CaseService(
            IRepository _repo,
            ILogger<CaseService> _logger,
            IUserContext userContext)
        {
            this.repo = _repo;
            this.logger = _logger;
            this.userContext = userContext;
        }

        private bool isGlobalAccess(long courtId, bool restrictedAccess, int userTypeId, long userCourtId)
        {
            bool isGlobalAccess = false;
            switch (userTypeId)
            {
                case NomenclatureConstants.UserTypes.GlobalAdmin:
                    isGlobalAccess = true;
                    break;
                case NomenclatureConstants.UserTypes.Lawyer:
                    isGlobalAccess = !restrictedAccess;
                    break;
                case NomenclatureConstants.UserTypes.CourtAdmin:
                    isGlobalAccess = courtId == userCourtId;
                    break;
            }
            return isGlobalAccess;
        }

        public void ProcessCaseNames(CaseVM model, DateTime UpgradeEpepDateStart, bool publicAccess, int userTypeId, long userCourtId)
        {
            if (NomenclatureConstants.UserTypes.GlobalAdmin == userTypeId)
            {
                return;
            }
            if (UpgradeEpepDateStart <= model.FormationDate)
            {
                if (!publicAccess || isGlobalAccess(model.CourtId, model.RestrictedAccess ?? true, userTypeId, userCourtId))
                {
                    return;
                }
            }

            if (model.SideLeftSubjectTypeId == NomenclatureConstants.SubjectTypes.Person)
            {
                model.SideLeft = model.SideLeft.ToInitials();
            }
            if (model.SideRightSubjectTypeId == NomenclatureConstants.SubjectTypes.Person)
            {
                model.SideRight = model.SideRight.ToInitials();
            }
        }

        public async Task<List<SideVM>> ProcessSideNames(List<SideVM> sides, DateTime UpgradeEpepDateStart, Guid caseGid, int userTypeId, long userCourtId)
        {

            if (sides == null)
            {
                return sides;
            }

            var caseInfo = await repo.AllReadonly<Case>().Where(x => x.Gid == caseGid)
                                        .Select(x => new
                                        {
                                            x.CaseId,
                                            x.FormationDate,
                                            x.CourtId,
                                            RestrictedAccess = x.RestrictedAccess ?? true
                                        }).FirstOrDefaultAsync();

            var hasAccess = await CheckCaseAccess(caseInfo.CaseId);

            if (UpgradeEpepDateStart <= caseInfo.FormationDate)
            {
                if (hasAccess || isGlobalAccess(caseInfo.CourtId, caseInfo.RestrictedAccess, userTypeId, userCourtId))
                {
                    return sides;
                }
            }

            foreach (var item in sides)
            {
                if (item.SubjectTypeId == NomenclatureConstants.SubjectTypes.Person)
                {
                    item.SubjectName = item.SubjectName.ToInitials();
                }
            }
            return sides;
        }

        public async Task<IQueryable<CaseVM>> SelectCase(FilterCaseVM filter)
        {
            filter.Sanitize();
            Expression<Func<Case, bool>> whereMyCases = x => true;
            Expression<Func<Case, bool>> whereFocusCases = x => true;
            Expression<Func<Case, bool>> whereLastViewedCases = x => true;
            if (filter.MyCasesOnly)
            {
                switch (userContext.UserType)
                {
                    case NomenclatureConstants.UserTypes.Person:
                        {
                            var caseIds = await repo.AllReadonly<UserAssignment>()
                                                .Where(x => x.UserRegistrationId == userContext.UserId)
                                                .Where(x => x.AssignmentRoleId == NomenclatureConstants.UserAssignmentRoles.Side)
                                                .Where(x => x.IsActive == true)
                                                .Select(x => x.CaseId)
                                                .ToArrayAsync();
                            whereMyCases = x => caseIds.Contains(x.CaseId);
                        }
                        break;
                    case NomenclatureConstants.UserTypes.Lawyer:
                        {
                            var caseIds = await repo.AllReadonly<UserAssignment>()
                                                .Where(x => x.UserRegistrationId == userContext.UserId)
                                                .Where(x => x.AssignmentRoleId == NomenclatureConstants.UserAssignmentRoles.Lawyer)
                                                .Where(x => x.IsActive == true)
                                                .Select(x => x.CaseId)
                                                .ToArrayAsync();
                            whereMyCases = x => caseIds.Contains(x.CaseId);

                            filter.CheckMeInSides = false;
                        }
                        break;
                    case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                        {
                            var caseIds = await repo.AllReadonly<UserAssignment>()
                                                .Where(x => x.UserRegistrationId == userContext.OrganizationUserId)
                                                .Where(x => x.IsActive == true)
                                                .Select(x => x.CaseId)
                                                .ToArrayAsync();
                            whereMyCases = x => caseIds.Contains(x.CaseId);

                            filter.CheckMeInSides = false;
                        }
                        break;
                    case NomenclatureConstants.UserTypes.OrganizationUser:
                        {
                            var caseIds = await repo.AllReadonly<UserOrganizationCase>()
                                                .Where(x => x.UserRegistrationId == userContext.UserId)
                                                .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                                                .Where(x => x.IsActive)
                                                .Select(x => x.CaseId)
                                                .ToArrayAsync();
                            whereMyCases = x => caseIds.Contains(x.CaseId);

                            filter.CheckMeInSides = false;
                        }
                        break;
                    default:
                        break;
                }
                whereFocusCases = x => !x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Archive);
                if (filter.FocusCasesOnly)
                {
                    whereFocusCases = x => x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Focus);
                }
                if (filter.ArchiveCasesOnly)
                {
                    whereFocusCases = x => x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Archive);
                }

            }

            if (!filter.MyCasesOnly && userContext.UserType != NomenclatureConstants.UserTypes.GlobalAdmin)
            {
                filter.SideUic = null;
            }

            if (filter.LastCasesOnly)
            {
                whereLastViewedCases = x => x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.View);
            }

            Expression<Func<Case, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.CourtId == filter.CourtId;
            }
            Expression<Func<Case, bool>> whereDocNumber = x => true;
            if (filter.DocNumber > 0)
            {
                whereDocNumber = x => x.IncomingDocument.IncomingNumber == filter.DocNumber;
            }
            Expression<Func<Case, bool>> whereCaseKind = x => true;
            if (filter.CaseKindId > 0)
            {
                whereCaseKind = x => x.CaseKindId == filter.CaseKindId;
            }
            Expression<Func<Case, bool>> whereRegNumber = x => true;
            if (filter.RegNumber > 0)
            {
                whereRegNumber = x => x.Number == filter.RegNumber;
            }
            Expression<Func<Case, bool>> whereRegYear = x => true;
            if (filter.RegYear > 0)
            {
                whereRegYear = x => x.CaseYear == filter.RegYear;
            }
            Expression<Func<Case, bool>> wherePrevNumber = x => true;
            if (filter.PrevNumber > 0)
            {
                wherePrevNumber = x => x.ConnectedCases.Any(c => c.PredecessorCase.Number == filter.PrevNumber);
            }
            Expression<Func<Case, bool>> wherePrevYear = x => true;
            if (filter.PrevYear > 0)
            {
                wherePrevYear = x => x.ConnectedCases.Any(c => c.PredecessorCase.CaseYear == filter.PrevYear);
            }
            Expression<Func<Case, bool>> whereGid = x => true;
            if (filter.CaseGid.HasValue)
            {
                whereGid = x => x.Gid == filter.CaseGid;
            }

            Expression<Func<Case, bool>> whereOrgUsers = x => true;
            if (filter.NoOrgUserCases && userContext.UserType == NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                whereOrgUsers = x => !x.OrganizationCases.Any(o => o.UserRegistration.UserTypeId == NomenclatureConstants.UserTypes.OrganizationUser && o.IsActive && o.UserRegistration.IsActive);
            }


            Expression<Func<Case, bool>> whereAct = x => true;
            if (filter.ActKindId > 0 || filter.ActNumber > 0 || filter.ActYear > 0)
            {
                DateTime actFromDate = DateTime.MinValue;
                DateTime actToDate = DateTime.MaxValue;
                string typequery = "";
                if (filter.ActYear > 0)
                {
                    typequery = "d";
                    actFromDate = new DateTime(filter.ActYear.Value, 1, 1);
                    actToDate = new DateTime(filter.ActYear.Value + 1, 1, 1).AddSeconds(-10);
                }
                if (filter.ActKindId > 0)
                {
                    typequery += "k";
                }
                if (filter.ActNumber > 0)
                {
                    typequery += "n";
                }
                switch (typequery)
                {
                    case "d":
                        whereAct = x => x.Acts.Any(a =>
                                a.DateSigned >= actFromDate && a.DateSigned <= actToDate
                            );
                        break;
                    case "dk":
                        whereAct = x => x.Acts.Any(a =>
                                a.DateSigned >= actFromDate && a.DateSigned <= actToDate
                                && a.ActKindId == filter.ActKindId.Value
                            );
                        break;
                    case "dkn":
                        whereAct = x => x.Acts.Any(a =>
                                a.DateSigned >= actFromDate && a.DateSigned <= actToDate
                                && a.Number == filter.ActNumber.Value
                                && a.ActKindId == filter.ActKindId.Value
                            );
                        break;
                    case "dn":
                        whereAct = x => x.Acts.Any(a =>
                                a.DateSigned >= actFromDate && a.DateSigned <= actToDate
                                && a.Number == filter.ActNumber.Value
                            );
                        break;
                    case "k":
                        whereAct = x => x.Acts.Any(a =>
                                 a.ActKindId == filter.ActKindId.Value
                            );
                        break;
                    case "kn":
                        whereAct = x => x.Acts.Any(a =>
                               a.Number == filter.ActNumber.Value
                               && a.ActKindId == filter.ActKindId.Value
                           );
                        break;
                    case "n":
                        whereAct = x => x.Acts.Any(a =>
                               a.Number == filter.ActNumber.Value
                           );
                        break;
                }


            }

            Expression<Func<Case, bool>> whereSideUic = x => true;
            if (!string.IsNullOrEmpty(filter.SideUic))
            {
                whereSideUic = x => x.Sides.Any(s => s.Subject.Uin == filter.SideUic);
            }

            if (filter.CheckMeInSides)
            {
                whereSideUic = x => x.Sides.Any(s => s.Subject.Uin == userContext.Identifier);
            }

            //Expression<Func<Case, int>> orderFocus = x => 0;

            //if (filter.MyCasesOnly)
            //{
            //    orderFocus = x => x.CaseFocuses.Where(x => x.UserRegistrationId == userContext.UserId && x.Focus == NomenclatureConstants.FocusTypes.Archive)
            //    .Select(x => x.Focus).FirstOrDefault();
            //}

            var checkFocus = filter.MyCasesOnly || filter.CaseGid.HasValue;


            Expression<Func<Case, CaseVM>> select =
                x => new CaseVM
                {
                    Gid = x.Gid,
                    CourtId = x.CourtId,
                    CourtName = x.Court.Name,
                    HasElectronicDocuments = (x.Court.ForElectronicDocument ?? false),
                    HasElectronicPayments = (x.Court.ForElectronicPayment ?? false),
                    CaseKindName = x.CaseKind.Name,
                    IncommingNumber = (x.IncomingDocumentId > 0) ? x.IncomingDocument.IncomingNumber : 0,
                    RegNumber = x.Number,
                    RegYear = x.CaseYear,
                    FormationDate = x.FormationDate,
                    DepartmentName = x.DepartmentName,
                    PanelName = x.PanelName,
                    RestrictedAccess = x.RestrictedAccess,
                    SystemCode = x.SystemCode,
                    SideLeftSubjectTypeId = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Select(s => s.Subject.SubjectTypeId).FirstOrDefault(),
                    SideLeft = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Select(s => s.Subject.Name).FirstOrDefault(),
                    //SideLeftCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Count() - 1,
                    SideRight = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Select(s => s.Subject.Name).FirstOrDefault(),
                    SideRightSubjectTypeId = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Select(s => s.Subject.SubjectTypeId).FirstOrDefault(),
                    //SideRightCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Count() - 1,
                    JudgeReporter = x.Reporters.Select(x => x.JudgeName).FirstOrDefault()
                };

            if (checkFocus)
            {
                select =
                    x => new CaseVM
                    {
                        Gid = x.Gid,
                        CourtId = x.CourtId,
                        CourtName = x.Court.Name,
                        HasElectronicDocuments = (x.Court.ForElectronicDocument ?? false),
                        HasElectronicPayments = (x.Court.ForElectronicPayment ?? false),
                        CaseKindName = x.CaseKind.Name,
                        IncommingNumber = (x.IncomingDocumentId > 0) ? x.IncomingDocument.IncomingNumber : 0,
                        RegNumber = x.Number,
                        RegYear = x.CaseYear,
                        FormationDate = x.FormationDate,
                        DepartmentName = x.DepartmentName,
                        PanelName = x.PanelName,
                        RestrictedAccess = x.RestrictedAccess,
                        SystemCode = x.SystemCode,
                        SideLeftSubjectTypeId = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Select(s => s.Subject.SubjectTypeId).FirstOrDefault(),
                        SideLeft = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Select(s => s.Subject.Name).FirstOrDefault(),
                        SideLeftCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Count() - 1,
                        SideRight = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Select(s => s.Subject.Name).FirstOrDefault(),
                        SideRightSubjectTypeId = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Select(s => s.Subject.SubjectTypeId).FirstOrDefault(),
                        SideRightCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Count() - 1,
                        JudgeReporter = x.Reporters.Select(x => x.JudgeName).FirstOrDefault(),
                        FocusCase = x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Focus),
                        ArchiveCase = x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Archive)
                    };

            }

            var result = repo.AllReadonly<Case>()
                        .AsSplitQuery()
                        .Where(whereMyCases)
                        .Where(whereFocusCases)
                        .Where(whereLastViewedCases)
                        .Where(whereCourt)
                        .Where(whereDocNumber)
                        .Where(whereCaseKind)
                        .Where(whereRegNumber)
                        .Where(whereRegYear)
                        .Where(wherePrevNumber)
                        .Where(wherePrevYear)
                        .Where(whereGid)
                        .Where(whereAct)
                        .Where(whereSideUic)
                        .Where(whereOrgUsers)
                        //.OrderBy(orderFocus)
                        .OrderByDescending(x => x.FormationDate)
                        .ThenByDescending(x => x.CaseId)
                        .Select(select).AsQueryable();

            //var sql = result.ToQueryString();
            return result;
        }

        public async Task<IQueryable<HearingOnlineVM>> SelectHearingOnline(FilterHearingVM filter)
        {
            filter.Sanitize();
            Expression<Func<Hearing, bool>> whereMyCases = x => true;
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                    {
                        var caseIds = await repo.AllReadonly<UserAssignment>()
                                            .Where(x => x.UserRegistrationId == userContext.UserId)
                                            .Where(x => x.AssignmentRoleId == null)
                                            .Where(x => x.IsActive == true)
                                            .Select(x => x.Side.CaseId)
                                            .ToArrayAsync();
                        whereMyCases = x => caseIds.Contains(x.CaseId);
                    }
                    break;
                case NomenclatureConstants.UserTypes.Lawyer:
                    {
                        var caseIds = await repo.AllReadonly<UserAssignment>()
                                            .Where(x => x.UserRegistrationId == userContext.UserId)
                                            .Where(x => x.AssignmentRoleId == NomenclatureConstants.UserAssignmentRoles.Lawyer)
                                            .Where(x => x.IsActive == true)
                                            .Select(x => x.Side.CaseId)
                                            .ToArrayAsync();
                        whereMyCases = x => caseIds.Contains(x.CaseId);
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    {
                        var caseIds = await repo.AllReadonly<UserAssignment>()
                                            .Where(x => x.UserRegistrationId == userContext.OrganizationUserId)
                                            .Where(x => x.IsActive == true)
                                            .Select(x => x.Side.CaseId)
                                            .ToArrayAsync();
                        whereMyCases = x => caseIds.Contains(x.CaseId);
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    {
                        var caseIds = await repo.AllReadonly<UserOrganizationCase>()
                                            .Where(x => x.UserRegistrationId == userContext.UserId)
                                            .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                                            .Where(x => x.IsActive)
                                            .Select(x => x.CaseId)
                                            .ToArrayAsync();
                        whereMyCases = x => caseIds.Contains(x.CaseId);
                    }
                    break;
                default:
                    break;
            }


            Expression<Func<Hearing, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.Case.CourtId == filter.CourtId;
            }
            Expression<Func<Hearing, bool>> whereCaseKind = x => true;
            if (filter.CaseKindId > 0)
            {
                whereCaseKind = x => x.Case.CaseKindId == filter.CaseKindId;
            }
            Expression<Func<Hearing, bool>> whereRegNumber = x => true;
            if (filter.RegNumber > 0)
            {
                whereRegNumber = x => x.Case.Number == filter.RegNumber;
            }
            Expression<Func<Hearing, bool>> whereRegYear = x => true;
            if (filter.RegYear > 0)
            {
                whereRegYear = x => x.Case.CaseYear == filter.RegYear;
            }
            Expression<Func<Hearing, bool>> whereDateFrom = x => true;
            if (filter.DateFrom.HasValue)
            {
                whereDateFrom = x => x.Date >= filter.DateFrom.Value;
            }
            Expression<Func<Hearing, bool>> whereDateTo = x => true;
            if (filter.DateTo.HasValue)
            {
                whereDateTo = x => x.Date <= filter.DateTo.MakeEndDate();
            }

            return repo.AllReadonly<Hearing>()
                        .AsSplitQuery()
                        .Where(x => x.VideoUrl != null)
                        .Where(whereMyCases)
                        .Where(whereCourt)
                        .Where(whereCaseKind)
                        .Where(whereRegNumber)
                        .Where(whereRegYear)
                        .Where(whereDateFrom)
                        .Where(whereDateTo)
                        .Where(x => x.VideoUrl != null)
                        .OrderBy(x => x.Date)
                        .Select(x => new HearingOnlineVM
                        {
                            Gid = x.Gid,
                            CaseGid = x.Case.Gid,
                            CourtName = x.Case.Court.Name,
                            CaseKindName = x.Case.CaseKind.Name,
                            CaseNumber = x.Case.Number,
                            CaseYear = x.Case.CaseYear,
                            Date = x.Date,
                            VideoUrl = x.VideoUrl,
                            HearingType = x.HearingType,
                            IsCanceled = x.IsCanceled
                        }).AsQueryable();
        }

        public IQueryable<CaseVM> SelectLastViewedCases()
        {
            return repo.AllReadonly<UserCaseFocus>()
                        //.AsSplitQuery()
                        .Where(x => x.UserRegistrationId == userContext.UserId)
                        .Where(x => x.Focus == NomenclatureConstants.FocusTypes.View)
                        .OrderByDescending(x => x.UserCaseFocusId)
                        .Select(x => x.Case)
                        .Select(x => new CaseVM
                        {
                            Gid = x.Gid,
                            CourtId = x.CourtId,
                            CourtName = x.Court.Name,
                            CaseKindName = x.CaseKind.Name,
                            //IncommingNumber = (x.IncomingDocumentId > 0) ? x.IncomingDocument.IncomingNumber : 0,
                            RegNumber = x.Number,
                            RegYear = x.CaseYear,
                            //FormationDate = x.FormationDate,
                            //DepartmentName = x.DepartmentName,
                            //SideLeft = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Select(s => s.Subject.Name).FirstOrDefault(),
                            //SideLeftCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.LeftInit).Count() - 1,
                            //SideRight = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Select(s => s.Subject.Name).FirstOrDefault(),
                            //SideRightCount = x.Sides.Where(s => s.SideInvolvementKind.SideType == NomenclatureConstants.SideTypes.Right).Count() - 1,
                            //JudgeReporter = x.Reporters.Select(x => x.JudgeName).FirstOrDefault(),
                            //FocusCase = checkFocus && x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == NomenclatureConstants.FocusTypes.Focus)
                        }).AsQueryable();
        }

        public async Task<SaveResultVM> ToggleFocusCase(Guid gid, int focus)
        {
            var result = new SaveResultVM(false);
            if (!userContext.IsAuthenticated)
            {
                return result;
            }

            var focusInfo = await repo.AllReadonly<Case>()
                                        .Where(x => x.Gid == gid)
                                        .Select(x => new
                                        {
                                            x.CaseId,
                                            FocusCase = x.CaseFocuses.Any(f => f.UserRegistrationId == userContext.UserId && f.Focus == focus)
                                        }).FirstOrDefaultAsync();

            if (focusInfo == null)
            {
                return result;
            }

            if (focusInfo.FocusCase)
            {
                var focusRecords = await repo.All<UserCaseFocus>()
                                    .Where(x => x.CaseId == focusInfo.CaseId
                                    && x.UserRegistrationId == userContext.UserId
                                    && x.Focus == focus)
                                    .ToListAsync();

                if (focusRecords.Any())
                {
                    repo.DeleteRange(focusRecords);
                    await repo.SaveChangesAsync();
                }
            }
            else
            {
                await repo.AddAsync(new UserCaseFocus()
                {
                    UserRegistrationId = userContext.UserId,
                    CaseId = focusInfo.CaseId,
                    Focus = focus,
                    DateWrt = DateTime.Now
                });

                await repo.SaveChangesAsync();
            }

            result.Result = true;
            return result;
        }

        public async Task SaveCaseView(Guid gid, int viewMode = NomenclatureConstants.FocusTypes.View)
        {
            var caseModel = await GetByGidAsync<Case>(gid);
            if (caseModel == null)
            {
                return;
            }

            var lastViewCase = await repo.All<UserCaseFocus>()
                                    .Where(x => x.UserRegistrationId == userContext.UserId)
                                    .Where(x => x.Focus == viewMode)
                                    .OrderByDescending(x => x.UserCaseFocusId)
                                    .FirstOrDefaultAsync();

            if (lastViewCase != null)
                if (lastViewCase.CaseId == caseModel.CaseId)
                {
                    lastViewCase.DateWrt = DateTime.Now;
                    await repo.SaveChangesAsync();
                    return;
                }

            var viewCase = new UserCaseFocus()
            {
                UserRegistrationId = userContext.UserId,
                CaseId = caseModel.CaseId,
                DateWrt = DateTime.Now,
                Focus = viewMode
            };

            await repo.AddAsync(viewCase);
            await repo.SaveChangesAsync();
        }

        public async Task<IQueryable<DocumentVM>> SelectDocumentsByCaseGid(GidLoaderVM loader)
        {
            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }

            var attachedDocs = repo.AllReadonly<AttachedDocument>();


            var incDocuments = repo.AllReadonly<IncomingDocument>()
                            .Where(x => x.CaseId == caseModel.CaseId || x.IncomingDocumentId == caseModel.IncomingDocumentId)
                            .Select(x => new DocumentVM
                            {
                                Gid = x.Gid,
                                Direction = 1,
                                Number = x.IncomingNumber,
                                Date = x.IncomingDate,
                                Type = NomenclatureConstants.SourceTypes.IncommingDocument,
                                TypeName = x.IncomingDocumentType.Name,
                                SubjectName = x.Subject.Name,
                                HasFiles = x.BlobKey != null || attachedDocs.Any(a => a.AttachmentType == NomenclatureConstants.AttachedTypes.IncommingDocument
                                                        && a.ParentId == x.IncomingDocumentId)
                            }).AsQueryable();

            var outDocuments = repo.AllReadonly<OutgoingDocument>()
                           .Where(x => x.CaseId == caseModel.CaseId)
                           .Select(x => new DocumentVM
                           {
                               Gid = x.Gid,
                               Direction = 2,
                               Number = x.OutgoingNumber,
                               Date = x.OutgoingDate,
                               Type = NomenclatureConstants.SourceTypes.OutgoingDocument,
                               TypeName = x.OutgoingDocumentType.Name,
                               SubjectName = x.Subject.Name,
                               HasFiles = x.BlobKey != null || attachedDocs.Any(a => a.AttachmentType == NomenclatureConstants.AttachedTypes.OutgoingDocument
                                                       && a.ParentId == x.OutgoingDocumentId)
                           }).AsQueryable();

            return incDocuments.Union(outDocuments).OrderByDescending(x => x.Date);
        }
        public async Task<IQueryable<HearingVM>> SelectHearingsByCaseGid(GidLoaderVM loader)
        {
            Expression<Func<Hearing, bool>> whereHearing = x => false;

            long caseId;
            if (loader.ObjectGid.HasValue)
            {
                whereHearing = x => x.Gid == loader.ObjectGid;
                caseId = await repo.GetPropByIdAsync<Hearing, long>(x => x.Gid == loader.ObjectGid, x => x.CaseId);
            }
            else
            {
                var caseModel = await GetByGidAsync<Case>(loader.Gid);
                if (caseModel == null)
                {
                    return null;
                }
                caseId = caseModel.CaseId;
                whereHearing = x => x.CaseId == caseModel.CaseId;
            }

            //Expression<Func<Hearing, HearingVM>> select = ;
            //if (loader.Public || !(await CheckCaseAccess(caseId)))
            //{
            //    select = x => new HearingVM
            //    {
            //        Gid = x.Gid,
            //        Date = x.Date,
            //        HearingType = x.HearingType,
            //        //HearingResult = x.HearingResult,
            //        IsCanceled = x.IsCanceled,
            //        //CourtRoom = x.CourtRoom,
            //        ProsecutorName = x.ProsecutorName,
            //        SecretaryName = x.SecretaryName
            //    };
            //}

            return repo.AllReadonly<Hearing>()
                        .Where(whereHearing)
                        .OrderByDescending(x => x.Date)
                        .Select(x => new HearingVM
                        {
                            Gid = x.Gid,
                            Date = x.Date,
                            HearingType = x.HearingType,
                            HearingResult = x.HearingResult,
                            IsCanceled = x.IsCanceled,
                            CourtRoom = x.CourtRoom,
                            VideoUrl = x.VideoUrl,
                            ProsecutorName = x.ProsecutorName,
                            SecretaryName = x.SecretaryName
                        }).AsQueryable();
        }

        public async Task<List<FileItemVM>> SelectHearingProtocolByHearingGid(Guid gid)
        {
            var result = new List<FileItemVM>();
            var info = await repo.AllReadonly<Hearing>()
                                .AsSplitQuery()
                                .Where(x => x.Gid == gid)
                                .Select(x => new
                                {
                                    x.CaseId,
                                    x.Case.CourtId,
                                    PrivateFileGid = x.PrivateBlobKey,
                                    PrivateFileName = (x.PrivateBlobKey != null) ? x.PrivateBlob.FileName : (string)null,
                                    PublicFileGid = x.PublicBlobKey,
                                    PublicFileName = (x.PublicBlobKey != null) ? x.PublicBlob.FileName : (string)null,
                                }).FirstOrDefaultAsync();

            if (info == null)
            {
                return result;
            }

            var hasCaseAccess = await CheckCaseAccess(info.CaseId);
            if (!hasCaseAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasCaseAccess = await hasGlobalAccess(0, info.CourtId);
            }

            if (info.PublicFileGid.HasValue)
            {
                result.Add(new FileItemVM()
                {
                    FileGid = info.PublicFileGid.Value,
                    FileName = info.PublicFileName,
                    Title = "Протокол от заседание"
                });
            }

            if (hasCaseAccess)
            {
                if (info.PrivateFileGid.HasValue)
                {
                    result.Add(new FileItemVM()
                    {
                        FileGid = info.PrivateFileGid.Value,
                        FileName = info.PrivateFileName,
                        Title = "Протокол от заседание - обезличен"
                    });
                }
            }

            return result;
        }

        public IQueryable<HearingVM> SelectNextHearingsByUser()
        {
            Expression<Func<Hearing, bool>> whereHearing = x => false;

            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                case NomenclatureConstants.UserTypes.Lawyer:
                    {
                        whereHearing = x => x.Case.UserAssignments.Any(a =>
                                        a.UserRegistrationId == userContext.UserId && a.IsActive);
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    {
                        whereHearing = x => x.Case.UserAssignments.Any(a =>
                                        a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive);

                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    {
                        whereHearing = x => x.Case.OrganizationCases.Any(a =>
                                      a.UserRegistrationId == userContext.UserId && a.IsActive);

                        //long[] caseIds = await repo.AllReadonly<UserOrganizationCase>()
                        //                        .Where(x => x.UserRegistrationId == userContext.UserId && x.IsActive)
                        //                        .Select(x => x.CaseId)
                        //                        .ToArrayAsync();

                        //whereSummon = x => caseIds.Contains(x.Side.CaseId);

                    }
                    break;
            }

            Expression<Func<Hearing, HearingVM>> select = x => new HearingVM
            {
                Gid = x.Gid,
                Date = x.Date,
                HearingType = x.HearingType,
                HearingResult = x.HearingResult,
                IsCanceled = x.IsCanceled,
                CourtName = x.Case.Court.Name,
                CourtRoom = x.CourtRoom,
                CaseInfo = $"{x.Case.CaseKind.Label} {x.Case.Number}/{x.Case.CaseYear}",
                CaseGid = x.Case.Gid,
                ProsecutorName = x.ProsecutorName,
                SecretaryName = x.SecretaryName
            };

            return repo.AllReadonly<Hearing>()
                        .Where(x => x.Date > DateTime.Now)
                        .Where(whereHearing)
                        .OrderBy(x => x.Date)
                        .Select(select).AsQueryable();
        }

        public async Task<IQueryable<AssignmentVM>> SelectAssignmentsByCaseGid(GidLoaderVM loader)
        {
            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }

            Expression<Func<Assignment, AssignmentVM>> select = x => new AssignmentVM
            {
                Gid = x.Gid,
                Date = x.Date,
                JudgeName = x.JudgeName,
                Type = x.Type,
                Assignor = x.Assignor,
                BlobKey = x.BlobKey
            };

            if (loader.Public || !(await CheckCaseAccess(caseModel.CaseId)))
            {
                select = x => new AssignmentVM
                {
                    Gid = x.Gid,
                    Date = x.Date,
                    JudgeName = x.JudgeName,
                    Type = x.Type,
                    Assignor = x.Assignor
                };
            }

            return repo.AllReadonly<Assignment>()
                            .Where(x => x.CaseId == caseModel.CaseId)
                            .OrderByDescending(x => x.Date)
                            .Select(select).AsQueryable();
        }

        public async Task<IQueryable<ActVM>> SelectActsByCaseGid(GidLoaderVM loader)
        {
            Expression<Func<Act, bool>> whereAct = x => false;
            long caseId;
            if (loader.ObjectGid.HasValue)
            {
                //Зареждане на акт по Gid
                whereAct = x => x.Gid == loader.ObjectGid;
                caseId = await repo.GetPropByIdAsync<Case, long>(x => x.Gid == loader.Gid, x => x.CaseId);
            }
            else
            {
                if (loader.ParentGid.HasValue)
                {
                    //Зареждане по заседание

                    var hearingModel = await GetByGidAsync<Hearing>(loader.ParentGid.Value);
                    if (hearingModel == null)
                    {
                        return null;
                    }
                    caseId = hearingModel.CaseId;
                    whereAct = x => x.HearingId == hearingModel.CaseId;
                }
                else
                {
                    //Зареждане по дело
                    var caseModel = await GetByGidAsync<Case>(loader.Gid);
                    if (caseModel == null)
                    {
                        return null;
                    }
                    caseId = caseModel.CaseId;
                    whereAct = x => x.CaseId == caseModel.CaseId;
                }
            }

            Expression<Func<Act, ActVM>> select = x => new ActVM
            {
                Gid = x.Gid,
                DateSigned = x.DateSigned,
                DateInPower = x.DateInPower,
                Number = x.Number,
                ActKindName = x.ActKind.Name,
                Preparators = x.ActPreparators
                                    .OrderBy(p => p.Role)
                                    .Select(p => new ActPreparatorVM
                                    {
                                        JudgeName = p.JudgeName,
                                        JudgeRole = p.Role
                                    }).ToArray()
            };
            //if (loader.Public || !(await CheckCaseAccess(caseId)))
            //{
            //    select = x => new ActVM
            //    {
            //        Gid = x.Gid,
            //        DateSigned = x.DateSigned,
            //        Number = x.Number,
            //        ActKindName = x.ActKind.Name

            //    };
            //}

            return repo.AllReadonly<Act>()
                            .AsSplitQuery()
                            .Where(whereAct)
                            .OrderByDescending(x => x.DateSigned)
                            .Select(select).AsQueryable();
        }

        public async Task<IQueryable<SideVM>> SelectSidesByCaseGid(GidLoaderVM loader)
        {
            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }

            return repo.AllReadonly<Side>()
                            .AsSplitQuery()
                            .Where(x => x.CaseId == caseModel.CaseId)
                            .OrderBy(x => x.SideInvolvementKind.SideType)
                            .Select(x => new SideVM
                            {
                                Gid = x.Gid,
                                SubjectName = x.Subject.Name,
                                SubjectTypeId = x.Subject.SubjectTypeId,
                                SideInvolvementKindName = x.SideInvolvementKind.Name,
                                Lawyers = x.UserAssignments.Where(a => a.AssignmentRoleId == NomenclatureConstants.UserAssignmentRoles.Lawyer && a.IsActive).Select(a => a.UserRegistration.FullName).ToArray(),
                                ProceduralRelation = x.ProceduralRelation
                            }).AsQueryable();
        }

        public async Task<bool> CheckCaseAccess(Guid caseGid)
        {
            return await CheckCaseAccess(await repo.GetPropByIdAsync<Case, long>(x => x.Gid == caseGid, x => x.CaseId));
        }

        async Task<bool> hasGlobalAccess(long? caseId, long? courtId)
        {
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.GlobalAdmin:
                    return true;
                case NomenclatureConstants.UserTypes.CourtAdmin:
                    if (courtId > 0)
                    {
                        return courtId == userContext.CourtId;
                    }
                    return (await repo.GetPropByIdAsync<Case, long>(x => x.CaseId == caseId, x => x.CourtId)) == userContext.CourtId;
                default:
                    return false;
            }
        }

        public async Task<bool> CheckCaseAccess(long caseId)
        {
            //TODO!!!!!
            //return true;
            if (!userContext.IsAuthenticated)
            {
                return false;
            }

            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    if (await repo.AllReadonly<UserAssignment>()
                                .Where(x => x.Side.CaseId == caseId && x.UserRegistrationId == userContext.OrganizationUserId)
                                .Where(x => x.IsActive)
                                .AnyAsync())
                    {
                        return true;
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    if (await repo.AllReadonly<UserOrganizationCase>()
                                .Where(x => x.CaseId == caseId && x.UserRegistrationId == userContext.UserId)
                                .Where(x => x.IsActive)
                                .AnyAsync())
                    {
                        return true;
                    }
                    break;
                default:
                    if (await repo.AllReadonly<UserAssignment>()
                                .Where(x => x.Side.CaseId == caseId && x.UserRegistrationId == userContext.UserId)
                                .Where(x => x.IsActive)
                                .AnyAsync())
                    {
                        return true;
                    }
                    break;
            }


            return false;
        }

        public async Task<IQueryable<SummonVM>> SelectSummonsByCaseGid(GidLoaderVM loader)
        {
            Expression<Func<Summon, bool>> whereSummon = x => false;
            long caseId = 0;
            if (loader.ObjectGid.HasValue)
            {
                //Зареждане по заседание
                var hearingModel = await GetByGidAsync<Hearing>(loader.ObjectGid.Value);
                if (hearingModel == null)
                {
                    return null;
                }
                caseId = hearingModel.CaseId;
                whereSummon = x => x.HearingId == hearingModel.HearingId;
            }
            else
            {
                var caseModel = await GetByGidAsync<Case>(loader.Gid);
                if (caseModel == null)
                {
                    return null;
                }
                caseId = caseModel.CaseId;
                whereSummon = x => x.Side.CaseId == caseModel.CaseId;

            }

            //if (!(await CheckCaseAccess(caseId)))
            //{
            //    return null;
            //}

            Expression<Func<Summon, SummonVM>> select = x => new SummonVM
            {
                Gid = x.Gid,
                SummonType = x.SummonType.Name,
                SummonKind = x.SummonKind,
                Subject = x.Addressee,
                DateCreated = x.DateCreated,
                DateServed = x.DateServed,
                IsRead = x.IsRead,
                ReadTime = x.ReadTime,
                CourtName = x.Side.Case.Court.Name,
                CaseInfo = $"{x.Side.Case.CaseKind.Label} {x.Side.Case.Number}/{x.Side.Case.CaseYear}",
                CaseGid = $"{x.Side.Case.Gid}"
            };

            return repo.AllReadonly<Summon>()
                        .Where(whereSummon)
                        .OrderByDescending(x => x.DateCreated)
                        .Select(select).AsQueryable();
        }




        public IQueryable<SummonVM> SelectLastSummonsByUser()
        {
            Expression<Func<Summon, bool>> whereSummon = x => false;

            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                case NomenclatureConstants.UserTypes.Lawyer:
                    {
                        whereSummon = x => x.Side.UserAssignments.Any(a =>
                                        a.UserRegistrationId == userContext.UserId && a.IsActive);
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    {
                        whereSummon = x => x.Side.UserAssignments.Any(a =>
                                        a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive);

                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    {
                        whereSummon = x => x.Side.Case.OrganizationCases.Any(a =>
                                      a.UserRegistrationId == userContext.UserId && a.IsActive);

                        //long[] caseIds = await repo.AllReadonly<UserOrganizationCase>()
                        //                        .Where(x => x.UserRegistrationId == userContext.UserId && x.IsActive)
                        //                        .Select(x => x.CaseId)
                        //                        .ToArrayAsync();

                        //whereSummon = x => caseIds.Contains(x.Side.CaseId);

                    }
                    break;
            }

            Expression<Func<Summon, SummonVM>> select = x => new SummonVM
            {
                Gid = x.Gid,
                SummonType = x.SummonType.Name,
                SummonKind = x.SummonKind,
                DateCreated = x.DateCreated,
                DateServed = x.DateServed,
                IsRead = x.IsRead,
                ReadTime = x.ReadTime,
                CourtName = x.Side.Case.Court.Name,
                CaseInfo = $"{x.Side.Case.CaseKind.Label} {x.Side.Case.Number}/{x.Side.Case.CaseYear}",
                CaseGid = $"{x.Side.Case.Gid}"
            };

            return repo.AllReadonly<Summon>()
                        .Where(whereSummon)
                        .OrderBy(x => x.IsRead)
                        .ThenByDescending(x => x.DateCreated)
                        .Select(select).AsQueryable();
        }



        public async Task<IQueryable<CaseElementVM>> SelectChronologyByCase(GidLoaderVM loader)
        {
            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }
            var hasAccess = await CheckCaseAccess(caseModel.CaseId);
            if (!hasAccess)
            {
                hasAccess = await hasGlobalAccess(0, caseModel.CourtId);
            }

            var result = new List<CaseElementVM>();

            if (hasAccess)
            {
                var initDocumentItem = await repo.AllReadonly<IncomingDocument>()
                                            .Where(x => x.IncomingDocumentId == caseModel.IncomingDocumentId)
                                            .Select(x => new CaseElementVM
                                            {
                                                OrderBy = -2,
                                                Type = NomenclatureConstants.SourceTypes.IncommingDocument,
                                                Gid = x.Gid,
                                                ItemType = x.IncomingDocumentType.Name,
                                                Date = x.IncomingDate,
                                                Number = x.IncomingNumber.ToString(),
                                                Detail = x.Subject.Name,
                                                DetailTitle = "Кореспондент",
                                                TypeName = "документ"
                                            }).FirstOrDefaultAsync();

                if (initDocumentItem != null)
                {
                    result.Add(initDocumentItem);
                }
            }

            var caseItem = await repo.AllReadonly<Case>()
                                        .Where(x => x.CaseId == caseModel.CaseId)
                                        .Select(x => new CaseElementVM
                                        {
                                            OrderBy = -1,
                                            Type = NomenclatureConstants.SourceTypes.Case,
                                            Gid = x.Gid,
                                            ItemType = x.CaseKind.Name,
                                            Date = x.FormationDate,
                                            Number = x.Number.ToString(),
                                            DetailTitle = x.CaseKind.Name,
                                            Detail = $"{x.Number}/{x.CaseYear}",
                                            TypeName = "дело"
                                        }).FirstOrDefaultAsync();
            result.Add(caseItem);

            if (hasAccess)
            {
                var incDocumentItems = await repo.AllReadonly<IncomingDocument>()
                                            .Where(x => x.CaseId == caseModel.CaseId)
                                            .Select(x => new CaseElementVM
                                            {
                                                Type = NomenclatureConstants.SourceTypes.IncommingDocument,
                                                Gid = x.Gid,
                                                ItemType = x.IncomingDocumentType.Name,
                                                Date = x.IncomingDate,
                                                Number = x.IncomingNumber.ToString(),
                                                Detail = x.Subject.Name,
                                                DetailTitle = "Кореспондент",
                                                TypeName = "документ"
                                            }).ToArrayAsync();
                result.AddRange(incDocumentItems);

                var outDocumentItems = await repo.AllReadonly<OutgoingDocument>()
                                           .Where(x => x.CaseId == caseModel.CaseId)
                                           .Where(x => x.OutgoingDate != null)
                                           .Select(x => new CaseElementVM
                                           {
                                               Type = NomenclatureConstants.SourceTypes.OutgoingDocument,
                                               Gid = x.Gid,
                                               ItemType = x.OutgoingDocumentType.Name,
                                               Date = x.OutgoingDate.Value,
                                               Number = x.OutgoingNumber.ToString(),
                                               Detail = x.Subject.Name,
                                               DetailTitle = "Кореспондент",
                                               TypeName = "документ"
                                           }).ToArrayAsync();
                result.AddRange(outDocumentItems);
            }
            //if (hasAccess)
            //{
            //    var summonsItems = await repo.AllReadonly<Summon>()
            //                               .Where(x => x.CaseId == caseModel.CaseId)
            //                               .Select(x => new CaseElementVM
            //                               {
            //                                   Type = NomenclatureConstants.SourceTypes.Summon,
            //                                   Gid = x.Gid,
            //                                   ItemType = x.SummonType.Name,
            //                                   Date = x.DateCreated,
            //                                   Detail = x.SummonKind,
            //                                   TypeName = "призовка"
            //                               }).ToArrayAsync();
            //    result.AddRange(summonsItems);
            //}

            var sideItems = (await repo.AllReadonly<Side>()
                                           .Where(x => x.CaseId == caseModel.CaseId)
                                           .Select(x => new
                                           {
                                               x.Gid,
                                               FullName = x.Subject.Name,
                                               x.Subject.SubjectTypeId,
                                               x.InsertDate,
                                               SideInvolvementKindName = x.SideInvolvementKind.Name
                                           })
                                           .ToArrayAsync())
                                           .Select(x => new CaseElementVM
                                           {
                                               Type = NomenclatureConstants.SourceTypes.Side,
                                               Gid = x.Gid,
                                               ItemType = (!hasAccess && x.SubjectTypeId == 1) ? x.FullName.ToInitials() : x.FullName,
                                               Date = x.InsertDate,
                                               Detail = (!hasAccess && x.SubjectTypeId == 1) ? x.FullName.ToInitials() : x.FullName,
                                               DetailTitle = x.SideInvolvementKindName,
                                               TypeName = "страна"
                                           }).ToArray();
            result.AddRange(sideItems);

            var assingmentItems = await repo.AllReadonly<Assignment>()
                                      .Where(x => x.CaseId == caseModel.CaseId)
                                      .Select(x => new CaseElementVM
                                      {
                                          Type = NomenclatureConstants.SourceTypes.Assignment,
                                          Gid = x.Gid,
                                          ItemType = x.Type,
                                          Date = x.Date,
                                          Detail = x.JudgeName,
                                          DetailTitle = "Избран съдия",
                                          TypeName = "разпределение"
                                      }).ToArrayAsync();
            result.AddRange(assingmentItems);

            var hearings = await repo.AllReadonly<Hearing>()
                                        .Where(x => x.CaseId == caseModel.CaseId)
                                        .Select(x => new CaseElementVM
                                        {
                                            Type = NomenclatureConstants.SourceTypes.Hearing,
                                            Gid = x.Gid,
                                            ItemType = x.HearingType,
                                            Date = x.Date,
                                            TypeName = "заседание",
                                            DetailTitle = "зала / резултат",
                                            Detail = $"{x.CourtRoom} / {x.HearingResult}"
                                        }).ToArrayAsync();

            foreach (var hearing in hearings)
            {
                result.Add(hearing);
                result.AddRange((await SelectHearingItemsByHearing(new GidLoaderVM()
                {
                    Gid = hearing.Gid,
                    InternalCall = true
                })).Where(x => x.Type != NomenclatureConstants.SourceTypes.Summon));
            }

            return result.OrderByDescending(x => x.OrderBy).ThenByDescending(x => x.Date).ThenBy(x => x.Type).AsQueryable();
        }

        public async Task<IQueryable<CaseElementVM>> SelectHearingItemsByHearing(GidLoaderVM loader)
        {
            Expression<Func<Summon, bool>> whereSummon = x => false;
            long caseId = 0;
            //Зареждане по заседание
            var hearingModel = await GetByGidAsync<Hearing>(loader.Gid);
            if (hearingModel == null)
            {
                return null;
            }
            caseId = hearingModel.CaseId;
            whereSummon = x => x.HearingId == hearingModel.HearingId;


            var hasAccess = await CheckCaseAccess(caseId);


            var acts = await repo.AllReadonly<Act>()
                                   .Where(x => x.HearingId == hearingModel.HearingId)
                                   .Select(x => new CaseElementVM
                                   {
                                       Type = NomenclatureConstants.SourceTypes.Act,
                                       TypeName = "Акт",
                                       Gid = x.Gid,
                                       Date = x.DateSigned,
                                       ItemType = x.ActKind.Name,
                                       Number = $"{x.Number}",
                                       DetailTitle = x.ActKind.Name,
                                       Detail = $"{x.Number}"
                                   }).ToListAsync();

            List<CaseElementVM> hearingDocuments = new List<CaseElementVM>();

            try
            {
                hearingDocuments = await repo.AllReadonly<HearingDocument>()
                                   .Where(x => x.HearingId == hearingModel.HearingId)
                                   .Select(x => new CaseElementVM
                                   {
                                       Type = NomenclatureConstants.SourceTypes.HearingDocument,
                                       TypeName = "Документ",
                                       Gid = x.Gid,
                                       Date = x.CreateDate,
                                       ItemType = x.HearingDocumentKind,
                                       DetailTitle = "Вносител",
                                       Detail = $"{x.Side.Subject.Name}",
                                   }).ToListAsync();
            }
            catch (Exception ex) { }

            List<CaseElementVM> summons;

            if (hasAccess)
            {
                summons = await repo.AllReadonly<Summon>()
                                     .Where(x => x.HearingId == hearingModel.HearingId)
                                     .Select(x => new CaseElementVM
                                     {
                                         Type = NomenclatureConstants.SourceTypes.Summon,
                                         TypeName = "Призовка",
                                         Gid = x.Gid,
                                         Date = x.CreateDate,
                                         ItemType = x.SummonKind,
                                         DetailTitle = "Адресат",
                                         Detail = $"{x.Addressee}",
                                     }).ToListAsync();
            }
            else
            {
                summons = new List<CaseElementVM>();
            }

            return acts.Union(hearingDocuments).Union(summons)
                    .OrderByDescending(x => x.Date)
                    .AsQueryable();
        }

        public async Task<IEnumerable<HearingParticipantVM>> SelectHearingParticipantByHearing(GidLoaderVM loader)
        {
            //Зареждане по заседание
            var hearingModel = await GetByGidAsync<Hearing>(loader.Gid);
            if (hearingModel == null)
            {
                return null;
            }


            return await repo.AllReadonly<HearingParticipant>()
                                   .Where(x => x.HearingId == hearingModel.HearingId)
                                   .Select(x => new HearingParticipantVM
                                   {
                                       FullName = x.JudgeName,
                                       SubstitudeFor = x.SubstituteFor,
                                       Role = x.Role
                                   }).ToListAsync();

        }

        public async Task<IEnumerable<HearingParticipantVM>> SelectActPreparatorsByAct(GidLoaderVM loader)
        {
            //Зареждане на акт
            var actModel = await GetByGidAsync<Act>(loader.Gid);
            if (actModel == null)
            {
                return null;
            }

            return await repo.AllReadonly<ActPreparator>()
                                   .Where(x => x.ActId == actModel.ActId)
                                   .Select(x => new HearingParticipantVM
                                   {
                                       FullName = x.JudgeName,
                                       Role = x.Role
                                   }).ToListAsync();

        }

        public async Task<IEnumerable<ConnectedCaseVM>> SelectConnectedCaseByCaseGid(GidLoaderVM loader)
        {
            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }


            //if (!(await CheckCaseAccess(caseModel.CaseId)))
            //{
            //    return null;
            //}

            List<long> caseIds = new List<long>();
            long priorCaseId = await repo.AllReadonly<ConnectedCase>()
                                    .Where(x => x.CaseId == caseModel.CaseId)
                                    .Select(x => x.PredecessorCaseId)
                                    .FirstOrDefaultAsync();

            long nextCaseId = await repo.AllReadonly<ConnectedCase>()
                                    .Where(x => x.PredecessorCaseId == caseModel.CaseId)
                                    .Select(x => x.CaseId)
                                    .FirstOrDefaultAsync();

            long lastCaseId = 0;
            if (nextCaseId > 0)
            {
                lastCaseId = await repo.AllReadonly<ConnectedCase>()
                                    .Where(x => x.PredecessorCaseId == nextCaseId)
                                    .Select(x => x.CaseId)
                                    .FirstOrDefaultAsync();
            }

            long firstCaseId = 0;
            if (priorCaseId > 0)
            {
                firstCaseId = await repo.AllReadonly<ConnectedCase>()
                                    .Where(x => x.CaseId == priorCaseId)
                                    .Select(x => x.PredecessorCaseId)
                                    .FirstOrDefaultAsync();
            }
            if (firstCaseId > 0)
            {
                caseIds.Add(firstCaseId);
            }
            if (priorCaseId > 0)
            {
                caseIds.Add(priorCaseId);
            }
            if (nextCaseId > 0)
            {
                caseIds.Add(nextCaseId);
            }
            if (lastCaseId > 0)
            {
                caseIds.Add(lastCaseId);
            }

            return await repo.AllReadonly<Case>()
                                   .Where(x => caseIds.Contains(x.CaseId))
                                   .OrderBy(x => x.CaseId)
                                   .Select(x => new ConnectedCaseVM
                                   {
                                       Gid = x.Gid,
                                       CaseKideName = x.CaseKind.Name,
                                       CourtName = x.Court.Name,
                                       Number = x.Number,
                                       Year = x.CaseYear
                                   }).ToListAsync();

        }

        public async Task<FilesPreviewVM> LoadFilesForObject(int type, Guid gid)
        {
            switch (type)
            {
                case NomenclatureConstants.SourceTypes.IncommingDocument:
                    return await _loadFilesForIncommingDocument(gid);
                case NomenclatureConstants.SourceTypes.OutgoingDocument:
                    return await _loadFilesForOutgoingDocument(gid);
                case NomenclatureConstants.SourceTypes.Assignment:
                    return await _loadFilesForAssignment(gid);

                case NomenclatureConstants.SourceTypes.Summon:
                    return await _loadFilesForSummon(gid);

                case NomenclatureConstants.SourceTypes.HearingDocument:
                    return await _loadFilesForHearingDocument(gid);

                case NomenclatureConstants.SourceTypes.Act:
                case NomenclatureConstants.SourceTypes.ActPrivate:
                case NomenclatureConstants.SourceTypes.ActPublic:
                case NomenclatureConstants.SourceTypes.MotivePrivate:
                case NomenclatureConstants.SourceTypes.MotivePublic:
                    return await _loadFilesForAct(gid);
                case NomenclatureConstants.SourceTypes.CaseScannedFiles:
                    return await _loadScannedFiles(gid);
                default:
                    return null;
            }
        }

        private async Task<FilesPreviewVM> _loadFilesForAssignment(Guid gid)
        {
            var result = await repo.AllReadonly<Assignment>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new FilesPreviewVM
                                    {
                                        CaseId = x.CaseId,
                                        Type = NomenclatureConstants.SourceTypes.Assignment,
                                        TypeName = "Разпределение",
                                        TypeDetails = x.Type,
                                        Date = x.CreateDate
                                    }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            //if (!(await CheckCaseAccess(result.CaseId)))
            //{
            //    return null;
            //}

            result.Files = await repo.AllReadonly<Assignment>()
                                    .Where(x => x.Gid == gid)
                                    .Where(x => x.BlobKey != null)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = NomenclatureConstants.SourceTypes.Assignment,
                                        Title = x.JudgeName,
                                        FileGid = x.BlobKey.Value,
                                        FileName = x.Blob.FileName
                                    }).ToListAsync();

            return result;
        }

        private async Task<FilesPreviewVM> _loadFilesForSummon(Guid gid)
        {
            var result = await repo.AllReadonly<Summon>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new FilesPreviewVM
                                    {
                                        ObjectGid = gid,
                                        CaseId = x.Side.CaseId,
                                        Type = NomenclatureConstants.SourceTypes.Summon,
                                        TypeName = x.SummonKind,
                                        TypeDetails = x.Addressee,
                                        Number = x.Number,
                                        Date = x.CreateDate,
                                        ForAction = !x.IsRead,
                                        SummonCourtDescription = x.CourtReadDescription,
                                        SummonCourtDate = x.ReadTime
                                    }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            if (!(await CheckCaseAccess(result.CaseId)))
            {
                return null;
            }

            result.Files = new List<FileItemVM>();
            result.Files.AddRange(await repo.AllReadonly<Summon>()
                                    .Where(x => x.Gid == gid)
                                    .Where(x => x.SummonBlobKey != null)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = NomenclatureConstants.SourceTypes.Summon,
                                        Title = x.SummonKind,
                                        FileGid = x.SummonBlobKey.Value,
                                        FileName = x.SummonBlob.FileName
                                    }).ToListAsync());

            result.Files.AddRange(await repo.AllReadonly<Summon>()
                                    .Where(x => x.Gid == gid)
                                    .Where(x => x.ReportBlobKey != null)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = NomenclatureConstants.SourceTypes.SummonReport,
                                        Title = "Отчет за доставено съобщение",
                                        FileGid = x.ReportBlobKey.Value,
                                        FileName = x.ReportBlob.FileName
                                    }).ToListAsync());

            result.Files.AddRange(await repo.AllReadonly<Summon>()
                                   .Where(x => x.Gid == gid)
                                   .Where(x => x.ReportReadTimeBlobKey != null)
                                   .Select(x => new FileItemVM
                                   {
                                       Type = NomenclatureConstants.SourceTypes.SummonReadStamp,
                                       Title = "Удостоверение за време на прочитане",
                                       FileGid = x.ReportReadTimeBlob.Key,
                                       FileName = x.ReportReadTimeBlob.FileName
                                   }).ToListAsync());

            Guid summonActGid = await repo.AllReadonly<Summon>()
                                        .Where(x => x.Gid == gid)
                                        .Where(x => x.ActId > 0)
                                        .Select(x => x.Act.Gid)
                                        .FirstOrDefaultAsync();

            if (summonActGid != Guid.Empty)
            {
                var actFiles = await _loadFilesForAct(summonActGid);
                result.Files.AddRange(actFiles.Files);
            }

            return result;
        }

        private async Task<FilesPreviewVM> _loadFilesForIncommingDocument(Guid gid)
        {
            var result = await repo.AllReadonly<IncomingDocument>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new FilesPreviewVM
                                    {
                                        CourtId = x.CourtId,
                                        CaseId = x.CaseId ?? 0,
                                        ObjectId = x.IncomingDocumentId,
                                        Type = NomenclatureConstants.SourceTypes.IncommingDocument,
                                        TypeName = x.IncomingDocumentType.Name,
                                        Number = x.IncomingNumber.ToString(),
                                        Date = x.IncomingDate
                                    }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            if (result.CaseId == 0)
            {
                result.CaseId = await repo.GetPropByIdAsync<Case, long>(x => x.IncomingDocumentId == result.ObjectId, x => x.CaseId);
            }

            var hasAccess = await CheckCaseAccess(result.CaseId);
            if (!hasAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasAccess = await hasGlobalAccess(0, result.CourtId);
            }

            if (hasAccess)
            {
                var docFiles = await repo.AllReadonly<IncomingDocument>()
                                        .Where(x => x.Gid == gid)
                                        .Where(x => x.BlobKey != null)
                                        .Select(x => new FileItemVM
                                        {
                                            Type = NomenclatureConstants.SourceTypes.IncommingDocument,
                                            Title = "Документ",
                                            FileGid = x.BlobKey.Value,
                                            FileName = x.Blob.FileName,
                                        }).ToListAsync();


                docFiles.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.IncommingDocument, result.ObjectId));
                result.Files = docFiles;
            }
            return result;
        }


        private async Task<FilesPreviewVM> _loadFilesForOutgoingDocument(Guid gid)
        {
            var result = await repo.AllReadonly<OutgoingDocument>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new FilesPreviewVM
                                    {
                                        CourtId = (x.CaseId > 0) ? x.Case.CourtId : 0,
                                        CaseId = x.CaseId ?? 0,
                                        ObjectId = x.OutgoingDocumentId,
                                        Type = NomenclatureConstants.SourceTypes.OutgoingDocument,
                                        TypeName = x.OutgoingDocumentType.Name,
                                        Number = x.OutgoingNumber.ToString(),
                                        Date = x.OutgoingDate
                                    }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            var hasAccess = await CheckCaseAccess(result.CaseId);
            if (!hasAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasAccess = await hasGlobalAccess(0, result.CourtId);
            }

            if (hasAccess)
            {

                var docFiles = await repo.AllReadonly<OutgoingDocument>()
                                    .Where(x => x.Gid == gid)
                                    .Where(x => x.BlobKey != null)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = NomenclatureConstants.SourceTypes.OutgoingDocument,
                                        Title = "Документ",
                                        FileGid = x.BlobKey.Value,
                                        FileName = x.Blob.FileName,
                                    }).ToListAsync();


                docFiles.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.OutgoingDocument, result.ObjectId));
                result.Files = docFiles;
            }
            return result;
        }

        private async Task<FilesPreviewVM> _loadFilesForHearingDocument(Guid gid)
        {
            var result = await repo.AllReadonly<HearingDocument>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new FilesPreviewVM
                                    {
                                        CourtId = x.Hearing.Case.CourtId,
                                        CaseId = x.Hearing.CaseId,
                                        ObjectId = x.HearingDocumentId,
                                        Type = NomenclatureConstants.SourceTypes.HearingDocument,
                                        TypeName = x.HearingDocumentKind,
                                        Date = x.Hearing.Date
                                    }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            var hasAccess = await CheckCaseAccess(result.CaseId);
            if (!hasAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasAccess = await hasGlobalAccess(0, result.CourtId);
            }
            if (hasAccess)
            {
                var docFiles = new List<FileItemVM>();
                docFiles.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.SessionFastDocument, result.ObjectId));
                result.Files = docFiles;
            }
            return result;
        }

        private async Task<IEnumerable<FileItemVM>> _loadFilesForAttachedDocument(int attachmentType, long parentId)
        {
            return await repo.AllReadonly<AttachedDocument>()
                                    .Where(x => x.AttachmentType == attachmentType && x.ParentId == parentId)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = NomenclatureConstants.SourceTypes.Summon,
                                        Title = x.FileTitle,
                                        FileGid = x.BlobKey,
                                        FileName = x.AttachedBlob.FileName
                                    }).ToListAsync();
        }

        private async Task<FilesPreviewVM> _loadScannedFiles(Guid caseGid)
        {
            var caseInfo = await repo.AllReadonly<Case>()
                                        .Where(x => x.Gid == caseGid)
                                        .Select(x => new
                                        {
                                            x.CaseId,
                                            x.CourtId
                                        }).FirstOrDefaultAsync();
            var hasAccess = await CheckCaseAccess(caseInfo.CaseId);
            if (!hasAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasAccess = await hasGlobalAccess(0, caseInfo.CourtId);
            }

            var result = new FilesPreviewVM();

            if (!hasAccess)
            {
                return result;
            }

            result.TypeName = "Сканирани документи";
            result.Files = await repo.AllReadonly<ScannedFile>()
                                        .Where(x => x.CaseId == caseInfo.CaseId)
                                        .OrderBy(x => x.ScannedFileId)
                                        .Select(x => new FileItemVM
                                        {
                                            FileGid = x.BlobKey,
                                            FileName = x.Blob.FileName,
                                            Title = (x.Description ?? x.Blob.FileName)
                                        }).ToListAsync();

            return result;
        }

        public async Task<bool> HasScannedFiles(Guid caseGid)
        {
            return await repo.AllReadonly<ScannedFile>()
                                        .Where(x => x.Case.Gid == caseGid)
                                        .AnyAsync();
        }

        private async Task<FilesPreviewVM> _loadFilesForAct(Guid gid)
        {
            var info = await repo.AllReadonly<Act>()
                                    .AsSplitQuery()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new
                                    {
                                        x.ActId,
                                        x.Case.CourtId,
                                        x.CaseId,
                                        TypeName = x.ActKind.Name,
                                        x.Number,
                                        x.DateSigned,
                                        ActPrivateFileName = (x.PrivateActBlobKey != null) ? x.PrivateActBlob.FileName : "",
                                        ActPrivateFileGid = (x.PrivateActBlobKey != null) ? x.PrivateActBlob.Key : (Guid?)null
                                        ,
                                        ActPublicFileName = (x.PublicActBlobKey != null) ? x.PublicActBlob.FileName : "",
                                        ActPublicFileGid = (x.PublicActBlobKey != null) ? x.PublicActBlob.Key : (Guid?)null
                                        ,
                                        MotivePrivateFileName = (x.PrivateMotiveBlobKey != null) ? x.PrivateMotiveBlob.FileName : "",
                                        MotivePrivateFileGid = (x.PrivateMotiveBlobKey != null) ? x.PrivateMotiveBlob.Key : (Guid?)null
                                        ,
                                        MotivePublicFileName = (x.PublicMotiveBlobKey != null) ? x.PublicMotiveBlob.FileName : "",
                                        MotivePublicFileGid = (x.PublicMotiveBlobKey != null) ? x.PublicMotiveBlob.Key : (Guid?)null
                                    }).FirstOrDefaultAsync();
            if (info == null)
            {
                return null;
            }

            var result = new FilesPreviewVM()
            {
                Type = NomenclatureConstants.SourceTypes.Act,
                CaseId = info.CaseId,
                TypeName = info.TypeName,
                Number = $"{info.Number}",
                Date = info.DateSigned
            };

            var hasCaseAccess = await CheckCaseAccess(result.CaseId);
            if (!hasCaseAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasCaseAccess = await hasGlobalAccess(0, info.CourtId);
            }

            var files = new List<FileItemVM>();
            if (hasCaseAccess && info.ActPrivateFileGid.HasValue)
            {
                files.Add(new FileItemVM()
                {
                    FileName = info.ActPrivateFileName,
                    FileGid = info.ActPrivateFileGid.Value,
                    Type = NomenclatureConstants.SourceTypes.ActPrivate,
                    Title = info.TypeName
                });
            }
            if (info.ActPublicFileGid.HasValue)
            {
                files.Add(new FileItemVM()
                {
                    FileName = info.ActPublicFileName,
                    FileGid = info.ActPublicFileGid.Value,
                    Type = NomenclatureConstants.SourceTypes.ActPublic,
                    Title = $"{info.TypeName} - Обезличен"
                });
            }
            if (hasCaseAccess && info.MotivePrivateFileGid.HasValue)
            {
                files.Add(new FileItemVM()
                {
                    FileName = info.MotivePrivateFileName,
                    FileGid = info.MotivePrivateFileGid.Value,
                    Type = NomenclatureConstants.SourceTypes.MotivePrivate,
                    Title = "Мотиви"
                });
            }
            if (info.MotivePublicFileGid.HasValue)
            {
                files.Add(new FileItemVM()
                {
                    FileName = info.MotivePublicFileName,
                    FileGid = info.MotivePublicFileGid.Value,
                    Type = NomenclatureConstants.SourceTypes.MotivePublic,
                    Title = "Обезличени мотиви"
                });
            }
            if (hasCaseAccess)
            {
                files.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.ActCoordination, info.ActId));
            }
            files.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.ActCoordinationPublic, info.ActId));
            if (hasCaseAccess)
            {
                files.AddRange(await _loadFilesForAttachedDocument(NomenclatureConstants.AttachedTypes.ActFiles, info.ActId));
            }
            result.Files = files;
            if (hasCaseAccess)
            {
                result.ActAppeals = await _loadAppealsByAct(info.ActId);
            }
            return result;
        }

        private async Task<List<ActAppealVM>> _loadAppealsByAct(long actId)
        {
            return await repo.AllReadonly<Appeal>()
                                    .Where(x => x.ActId == actId)
                                    .Select(x => new ActAppealVM
                                    {
                                        AppealKindName = x.AppealKind.Name,
                                        DateFiled = x.DateFiled,
                                        SideInvolmentKindName = x.Side.SideInvolvementKind.Name,
                                        SideName = x.Side.Subject.Name
                                    }).ToListAsync();
        }

        public async Task<IEnumerable<CalendarVM>> SelectCalendarByUser(DateTime from, DateTime to)
        {
            Expression<Func<Hearing, CalendarVM>> select = x => new CalendarVM
            {
                start = x.Date,
                allDay = false,
                title = $"{x.Case.CaseKind.Label} {x.Case.Number}/{x.Case.CaseYear}"
            };

            List<CalendarVM> result = await queryHearingsByDates(from, to)
                        .OrderBy(x => x.Date)
                        .Select(select)
                        .ToListAsync();

            if (userContext.UserType != NomenclatureConstants.UserTypes.Lawyer)
            {
                return result;
            }

            var vacations = await repo.AllReadonly<UserVacation>()
                                        .Where(x => x.UserId == userContext.UserId)
                                        .Where(x => x.DateFrom <= to && x.DateTo >= from)
                                        .Select(x => new
                                        {
                                            x.DateFrom,
                                            x.DateTo,
                                            Title = x.VacationType.Name
                                        })
                                        .ToListAsync();
            foreach (var item in vacations)
            {
                result.Add(new CalendarVM()
                {
                    allDay = true,
                    classNames = (new List<string>() { "epep-calendar__allday" }).ToArray(),
                    start = item.DateFrom,
                    end = item.DateTo,
                    title = " "
                });
            }

            return result;
        }

        private IQueryable<Hearing> queryHearingsByDates(DateTime from, DateTime to)
        {
            Expression<Func<Hearing, bool>> wherePerson = x => false;
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    wherePerson = x => x.Case.UserAssignments.Any(a => a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive);
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    wherePerson = x => x.Case.UserAssignments.Any(a => a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive);
                    break;
                default:
                    wherePerson = x => x.Case.UserAssignments.Any(a => a.UserRegistrationId == userContext.UserId && a.IsActive);
                    break;
            }


            return repo.AllReadonly<Hearing>()
                        .Where(wherePerson)
                        .Where(x => x.Date >= from.Date && x.Date <= to.MakeEndDate());
        }

        public async Task<CalendarDateVM> GetCalendarDateEvents(DateTime date)
        {
            var result = new CalendarDateVM()
            {
                Date = date,
                Events = await queryHearingsByDates(date, date)
                            .Select(x => new CalendarDateEventVM
                            {
                                Gid = x.Gid,
                                Type = NomenclatureConstants.SourceTypes.Hearing,
                                Title = x.HearingType,
                                Date = x.Date,
                                CaseGid = x.Case.Gid,
                                CourtName = x.Case.Court.Name,
                                CaseInfo = $"{x.Case.CaseKind.Label} {x.Case.Number}/{x.Case.CaseYear}"
                            }).ToArrayAsync()
            };

            return result;
        }
        public async Task<IQueryable<UserWithAccessVM>> SelectUsersWithAccessByCaseGid(GidLoaderVM loader)
        {
            long caseId = 0;

            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }
            caseId = caseModel.CaseId;

            var hasCaseAccess = await CheckCaseAccess(caseId);
            if (!hasCaseAccess && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(userContext.UserType))
            {
                hasCaseAccess = await hasGlobalAccess(0, caseModel.CourtId);
            }

            if (!hasCaseAccess)
            {
                return null;
            }

            return repo.AllReadonly<UserAssignment>()
                        .Where(x => x.Side.CaseId == caseId)
                        .Where(x => x.IsActive == true)
                        .OrderByDescending(x => x.CreateDate)
                        .Select(x => new UserWithAccessVM
                        {
                            UserName = x.UserRegistration.FullName,
                            //UserType = x.UserRegistration.UserTypeId,
                            //Uic = x.UserRegistration.EGN,
                            SideName = x.Side.Subject.Name,
                            //SubjectTypeId = x.Side.Subject.SubjectTypeId ?? NomenclatureConstants.SubjectTypes.Person,
                            SideKindName = x.Side.SideInvolvementKind.Name
                        }).AsQueryable();
        }
    }
}
