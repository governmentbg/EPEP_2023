using Epep.Core.Constants;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using IOWebApplication.Infrastructure.Models.ViewModels.Common;

namespace Epep.Core.Contracts
{
    public interface ICaseService : IBaseService
    {
        Task<bool> CheckCaseAccess(long caseId);
        Task<bool> CheckCaseAccess(Guid caseGid);
        Task<CalendarDateVM> GetCalendarDateEvents(DateTime date);
        Task<bool> HasScannedFiles(Guid caseGid);
        Task<FilesPreviewVM> LoadFilesForObject(int type, Guid gid);
        void ProcessCaseNames(CaseVM model, DateTime UpgradeEpepDateStart, bool publicAccess, int userTypeId, long userCourtId);
        Task<List<SideVM>> ProcessSideNames(List<SideVM> sides, DateTime UpgradeEpepDateStart, Guid caseGid, int userTypeId, long userCourtId);
        Task SaveCaseView(Guid gid, int viewMode = NomenclatureConstants.FocusTypes.View);
        Task<IEnumerable<HearingParticipantVM>> SelectActPreparatorsByAct(GidLoaderVM loader);
        Task<IQueryable<ActVM>> SelectActsByCaseGid(GidLoaderVM loader);
        Task<IQueryable<AssignmentVM>> SelectAssignmentsByCaseGid(GidLoaderVM loader);
        Task<IEnumerable<CalendarVM>> SelectCalendarByUser(DateTime from, DateTime to);
        Task<IQueryable<CaseVM>> SelectCase(FilterCaseVM filter);
        Task<IQueryable<CaseElementVM>> SelectChronologyByCase(GidLoaderVM loader);
        Task<IEnumerable<ConnectedCaseVM>> SelectConnectedCaseByCaseGid(GidLoaderVM loader);
        Task<IQueryable<DocumentVM>> SelectDocumentsByCaseGid(GidLoaderVM loader);
        Task<IQueryable<CaseElementVM>> SelectHearingItemsByHearing(GidLoaderVM loader);
        Task<IQueryable<HearingOnlineVM>> SelectHearingOnline(FilterHearingVM filter);
        Task<IEnumerable<HearingParticipantVM>> SelectHearingParticipantByHearing(GidLoaderVM loader);
        Task<List<FileItemVM>> SelectHearingProtocolByHearingGid(Guid gid);
        Task<IQueryable<HearingVM>> SelectHearingsByCaseGid(GidLoaderVM loader);
        IQueryable<SummonVM> SelectLastSummonsByUser();
        IQueryable<CaseVM> SelectLastViewedCases();
        IQueryable<HearingVM> SelectNextHearingsByUser();
        Task<IQueryable<SideVM>> SelectSidesByCaseGid(GidLoaderVM loader);
        Task<IQueryable<SummonVM>> SelectSummonsByCaseGid(GidLoaderVM loader);
        Task<IQueryable<UserWithAccessVM>> SelectUsersWithAccessByCaseGid(GidLoaderVM loader);
        Task<SaveResultVM> ToggleFocusCase(Guid gid, int focus);
    }
}
