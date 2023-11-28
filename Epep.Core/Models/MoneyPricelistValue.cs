using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("MoneyPricelistValues")]
    public class MoneyPricelistValue
    {
        [Key]
        public long Id { get; set; }

        public long MoneyPricelistId { get; set; }

        [Display(Name ="Валута")]
        public long MoneyCurrencyId { get; set; }

        [Display(Name ="Тип стойност")]
        public int Type { get; set; }

        [Display(Name ="Стойност")]
        [Precision(18, 2)]
        public decimal? Value { get; set; }
        [Display(Name ="Минимална стойност")]
        [Precision(18, 2)]
        public decimal? MinValue { get; set; }
        [Display(Name ="Процент (0-100)")]
        [Precision(18, 2)]
        public decimal? Procent { get; set; }

        [Display(Name ="Начална дата")]
        public DateTime DateFrom { get; set; }
        [Display(Name ="Крайна дата")]
        public DateTime? DateTo { get; set; }

        [ForeignKey(nameof(MoneyCurrencyId))]
        public virtual MoneyCurrency Currency { get; set; }

        [ForeignKey(nameof(MoneyPricelistId))]
        public virtual MoneyPricelist Pricelist { get; set; }
    }
}
