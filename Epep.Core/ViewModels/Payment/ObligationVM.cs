using Epep.Core.Convertors;
using Epep.Core.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Payment
{
    public class ObligationVM
    {
        public Guid Gid { get; set; }
        public DateTime CreateDate { get; set; }
        public string ObligationTypeName { get; set; }
        public string CurrencyName { get; set; }
        public string ParentDescription { get; set; }
        public string Description { get; set; }
        public decimal MoneyAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public bool IsNotPaid
        {
            get
            {
                return PaymentDate == null;
            }
        }
    }

    public class FilterObligationVM
    {
        [Display(Name = "Дата от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Дата до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Статус")]
        public int? PaymentStatus { get; set; }

        [Display(Name = "Задължение към")]
        public string Description { get; set; }

        public void Sanitize()
        {
            PaymentStatus = PaymentStatus.EmptyToNull();
            Description = Description.EmptyToNull();
        }
    }
}
