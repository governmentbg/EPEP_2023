using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models

{
    [Table("UserRegistrationTypes")]
    public partial class UserRegistrationType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}