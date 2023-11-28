using System.Collections.Generic;
using System.Linq;

using PagedList;

namespace eCase.Web.Models.Case
{
    public class CaseDetailsVM
    {
        public int? sumPage { get; set; }
        public int? actSumPage { get; set; }
        public int? appSumPage { get; set; }
        public int? hearSumPage { get; set; }

        public int? hPage { get; set; }
        public int? aPage { get; set; }
        public int? asPage { get; set; }
        public int? ccPage { get; set; }
        public int? idPage { get; set; }
        public int? odPage { get; set; }
        public int? sdPage { get; set; }

        public eCase.Domain.Entities.Case Case { get; set; }

        public IPagedList<eCase.Domain.Entities.Summon> Summons { get; set; }
        public IPagedList<eCase.Domain.Entities.Summon> ActSummons { get; set; }
        public IPagedList<eCase.Domain.Entities.Summon> AppealSummons { get; set; }
        public IPagedList<eCase.Domain.Entities.Summon> HearingSummons { get; set; }
        public IPagedList<eCase.Domain.Entities.Hearing> Hearings { get; set; }
        public IPagedList<eCase.Domain.Entities.Act> Acts { get; set; }
        public IPagedList<eCase.Domain.Entities.Appeal> Appeals { get; set; }
        public IPagedList<eCase.Domain.Entities.Assignment> Assignments { get; set; }
        public IPagedList<eCase.Domain.Entities.Case> ConnectedCases { get; set; }
        public IPagedList<eCase.Domain.Entities.CaseRuling> CaseRulings { get; set; }
        public IPagedList<eCase.Domain.Entities.IncomingDocument> IncomingDocuments { get; set; }
        public IPagedList<eCase.Domain.Entities.OutgoingDocument> OutgoingDocuments { get; set; }
        public IPagedList<eCase.Domain.Entities.ScannedFile> ScannedFiles { get; set; }
        public List<eCase.Domain.Entities.AttachedDocument> AttachedDocument { get; set; }

        public int SummonsCount { get { return this.Summons.Where(s => !s.IsRead).Count(); } }
        public int ActSummonsCount { get { return this.ActSummons.Where(s => !s.IsRead).Count(); } }
        public int AppealSummonsCount { get { return this.AppealSummons.Where(s => !s.IsRead).Count(); } }
        public int HearingSummonsCount { get { return this.HearingSummons.Where(s => !s.IsRead).Count(); } }

        public bool HasPermissions { get; set; }
    }
}