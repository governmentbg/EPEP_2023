using Epep.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.User
{
    public class UserAccessVM
    {
        public Guid CaseGid { get; set; }
        public Guid SideGid { get; set; }

        [Display(Name = "Страна")]
        public string SideName { get; set; }

        [Display(Name = "Качество")]
        public string SideRole { get; set; }

        public bool HasAccess { get; set; }
        public int RequestAccess { get; set; }
        public int RequestSummon { get; set; }
    }
}
