using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Common
{
    public class CourtListVM
    {
        public long CourtId { get; set; }
        public long CourtTypeId { get; set; }
        public string CourtType { get; set; }
        public string CourtName { get; set; }
        public bool HasElectronicDocument { get; set; }
        public bool HasElectronicPayment { get; set; }
        public string Url { get; set; }
        public bool isActive { get; set; }
    }

    public class FilterCourtVM
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Вид")]
        public long? CourtTypeId { get; set; }

        public void Sanitize()
        {
            Name = Name.EmptyToNull();
            CourtTypeId = CourtTypeId.EmptyToNull();
        }
    }
}
