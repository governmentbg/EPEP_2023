using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Epep.Core.Models
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
        [Display(Name = "Етикет")]
        public string ShortName { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "В сила от")]
        public DateTime DateFrom { get; set; }
        [Display(Name = "В сила до")]
        public DateTime? DateTo { get; set; }

        [NotMapped]
        [JsonInclude]
        public string DocumentsIds { get; set; }

        [NotMapped]
        [Display(Name = "Приложимо за")]
        public string DocumentsList { get; set; }

        public virtual ICollection<MoneyPricelistDocument> PricelistDocuments { get; set; }

        public MoneyPricelist()
        {
            PricelistDocuments = new HashSet<MoneyPricelistDocument>();
        }
    }
}
