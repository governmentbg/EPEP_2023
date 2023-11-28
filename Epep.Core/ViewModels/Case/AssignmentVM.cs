using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Case
{
    public class AssignmentVM
    {
        public Guid Gid { get; set; }
        public string JudgeName { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Assignor { get; set; }
        public Guid? BlobKey { get; set; }
    }
}
