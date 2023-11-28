using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    public partial class ElectronicDocumentSide : IGidRoot
    {
        [Key]
        public long Id { get; set; }
        public Guid Gid { get; set; }
        public long ElectronicDocumentId { get; set; }
        public long SideInvolvementKindId { get; set; }
        public long SubjectId { get; set; }

        [ForeignKey(nameof(ElectronicDocumentId))]
        public virtual ElectronicDocument ElectronicDocument { get; set; }

        [ForeignKey(nameof(SideInvolvementKindId))]
        public virtual SideInvolvementKind SideInvolvementKind { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }
    }

}
