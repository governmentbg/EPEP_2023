using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Payment
{
    public class TestPaymentVM
    {
        [Display(Name = "Съд")]
        public long CourtId { get; set; }

        [Display(Name = "Сума")]
        public decimal Amount { get; set; }
    }
}
