using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models

{
    [Table("UserVacationTypes")]
    public partial class UserVacationType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}