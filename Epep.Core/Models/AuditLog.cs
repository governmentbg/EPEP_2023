using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    /// <summary>
    /// Журнал на промените
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }      

        public DateTime DateWrt { get; set; }

        public int OperationId { get; set; }

        public string ObjectInfo { get; set; }

        public string ActionInfo { get; set; }

        public string RequestUrl { get; set; }

        public string ClientIP { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserRegistration User { get; set; }
       
        [ForeignKey(nameof(OperationId))]
        public virtual AuditLogOperation Operation { get; set; }
    }

}
