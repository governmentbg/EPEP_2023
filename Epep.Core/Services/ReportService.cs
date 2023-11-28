using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class ReportService : BaseService, IReportService
    {
        public ReportService(
            IRepository _repo,
            ILogger<ReportService> _logger)
        {
            this.repo = _repo;
            this.logger = _logger;
        }

        public async Task<IQueryable<ReportUserAssignmentVM>> ReportUserAssignments(Guid userGid)
        {
            var userModel = await GetByGidAsync<UserRegistration>(userGid);

            Expression<Func<UserAssignment, bool>> whereUser = x => x.UserRegistrationId == userModel.Id;
            if (NomenclatureConstants.UserTypes.OrganizationUserTypes.Contains(userModel.UserTypeId))
            {
                whereUser = x => x.UserRegistrationId == userModel.OrganizationUserId;
            }

            return repo.AllReadonly<UserAssignment>()
                            .Where(whereUser)
                            .Where(x => x.IsActive)
                            .OrderByDescending(x => (x.CreateDate))
                            .Select(x => new ReportUserAssignmentVM
                            {
                                CourtName = x.Case.Court.Name,
                                CaseKind = x.Case.CaseKind.Label,
                                CaseKindName = x.Case.CaseKind.Name,
                                CaseNumber = x.Case.Number,
                                CaseYear = x.Case.CaseYear,
                                AssignmentRole = x.AssignmentRole.Name,
                                CreateDate = x.CreateDate,
                                SideName = x.Side.Subject.Name,
                                SideRoleName = x.Side.SideInvolvementKind.Name
                            });
        }


        public async Task<byte[]> ReportUserAssignmentsExcel(Guid userGid)
        {
            NPoiExcelService excelService = new NPoiExcelService("Sheet1");
            var dataRows = await (await ReportUserAssignments(userGid)).ToListAsync();

            var userModel = await GetByGidAsync<UserRegistration>(userGid);

            var styleTitle = excelService.CreateTitleStyle();
            excelService.AddRange($"Справка за разрешени достъпи до дела на потребител", 8,
                      styleTitle); excelService.AddRow();
            excelService.AddRange($"{userModel.FullName}, {userModel.Email}", 8,
                      styleTitle); excelService.AddRow();

            excelService.AddList(
                dataRows,
                new int[] { 10000, 10000, 5000, 5000, 10000, 5000, 5000, 8000 },
                new List<Expression<Func<ReportUserAssignmentVM, object>>>()
                {
                    x => x.CourtName,
                    x => x.CaseKindName,
                    x => x.CaseNumber,
                    x => x.CaseYear,
                    x => x.SideName,
                    x => x.SideRoleName,
                    x => x.AssignmentRole,
                    x => x.CreateDate
                },
                //NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index,
                //NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index
            );
            excelService.AddRow();
            excelService.AddRow();
            excelService.AddRangeMoveCol(dataRows.Count + " бр. записи отговарящи на зададените критерии.", 2, 1);

            return excelService.ToArray();
        }

        public async Task<IQueryable<ReportLawyerViewVM>> ReportLawyerView(FilterLawyerViewVM filter)
        {
            filter.Sanitize();


            Expression<Func<UserCaseFocus, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.Case.CourtId == filter.CourtId;
            }
            Expression<Func<UserCaseFocus, bool>> whereCaseKind = x => true;
            if (filter.CaseKindId > 0)
            {
                whereCaseKind = x => x.Case.CaseKindId == filter.CaseKindId;
            }
            Expression<Func<UserCaseFocus, bool>> whereRegNumber = x => true;
            if (filter.RegNumber > 0)
            {
                whereRegNumber = x => x.Case.Number == filter.RegNumber;
            }
            Expression<Func<UserCaseFocus, bool>> whereRegYear = x => true;
            if (filter.RegYear > 0)
            {
                whereRegYear = x => x.Case.CaseYear == filter.RegYear;
            }

            Expression<Func<UserCaseFocus, bool>> whereDateFrom = x => true;
            if (filter.DateFrom.HasValue)
            {
                whereDateFrom = x => x.DateWrt >= filter.DateFrom.Value;
            }
            Expression<Func<UserCaseFocus, bool>> whereDateTo = x => true;
            if (filter.DateTo.HasValue)
            {
                whereDateTo = x => x.DateWrt <= filter.DateTo.MakeEndDate();
            }

            var lawyerModel = await GetByGidAsync<Lawyer>(filter.LawyerGid);
            var userIds = await repo.AllReadonly<UserRegistration>()
                                    .Where(x => x.EGN == lawyerModel.Uic && x.IsActive && x.UserTypeId == NomenclatureConstants.UserTypes.Person)
                                    .Select(x => x.Id)
                                    .ToArrayAsync();

            return repo.AllReadonly<UserCaseFocus>()
                            .Where(x => userIds.Contains(x.UserRegistrationId) && x.Focus == NomenclatureConstants.FocusTypes.LawyerView)
                            .Where(whereCourt)
                            .Where(whereCaseKind)
                            .Where(whereRegNumber)
                            .Where(whereRegYear)
                            .Where(whereDateFrom)
                            .Where(whereDateTo)
                            .OrderByDescending(x => x.DateWrt)
                            .Select(x => new ReportLawyerViewVM
                            {
                                CourtName = x.Case.Court.Name,
                                CaseKind = x.Case.CaseKind.Label,
                                CaseKindName = x.Case.CaseKind.Name,
                                CaseNumber = x.Case.Number,
                                CaseYear = x.Case.CaseYear,
                                CaseGid = x.Case.Gid,
                                ViewDate = x.DateWrt
                            });
        }

        public async Task<byte[]> ReportLawyerViewExcel(FilterLawyerViewVM filter)
        {
            NPoiExcelService excelService = new NPoiExcelService("Sheet1");
            var dataRows = await (await ReportLawyerView(filter)).ToListAsync();

            var lawyerModel = await GetByGidAsync<Lawyer>(filter.LawyerGid);

            var filterText = "";
            if (filter.CourtId > 0)
                filterText += $"Съд: {await repo.GetPropByIdAsync<Court, string>(x => x.CourtId == filter.CourtId, x => x.Name)}; ";

            if (filter.CaseKindId > 0)
                filterText += $"Вид дело: {await repo.GetPropByIdAsync<CaseKind, string>(x => x.CaseKindId == filter.CaseKindId, x => x.Name)}; ";

            if (filter.RegNumber > 0)
                filterText += $"Дело номер: {filter.RegNumber}; ";
            if (filter.RegYear > 0)
                filterText += $"Дело година: {filter.RegYear}; ";
            if (filter.DateFrom != null)
                filterText += $"От дата: {filter.DateFrom.Value:dd.MM.yyyy}; ";
            if (filter.DateTo != null)
                filterText += $"До дата: {filter.DateTo.Value:dd.MM.yyyy};";


            var styleTitle = excelService.CreateTitleStyle();
            excelService.AddRange($"Справка за достъпи до дела по Закона за адвокатурата", 5,
                      styleTitle); excelService.AddRow();
            excelService.AddRange($"адвокат {lawyerModel.Name}, {lawyerModel.Number}", 5,
                      styleTitle); excelService.AddRow();
            excelService.AddRange(filterText, 5,
                      styleTitle); excelService.AddRow();

            excelService.AddList(
                dataRows,
                new int[] { 10000, 10000, 5000, 5000, 8000 },
                new List<Expression<Func<ReportLawyerViewVM, object>>>()
                {
                    x => x.CourtName,
                    x => x.CaseKindName,
                    x => x.CaseNumber,
                    x => x.CaseYear,
                    x => x.ViewDate
                },
                //NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index,
                //NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index
            );
            excelService.AddRow();
            excelService.AddRow();
            excelService.AddRangeMoveCol(dataRows.Count + " бр. записи отговарящи на зададените критерии.", 2, 1);

            return excelService.ToArray();
        }

        public async Task<IQueryable<ReportCaseStatsVM>> ReportCaseStat(FilterCaseVM filter)
        {
            filter.Sanitize();

            Expression<Func<Case, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.CourtId == filter.CourtId;
            }

            Expression<Func<Case, bool>> whereRegYear = x => true;
            if (filter.RegYear > 0)
            {
                whereRegYear = x => x.CaseYear == filter.RegYear;
            }

            var result = await repo.AllReadonly<Case>()
                            .Where(whereCourt)
                            .Where(whereRegYear)
                            .GroupBy(x => new { CourtName = x.Court.Name, x.CaseTypeId })
                            .Select(x => new
                            {
                                x.Key.CourtName,
                                x.Key.CaseTypeId,
                                Cnt = x.Count()
                            })
                            .Select(x => new
                            {
                                x.CourtName,
                                G = (x.CaseTypeId == 1) ? x.Cnt : 0,
                                A = (x.CaseTypeId == 2) ? x.Cnt : 0,
                                N = (x.CaseTypeId == 3) ? x.Cnt : 0,
                                F = (x.CaseTypeId == 4) ? x.Cnt : 0,
                                T = (x.CaseTypeId == 5) ? x.Cnt : 0,
                            })
                            .GroupBy(x => new { x.CourtName })
                            .OrderBy(x => x.Key.CourtName)
                            .Select(x => new ReportCaseStatsVM
                            {
                                CourtName = x.Key.CourtName,
                                Grd = x.Sum(s => s.G),
                                Adm = x.Sum(s => s.A),
                                Nkz = x.Sum(s => s.N),
                                Frm = x.Sum(s => s.F),
                                Trg = x.Sum(s => s.T)
                            }).ToListAsync();

            var totalRow = new ReportCaseStatsVM()
            {
                CourtName = "ОБЩО:",
                Grd = result.Sum(x => x.Grd),
                Adm = result.Sum(x => x.Adm),
                Nkz = result.Sum(x => x.Nkz),
                Frm = result.Sum(x => x.Frm),
                Trg = result.Sum(x => x.Trg),
            };
            return result.Prepend(totalRow).AsQueryable();

        }

        public async Task<byte[]> ReportCaseStatExcel(FilterCaseVM filter)
        {
            NPoiExcelService excelService = new NPoiExcelService("Sheet1");
            var dataRows =(await ReportCaseStat(filter)).ToList();

            var filterText = "";
            if (filter.CourtId > 0)
                filterText += $"Съд: {await repo.GetPropByIdAsync<Court, string>(x => x.CourtId == filter.CourtId, x => x.Name)}; ";
            if (filter.RegYear > 0)
                filterText += $"Година: {filter.RegYear}; ";


            var styleTitle = excelService.CreateTitleStyle();
            excelService.AddRange($"Брой дела по видове дела", 7,
                      styleTitle); excelService.AddRow();
            excelService.AddRange(filterText, 7,
                      styleTitle); excelService.AddRow();

            excelService.AddList(
                dataRows,
                new int[] { 10000, 5000, 5000, 5000, 5000, 5000, 5000 },
                new List<Expression<Func<ReportCaseStatsVM, object>>>()
                {
                    x => x.CourtName,
                    x => x.Grd,
                    x => x.Nkz,
                    x => x.Trg,
                    x => x.Frm,
                    x => x.Adm,
                    x => x.Total
                },
                //NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index,
                //NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index,
                NPOI.HSSF.Util.HSSFColor.White.Index
            );
            excelService.AddRow();
            excelService.AddRow();
            excelService.AddRangeMoveCol(dataRows.Count + " бр. записи отговарящи на зададените критерии.", 2, 1);

            return excelService.ToArray();
        }
    }
}
