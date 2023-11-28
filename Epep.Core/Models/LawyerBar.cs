using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("LawyerBar")]
    public partial class LawyerBar
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Статус")] 
        public long? LawyerStateId { get; set; }
        public string LawyerStateName { get; set; }

        [Display(Name = "Вид")]
        public long? LawyerTypeId { get; set; }
        public string LawyerTypeName { get; set; }
        [Display(Name = "Имена")]
        public string Name { get; set; }
        [Display(Name = "Номер")]
        public string Number { get; set; }
        [Display(Name = "Идентификатор")]
        public string Uic { get; set; }
        [Display(Name = "Колегия")]
        public string College { get; set; }

        public DateTime DateWrt { get; set; }

        [ForeignKey(nameof(LawyerTypeId))]
        public virtual LawyerType LawyerType { get; set; }

        [ForeignKey(nameof(LawyerStateId))]
        public virtual LawyerState LawyerState { get; set; }
    }

    
}
