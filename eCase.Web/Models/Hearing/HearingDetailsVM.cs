using System.Collections.Generic;
using System.Linq;

using PagedList;

namespace eCase.Web.Models.Hearing
{
    public class HearingDetailsVM
    {
        public int? sPage { get; set; }

        public eCase.Domain.Entities.Hearing Hearing { get; set; }

        public IPagedList<eCase.Domain.Entities.Summon> Summons { get; set; }
        public IEnumerable<eCase.Domain.Entities.HearingDocument> HearingDocuments { get; set; }

        public List<eCase.Domain.Entities.AttachedDocument> AttachedDocument { get; set; }

        public int SummonsCount
        {
            get
            {
                return this.Summons.Where(s => !s.IsRead).Count();
            }
        }

        public bool HasPermissions { get; set; }

        public bool HasProtocolFile { get; set; }
    }
}
