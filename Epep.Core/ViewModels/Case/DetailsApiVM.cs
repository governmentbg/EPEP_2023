using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Case
{
    public class DetailsApiVM
    {
        public string TypeName { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }

        public IEnumerable<CaseElementVM> CaseElements { get; set; }
        public IEnumerable<DetailsParticipants> Participants { get; set; }

        public IEnumerable<FileItemApiVM> Files { get; set; }
    }

    public class DetailsParticipants
    {
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
