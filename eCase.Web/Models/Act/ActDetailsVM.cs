using System.Collections.Generic;
using System.Linq;

using PagedList;

namespace eCase.Web.Models.Act
{
    public class ActDetailsVM
    {
        public int? sPage { get; set; }
        public eCase.Domain.Entities.Act Act { get; set; }
        public IPagedList<eCase.Domain.Entities.Summon> Summons { get; set; }
        public List<eCase.Domain.Entities.AttachedDocument> AttachedDocument { get; set; }


        public int SummonsCount
        {
            get
            {
                return this.Summons.Where(s => !s.IsRead).Count();
            }
        }

        public bool HasPermissions { get; set; }
        public bool HasActFile { get; set; }
        public bool HasMotiveFile { get; set; }
        public bool IsCriminal { get; set; }
        public bool HasPublicActFile { get; set; }
        public bool HasPublicMotiveFile { get; set; }
    }
}
