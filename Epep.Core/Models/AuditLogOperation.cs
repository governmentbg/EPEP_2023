using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    /// <summary>
    /// Журнал на промените - видове операции
    /// </summary>
    [Table("AuditLogOperations")]
    public class AuditLogOperation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
