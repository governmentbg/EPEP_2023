using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("MoneyPricelistDocuments")]
    public class MoneyPricelistDocument
    {
        [Key]
        public long Id { get; set; }

        public long MoneyPricelistId { get; set; }

        public long ElectronicDocumentTypeId { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [ForeignKey(nameof(MoneyPricelistId))]
        public virtual MoneyPricelist Pricelist { get; set; }

        [ForeignKey(nameof(ElectronicDocumentTypeId))]
        public virtual ElectronicDocumentType ElectronicDocumentType { get; set; }
    }
}
