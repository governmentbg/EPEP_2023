using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Case
{
    public class ActVM
    {
        public Guid Gid { get; set; }
        public string ActKindName { get; set; }
        public int? Number { get; set; }
        public DateTime DateSigned { get; set; }
        public DateTime? DateInPower { get; set; }
        public ActPreparatorVM[] Preparators { get; set; }
    }

    public class ActPreparatorVM
    {
        public string JudgeName { get; set; }
        public string JudgeRole { get; set; }
    }
}
