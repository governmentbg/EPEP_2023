using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eCase.Web.Models.Statistics
{
    public class CasesCountVM
    {
        public List<CourtInfo> Courts { get; set; }
        public long TotalCount { get; set; }

        public int? Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }

    public class CourtInfo
    {
        public string CourtName { get; set; }
        public int CasesCount { get; set; }
    }
}
