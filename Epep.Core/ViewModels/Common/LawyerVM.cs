using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Common
{
    public class LawyerVM
    {
        public Guid Gid { get; set; }
        public string LawyerTypeName { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Uic { get; set; }
        public string College { get; set; }
    }

    public class FilterLawyerVM
    {
        [Display(Name = "Номер")]
        public string Number { get; set; }
        [Display(Name = "Имена")]
        public string Name { get; set; }
        public string Uic { get; set; }
        [Display(Name = "Колегия")]
        public string College { get; set; }

        public void Sanitize()
        {
            Number = Number.EmptyToNull();
            Name = Name.EmptyToNull();
            Uic = Uic.EmptyToNull();
            College = College.EmptyToNull();
        }
    }
}
