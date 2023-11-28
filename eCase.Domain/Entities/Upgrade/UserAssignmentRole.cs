using System.ComponentModel.DataAnnotations.Schema;

namespace eCase.Domain.Entities.Upgrade

{
    [Table("UserAssignmentRoles")]
    public partial class UserAssignmentRole
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}