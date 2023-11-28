using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCase.Domain.Entities.Upgrade
{
    [Table("MoneyPricelists")]
    public class MoneyPricelist
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
    }
}
