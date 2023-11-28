using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("ElectronicPayments")]
    public class ElectronicPayment : IGidRoot
    {
        [Key]
        public long Id { get; set; }
        public Guid Gid { get; set; }
        public long MoneyObligationId { get; set; }

        public string InvoiceNumber { get; set; }

        public long MoneyCurrencyId { get; set; }
        [Precision(18, 2)]
        public decimal MoneyAmount { get; set; }
        public string PaymentKey { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateTime? FailDate { get; set; }
        public string FailDescription { get; set; }

        [ForeignKey(nameof(MoneyObligationId))]
        public virtual MoneyObligation Obligation { get; set; }

        [ForeignKey(nameof(MoneyCurrencyId))]
        public virtual MoneyCurrency MoneyCurrency { get; set; }
    }

}
