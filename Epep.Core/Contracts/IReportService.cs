using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Report;

namespace Epep.Core.Contracts
{
    public interface IReportService : IBaseService
    {
        Task<IQueryable<ReportCaseStatsVM>> ReportCaseStat(FilterCaseVM filter);
        Task<byte[]> ReportCaseStatExcel(FilterCaseVM filter);
        Task<IQueryable<ReportLawyerViewVM>> ReportLawyerView(FilterLawyerViewVM filter);
        Task<byte[]> ReportLawyerViewExcel(FilterLawyerViewVM filter);
        Task<IQueryable<ReportUserAssignmentVM>> ReportUserAssignments(Guid userGid);
        Task<byte[]> ReportUserAssignmentsExcel(Guid userGid);
    }
}
