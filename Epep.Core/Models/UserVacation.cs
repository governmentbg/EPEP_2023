using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("UserVacations")]
    public class UserVacation
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        [Display(Name = "Вид")]
        public int VacationTypeId { get; set; }

        [Display(Name = "Дата от")]
        public DateTime DateFrom { get; set; }
        [Display(Name = "Дата до")]
        public DateTime DateTo { get; set; }

        [Display(Name = "Номер документ")]
        public string DocumentNumber { get; set; }

        public DateTime DateWrt { get; set; }

        public DateTime? DateExpire { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserRegistration UserRegistration { get; set; }

        [ForeignKey(nameof(VacationTypeId))]
        public virtual UserVacationType VacationType { get; set; }
    }
}
